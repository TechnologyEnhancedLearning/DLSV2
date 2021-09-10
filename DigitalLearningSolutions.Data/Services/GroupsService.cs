namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using MimeKit;

    public interface IGroupsService
    {
        void SynchroniseUserChangesWithGroups(
            DelegateUser oldDelegateDetails,
            AccountDetailsData newDelegateDetails,
            CentreAnswersData newCentreAnswers,
            string baseUrl
        );

        void EnrolDelegateOnGroupCourses(
            DelegateUser oldDetails,
            AccountDetailsData newDetails,
            int groupId,
            string baseUrl,
            int? addedByAdminId = null
        );
    }

    public class GroupsService : IGroupsService
    {
        private const string AddedByProcess = "AddDelegateToGroup_Refactor";
        private const string EnrolEmailSubject = "New Learning Portal Course Enrolment";
        private readonly IClockService clockService;
        private readonly IEmailService emailService;
        private readonly IGroupsDataService groupsDataService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly ITutorialContentDataService tutorialContentDataService;
        private readonly IProgressDataService progressDataService;

        public GroupsService(
            IGroupsDataService groupsDataService,
            IClockService clockService,
            ITutorialContentDataService tutorialContentDataService,
            IEmailService emailService,
            IJobGroupsDataService jobGroupsDataService,
            IProgressDataService progressDataService
        )
        {
            this.groupsDataService = groupsDataService;
            this.clockService = clockService;
            this.tutorialContentDataService = tutorialContentDataService;
            this.emailService = emailService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.progressDataService = progressDataService;
        }

        public void SynchroniseUserChangesWithGroups(
            DelegateUser oldDelegateDetails,
            AccountDetailsData newDelegateDetails,
            CentreAnswersData newCentreAnswers,
            string baseUrl
        )
        {
            var changedLinkedFields = LinkedFieldHelper.GetLinkedFieldNumbersWithValuesFromChangedAnswers(
                oldDelegateDetails,
                newCentreAnswers,
                jobGroupsDataService
            );

            var allSynchronisedGroupsAtCentre =
                GetSynchronisedGroupsForCentre(oldDelegateDetails.CentreId).ToList();

            foreach (var changedAnswer in changedLinkedFields)
            {
                var groupToRemoveDelegateFrom = allSynchronisedGroupsAtCentre.SingleOrDefault(
                    g => g.LinkedToField == changedAnswer.LinkedFieldNumber && g.GroupLabel == changedAnswer.OldValue
                );

                var groupToAddDelegateTo = allSynchronisedGroupsAtCentre.SingleOrDefault(
                    g => g.LinkedToField == changedAnswer.LinkedFieldNumber && g.GroupLabel == changedAnswer.NewValue
                );

                using var transaction = new TransactionScope();
                if (groupToRemoveDelegateFrom != null)
                {
                    RemoveDelegateFromGroup(oldDelegateDetails.Id, groupToRemoveDelegateFrom.GroupId);
                }

                if (groupToAddDelegateTo != null)
                {
                    groupsDataService.AddDelegateToGroup(
                        oldDelegateDetails.Id,
                        groupToAddDelegateTo.GroupId,
                        clockService.UtcNow,
                        1
                    );

                    EnrolDelegateOnGroupCourses(
                        oldDelegateDetails,
                        newDelegateDetails,
                        groupToAddDelegateTo.GroupId,
                        baseUrl
                    );
                }

                transaction.Complete();
            }
        }

        public void EnrolDelegateOnGroupCourses(
            DelegateUser oldDetails,
            AccountDetailsData newDetails,
            int groupId,
            string baseUrl,
            int? addedByAdminId = null
        )
        {
            var groupCourses = groupsDataService.GetGroupCourses(groupId, oldDetails.CentreId);

            foreach (var groupCourse in groupCourses)
            {
                DateTime? completeByDate = null;
                if (groupCourse.CompleteWithinMonths != 0)
                {
                    completeByDate = clockService.UtcNow.AddMonths(groupCourse.CompleteWithinMonths);
                }

                var candidateProgressOnCourse =
                    progressDataService.GetDelegateProgressForCourse(oldDetails.Id, groupCourse.CustomisationId);
                var existingRecordToUpdate = candidateProgressOnCourse.FirstOrDefault(
                    p => !p.SystemRefreshed && p.Completed == null && p.RemovedDate == null
                );

                if (existingRecordToUpdate != null)
                {
                    var updatedSupervisorAdminId = groupCourse.SupervisorAdminId > 0
                        ? groupCourse.SupervisorAdminId.Value
                        : existingRecordToUpdate.SupervisorAdminId;
                    progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                        existingRecordToUpdate.ProgressId,
                        updatedSupervisorAdminId,
                        completeByDate
                    );
                }
                else
                {
                    var newProgressId = progressDataService.CreateNewDelegateProgress(
                        oldDetails.Id,
                        groupCourse.CustomisationId,
                        groupCourse.CurrentVersion,
                        clockService.UtcNow,
                        3,
                        addedByAdminId,
                        completeByDate,
                        groupCourse.SupervisorAdminId ?? 0
                    );

                    var tutorialsForCourse =
                        tutorialContentDataService.GetTutorialsForCourse(groupCourse.CustomisationId);

                    foreach (var tutorial in tutorialsForCourse)
                    {
                        progressDataService.CreateNewAspProgress(tutorial, newProgressId);
                    }
                }

                if (newDetails.Email != null)
                {
                    var fullName = newDetails.FirstName + " " + newDetails.Surname;
                    var email = BuildEnrolmentEmail(newDetails.Email, fullName, groupCourse, baseUrl, completeByDate);
                    emailService.ScheduleEmail(email, AddedByProcess);
                }
            }
        }

        private IEnumerable<Group> GetSynchronisedGroupsForCentre(int centreId)
        {
            return groupsDataService.GetGroupsForCentre(centreId)
                .Where(g => g.ChangesToRegistrationDetailsShouldChangeGroupMembership);
        }

        private void RemoveDelegateFromGroup(int delegateId, int groupId)
        {
            groupsDataService.DeleteGroupDelegatesRecordForDelegate(groupId, delegateId);
            groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(groupId, delegateId, clockService.UtcNow);
        }

        private Email BuildEnrolmentEmail(
            string emailAddress,
            string fullName,
            GroupCourse course,
            string baseUrl,
            DateTime? completeByDate
        )
        {
            var linkToLearningPortal = baseUrl + "/LearningPortal/Current";
            var linkToCourse = baseUrl + "/LearningMenu/" + course.CustomisationId;
            string emailBodyText = $@"
                Dear {fullName}
                This is an automated message to notify you that you have been enrolled on the course 
                {course.CourseName}
                by the system because a previous course completion has expired.
                To login to the course directly click here:{linkToCourse}.
                To login to the Learning Portal to access and complete your course click here: 
                {linkToLearningPortal}.";
            string emailBodyHtml = $@"
                <p>Dear {fullName}</p>
                <p>This is an automated message to notify you that you have been enrolled on the course 
                <b>{course.CourseName}</b> 
                by the system because a previous course completion has expired.</p>
                <p>To login to the course directly <a href=""{linkToCourse}"">click here</a>.</p>
                <p>To login to the Learning Portal to access and complete your course 
                <a href=""{linkToLearningPortal}"">click here</a>.</p>";

            if (completeByDate != null)
            {
                emailBodyText += $"The date the course should be completed by is {completeByDate.Value:dd/MM/yyyy}";
                emailBodyHtml +=
                    $"<p>The date the course should be completed by is {completeByDate.Value:dd/MM/yyyy}</p>";
            }

            var body = new BodyBuilder
            {
                TextBody = emailBodyText,
                HtmlBody = emailBodyHtml
            };

            return new Email(EnrolEmailSubject, body, emailAddress);
        }
    }
}
