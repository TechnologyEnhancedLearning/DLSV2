﻿namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Configuration;
    using MimeKit;

    public interface IGroupsService
    {
        int AddDelegateGroup(int centreId, string groupLabel, string? groupDescription, int adminUserId);

        void SynchroniseUserChangesWithGroups(
            DelegateUser delegateAccountWithOldDetails,
            AccountDetailsData newDelegateDetails,
            CentreAnswersData newCentreAnswers
        );

        void EnrolDelegateOnGroupCourses(
            DelegateUser delegateAccountWithOldDetails,
            AccountDetailsData newDetails,
            int groupId,
            int? addedByAdminId = null
        );

        void DeleteDelegateGroup(int groupId, bool deleteStartedEnrolment);

        IEnumerable<Group> GetGroupsForCentre(int centreId);

        IEnumerable<GroupDelegate> GetGroupDelegates(int groupId);

        IEnumerable<GroupCourse> GetGroupCourses(int groupId, int centreId);

        string? GetGroupName(int groupId, int centreId);

        int? GetRelatedProgressIdForGroupDelegate(int groupId, int delegateId);

        Group? GetGroupAtCentreById(int groupId, int centreId);

        void UpdateGroupDescription(int groupId, int centreId, string? groupDescription);

        void RemoveDelegateFromGroup(
            int groupId,
            int delegateId,
            bool removeStartedEnrolments
        );

        int? GetGroupCentreId(int groupId);
    }

    public class GroupsService : IGroupsService
    {
        private const string AddedByProcess = "AddDelegateToGroup_Refactor";
        private const string EnrolEmailSubject = "New Learning Portal Course Enrolment";
        private readonly ICentreCustomPromptsService centreCustomPromptsService;
        private readonly IClockService clockService;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly IGroupsDataService groupsDataService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IProgressDataService progressDataService;
        private readonly ITutorialContentDataService tutorialContentDataService;

        public GroupsService(
            IGroupsDataService groupsDataService,
            IClockService clockService,
            ITutorialContentDataService tutorialContentDataService,
            IEmailService emailService,
            IJobGroupsDataService jobGroupsDataService,
            IProgressDataService progressDataService,
            IConfiguration configuration,
            ICentreCustomPromptsService centreCustomPromptsService
        )
        {
            this.groupsDataService = groupsDataService;
            this.clockService = clockService;
            this.tutorialContentDataService = tutorialContentDataService;
            this.emailService = emailService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.progressDataService = progressDataService;
            this.configuration = configuration;
            this.centreCustomPromptsService = centreCustomPromptsService;
        }

        public void SynchroniseUserChangesWithGroups(
            DelegateUser delegateAccountWithOldDetails,
            AccountDetailsData newDelegateDetails,
            CentreAnswersData newCentreAnswers
        )
        {
            var changedLinkedFields = LinkedFieldHelper.GetLinkedFieldChanges(
                delegateAccountWithOldDetails.GetCentreAnswersData(),
                newCentreAnswers,
                jobGroupsDataService,
                centreCustomPromptsService
            );

            var allSynchronisedGroupsAtCentre =
                GetSynchronisedGroupsForCentre(delegateAccountWithOldDetails.CentreId).ToList();

            foreach (var changedAnswer in changedLinkedFields)
            {
                var groupToRemoveDelegateFrom = allSynchronisedGroupsAtCentre.SingleOrDefault(
                    g => g.LinkedToField == changedAnswer.LinkedFieldNumber &&
                         GroupLabelMatchesAnswer(g.GroupLabel, changedAnswer.OldValue, changedAnswer.LinkedFieldName)
                );

                var groupToAddDelegateTo = allSynchronisedGroupsAtCentre.SingleOrDefault(
                    g => g.LinkedToField == changedAnswer.LinkedFieldNumber &&
                         GroupLabelMatchesAnswer(g.GroupLabel, changedAnswer.NewValue, changedAnswer.LinkedFieldName)
                );

                using var transaction = new TransactionScope();
                if (groupToRemoveDelegateFrom != null)
                {
                    RemoveDelegateFromGroup(delegateAccountWithOldDetails.Id, groupToRemoveDelegateFrom.GroupId);
                }

                if (groupToAddDelegateTo != null)
                {
                    groupsDataService.AddDelegateToGroup(
                        delegateAccountWithOldDetails.Id,
                        groupToAddDelegateTo.GroupId,
                        clockService.UtcNow,
                        1
                    );

                    EnrolDelegateOnGroupCourses(
                        delegateAccountWithOldDetails,
                        newDelegateDetails,
                        groupToAddDelegateTo.GroupId
                    );
                }

                transaction.Complete();
            }
        }

        public void EnrolDelegateOnGroupCourses(
            DelegateUser delegateAccountWithOldDetails,
            AccountDetailsData newDetails,
            int groupId,
            int? addedByAdminId = null
        )
        {
            var groupCourses = groupsDataService.GetGroupCourses(groupId, delegateAccountWithOldDetails.CentreId);

            foreach (var groupCourse in groupCourses)
            {
                var completeByDate = groupCourse.CompleteWithinMonths != 0
                    ? (DateTime?)clockService.UtcNow.AddMonths(groupCourse.CompleteWithinMonths)
                    : null;

                var candidateProgressOnCourse =
                    progressDataService.GetDelegateProgressForCourse(
                        delegateAccountWithOldDetails.Id,
                        groupCourse.CustomisationId
                    );
                var existingRecordsToUpdate =
                    candidateProgressOnCourse.Where(ProgressShouldBeUpdatedOnEnrolment).ToList();

                if (existingRecordsToUpdate.Any())
                {
                    foreach (var progressRecord in existingRecordsToUpdate)
                    {
                        var updatedSupervisorAdminId = groupCourse.SupervisorAdminId > 0
                            ? groupCourse.SupervisorAdminId.Value
                            : progressRecord.SupervisorAdminId;
                        progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                            progressRecord.ProgressId,
                            updatedSupervisorAdminId,
                            completeByDate
                        );
                    }
                }
                else
                {
                    var newProgressId = progressDataService.CreateNewDelegateProgress(
                        delegateAccountWithOldDetails.Id,
                        groupCourse.CustomisationId,
                        groupCourse.CurrentVersion,
                        clockService.UtcNow,
                        3,
                        addedByAdminId,
                        completeByDate,
                        groupCourse.SupervisorAdminId ?? 0
                    );

                    var tutorialsForCourse =
                        tutorialContentDataService.GetTutorialIdsForCourse(groupCourse.CustomisationId);

                    foreach (var tutorial in tutorialsForCourse)
                    {
                        progressDataService.CreateNewAspProgress(tutorial, newProgressId);
                    }
                }

                if (newDetails.Email != null)
                {
                    var fullName = newDetails.FirstName + " " + newDetails.Surname;
                    var email = BuildEnrolmentEmail(newDetails.Email, fullName, groupCourse, completeByDate);
                    emailService.ScheduleEmail(email, AddedByProcess);
                }
            }
        }

        public int AddDelegateGroup(int centreId, string groupLabel, string? groupDescription, int adminUserId)
        {
            var groupDetails = new GroupDetails
            {
                CentreId = centreId,
                GroupLabel = groupLabel,
                GroupDescription = groupDescription,
                AdminUserId = adminUserId,
                CreatedDate = clockService.UtcNow,
                LinkedToField = 0,
                SyncFieldChanges = false,
                AddNewRegistrants = false,
                PopulateExisting = false,
            };

            return groupsDataService.AddDelegateGroup(groupDetails);
        }

        public void DeleteDelegateGroup(int groupId, bool deleteStartedEnrolment)
        {
            using var transaction = new TransactionScope();

            groupsDataService.RemoveRelatedProgressRecordsForGroup(
                groupId,
                deleteStartedEnrolment,
                clockService.UtcNow
            );
            groupsDataService.DeleteGroupDelegates(groupId);
            groupsDataService.DeleteGroupCustomisations(groupId);
            groupsDataService.DeleteGroup(groupId);

            transaction.Complete();
        }

        public IEnumerable<Group> GetGroupsForCentre(int centreId)
        {
            return groupsDataService.GetGroupsForCentre(centreId);
        }

        public IEnumerable<GroupDelegate> GetGroupDelegates(int groupId)
        {
            return groupsDataService.GetGroupDelegates(groupId);
        }

        public IEnumerable<GroupCourse> GetGroupCourses(int groupId, int centreId)
        {
            return groupsDataService.GetGroupCourses(groupId, centreId);
        }

        public string? GetGroupName(int groupId, int centreId)
        {
            return groupsDataService.GetGroupName(groupId, centreId);
        }

        public int? GetRelatedProgressIdForGroupDelegate(int groupId, int delegateId)
        {
            return groupsDataService.GetRelatedProgressIdForGroupDelegate(groupId, delegateId);
        }

        public Group? GetGroupAtCentreById(int groupId, int centreId)
        {
            return groupsDataService.GetGroupAtCentreById(groupId, centreId);
        }

        public void UpdateGroupDescription(int groupId, int centreId, string? groupDescription)
        {
            groupsDataService.UpdateGroupDescription(groupId, centreId, groupDescription);
        }

        public void RemoveDelegateFromGroup(
            int groupId,
            int delegateId,
            bool removeStartedEnrolments
        )
        {
            using var transaction = new TransactionScope();

            var currentDate = clockService.UtcNow;
            groupsDataService.RemoveRelatedProgressRecordsForGroup(
                groupId,
                delegateId,
                removeStartedEnrolments,
                currentDate
            );

            groupsDataService.DeleteGroupDelegatesRecordForDelegate(groupId, delegateId);
            transaction.Complete();
        }

        public int? GetGroupCentreId(int groupId)
        {
            return groupsDataService.GetGroupCentreId(groupId);
        }

        private IEnumerable<Group> GetSynchronisedGroupsForCentre(int centreId)
        {
            return groupsDataService.GetGroupsForCentre(centreId)
                .Where(g => g.ChangesToRegistrationDetailsShouldChangeGroupMembership);
        }

        private bool GroupLabelMatchesAnswer(string groupLabel, string? answer, string? linkedFieldName)
        {
            return groupLabel == answer || groupLabel == linkedFieldName + " - " + answer;
        }

        private static bool ProgressShouldBeUpdatedOnEnrolment(Progress progress)
        {
            return progress.Completed == null && progress.RemovedDate == null;
        }

        private void RemoveDelegateFromGroup(int delegateId, int groupId)
        {
            const bool removeStartedEnrolments = false;
            groupsDataService.RemoveRelatedProgressRecordsForGroup(
                groupId,
                delegateId,
                removeStartedEnrolments,
                clockService.UtcNow
            );
            groupsDataService.DeleteGroupDelegatesRecordForDelegate(groupId, delegateId);
        }

        private Email BuildEnrolmentEmail(
            string emailAddress,
            string fullName,
            GroupCourse course,
            DateTime? completeByDate
        )
        {
            var baseUrl = configuration.GetAppRootPath();
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
                HtmlBody = emailBodyHtml,
            };

            return new Email(EnrolEmailSubject, body, emailAddress);
        }
    }
}
