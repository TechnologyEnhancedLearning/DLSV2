namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public interface IGroupsService
    {
        int AddDelegateGroup(
            int centreId,
            string groupLabel,
            string? groupDescription,
            int adminUserId,
            int linkedToField = 0,
            bool syncFieldChanges = false,
            bool addNewRegistrants = false,
            bool populateExisting = false
        );

        void AddDelegateToGroupAndEnrolOnGroupCourses(
            int groupId,
            DelegateUser delegateUser,
            int? addedByAdminId = null
        );

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

        string? GetGroupName(int groupId, int centreId);

        int? GetRelatedProgressIdForGroupDelegate(int groupId, int delegateId);

        Group? GetGroupAtCentreById(int groupId, int centreId);

        void UpdateGroupDescription(int groupId, int centreId, string? groupDescription);

        void RemoveDelegateFromGroup(
            int groupId,
            int delegateId,
            bool removeStartedEnrolments
        );

        void RemoveGroupCourseAndRelatedProgress(int customisationId, int groupId, bool deleteStartedEnrolment);

        int? GetGroupCentreId(int groupId);

        IEnumerable<GroupCourse> GetUsableGroupCoursesForCentre(int groupId, int centreId);

        GroupCourse? GetUsableGroupCourseForCentre(int groupCustomisationId, int groupId, int centreId);

        IEnumerable<GroupCourse> GetGroupCoursesForCategory(int groupId, int centreId, int? categoryId);

        void UpdateGroupName(int groupId, int centreId, string groupName);

        void AddCourseToGroup(
            int groupId,
            int customisationId,
            int completeWithinMonths,
            int addedByAdminId,
            bool cohortLearners,
            int? supervisorAdminId,
            int centreId
        );

        void GenerateGroupsFromRegistrationField(GroupGenerationDetails groupDetails);
    }

    public class GroupsService : IGroupsService
    {
        private const string AddDelegateToGroupAddedByProcess = "AddDelegateToGroup_Refactor";
        private const string AddCourseToGroupAddedByProcess = "AddCourseToDelegateGroup_Refactor";
        private const string EnrolEmailSubject = "New Learning Portal Course Enrolment";
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IClockService clockService;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly IGroupsDataService groupsDataService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly ILogger<IGroupsService> logger;
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
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            ILogger<IGroupsService> logger
        )
        {
            this.groupsDataService = groupsDataService;
            this.clockService = clockService;
            this.tutorialContentDataService = tutorialContentDataService;
            this.emailService = emailService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.progressDataService = progressDataService;
            this.configuration = configuration;
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.logger = logger;
        }

        public int AddDelegateGroup(
            int centreId,
            string groupLabel,
            string? groupDescription,
            int adminUserId,
            int linkedToField = 0,
            bool syncFieldChanges = false,
            bool addNewRegistrants = false,
            bool populateExisting = false
        )
        {
            var groupDetails = new GroupDetails
            {
                CentreId = centreId,
                GroupLabel = groupLabel,
                GroupDescription = groupDescription,
                AdminUserId = adminUserId,
                CreatedDate = clockService.UtcNow,
                LinkedToField = linkedToField,
                SyncFieldChanges = syncFieldChanges,
                AddNewRegistrants = addNewRegistrants,
                PopulateExisting = populateExisting,
            };

            return groupsDataService.AddDelegateGroup(groupDetails);
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
                centreRegistrationPromptsService
            );

            var allSynchronisedGroupsAtCentre =
                GetSynchronisedGroupsForCentre(delegateAccountWithOldDetails.CentreId).ToList();

            foreach (var changedAnswer in changedLinkedFields)
            {
                var groupsToRemoveDelegateFrom = allSynchronisedGroupsAtCentre.Where(
                    g => g.LinkedToField == changedAnswer.LinkedFieldNumber &&
                         GroupLabelMatchesAnswer(g.GroupLabel, changedAnswer.OldValue, changedAnswer.LinkedFieldName)
                );

                var groupsToAddDelegateTo = allSynchronisedGroupsAtCentre.Where(
                    g => g.LinkedToField == changedAnswer.LinkedFieldNumber &&
                         GroupLabelMatchesAnswer(g.GroupLabel, changedAnswer.NewValue, changedAnswer.LinkedFieldName)
                );

                using var transaction = new TransactionScope();
                foreach (var groupToRemoveDelegateFrom in groupsToRemoveDelegateFrom)
                {
                    RemoveDelegateFromGroup(delegateAccountWithOldDetails.Id, groupToRemoveDelegateFrom.GroupId);
                }

                foreach (var groupToAddDelegateTo in groupsToAddDelegateTo)
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
            var groupCourses = GetUsableGroupCoursesForCentre(groupId, delegateAccountWithOldDetails.CentreId);
            var fullName = newDetails.FirstName + " " + newDetails.Surname;

            foreach (var groupCourse in groupCourses)
            {
                EnrolDelegateOnGroupCourse(
                    delegateAccountWithOldDetails.Id,
                    newDetails.Email,
                    fullName,
                    addedByAdminId,
                    groupCourse,
                    false
                );
            }
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

        public void AddDelegateToGroupAndEnrolOnGroupCourses(
            int groupId,
            DelegateUser delegateUser,
            int? addedByAdminId = null
        )
        {
            using var transaction = new TransactionScope();

            groupsDataService.AddDelegateToGroup(delegateUser.Id, groupId, clockService.UtcNow, 0);

            var accountDetailsData = new MyAccountDetailsData(
                delegateUser.Id,
                delegateUser.FirstName!,
                delegateUser.LastName,
                delegateUser.EmailAddress!
            );

            EnrolDelegateOnGroupCourses(
                delegateUser,
                accountDetailsData,
                groupId,
                addedByAdminId
            );

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

        public IEnumerable<GroupCourse> GetUsableGroupCoursesForCentre(int groupId, int centreId)
        {
            return groupsDataService.GetGroupCoursesVisibleToCentre(centreId)
                .Where(gc => gc.IsUsable && gc.GroupId == groupId);
        }

        public GroupCourse? GetUsableGroupCourseForCentre(int groupCustomisationId, int groupId, int centreId)
        {
            var groupCourse = groupsDataService.GetGroupCourseIfVisibleToCentre(groupCustomisationId, centreId);

            if ((!groupCourse?.IsUsable ?? true) || groupCourse.GroupId != groupId)
            {
                return null;
            }

            return groupCourse;
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

        public void RemoveGroupCourseAndRelatedProgress(
            int groupCustomisationId,
            int groupId,
            bool deleteStartedEnrolment
        )
        {
            using var transaction = new TransactionScope();

            groupsDataService.RemoveRelatedProgressRecordsForGroupCourse(
                groupId,
                groupCustomisationId,
                deleteStartedEnrolment,
                clockService.UtcNow
            );
            groupsDataService.DeleteGroupCustomisation(groupCustomisationId);
            transaction.Complete();
        }

        public int? GetGroupCentreId(int groupId)
        {
            return groupsDataService.GetGroupCentreId(groupId);
        }

        public IEnumerable<GroupCourse> GetGroupCoursesForCategory(int groupId, int centreId, int? categoryId)
        {
            return GetUsableGroupCoursesForCentre(groupId, centreId)
                .Where(gc => !categoryId.HasValue || categoryId == gc.CourseCategoryId);
        }

        public void UpdateGroupName(int groupId, int centreId, string groupName)
        {
            groupsDataService.UpdateGroupName(groupId, centreId, groupName);
        }

        public void AddCourseToGroup(
            int groupId,
            int customisationId,
            int completeWithinMonths,
            int addedByAdminId,
            bool cohortLearners,
            int? supervisorAdminId,
            int centreId
        )
        {
            using var transaction = new TransactionScope();

            var groupCustomisationId = groupsDataService.InsertGroupCustomisation(
                groupId,
                customisationId,
                completeWithinMonths,
                addedByAdminId,
                cohortLearners,
                supervisorAdminId
            );

            var groupDelegates = GetGroupDelegates(groupId);
            var groupCourse = groupsDataService.GetGroupCourseIfVisibleToCentre(groupCustomisationId, centreId);

            if (groupCourse == null)
            {
                transaction.Dispose();
                logger.LogError("Attempted to add a course that a centre does not have access to to a group.");
                throw new CourseAccessDeniedException(
                    $"No course with customisationId {customisationId} available at centre {centreId}"
                );
            }

            foreach (var groupDelegate in groupDelegates)
            {
                var fullName = groupDelegate.FirstName + " " + groupDelegate.LastName;
                EnrolDelegateOnGroupCourse(
                    groupDelegate.DelegateId,
                    groupDelegate.EmailAddress,
                    fullName,
                    addedByAdminId,
                    groupCourse,
                    true
                );
            }

            transaction.Complete();
        }

        public void GenerateGroupsFromRegistrationField(GroupGenerationDetails groupDetails)
        {
            var isJobGroup = groupDetails.RegistrationField.Equals(RegistrationField.JobGroup);
            var linkedToField = groupDetails.RegistrationField.LinkedToFieldId;

            (List<(int id, string name)> newGroupNames, string groupNamePrefix) = isJobGroup
                ? GetJobGroupsAndPrefix()
                : GetCustomPromptsAndPrefix(groupDetails.CentreId, groupDetails.RegistrationField.Id);

            var groupsAtCentre = GetGroupsForCentre(groupDetails.CentreId).Select(g => g.GroupLabel).ToList();

            using var transaction = new TransactionScope();
            foreach (var (id, newGroupName) in newGroupNames)
            {
                var groupName = groupDetails.PrefixGroupName ? $"{groupNamePrefix} - {newGroupName}" : newGroupName;

                if (groupDetails.SkipDuplicateNames && groupsAtCentre.Contains(groupName))
                {
                    continue;
                }

                var newGroupId = AddDelegateGroup(
                    groupDetails.CentreId,
                    groupName,
                    null,
                    groupDetails.AdminId,
                    linkedToField,
                    groupDetails.SyncFieldChanges,
                    groupDetails.AddNewRegistrants,
                    groupDetails.PopulateExisting
                );

                if (groupDetails.PopulateExisting)
                {
                    groupsDataService.AddDelegatesWithMatchingAnswersToGroup(
                        newGroupId,
                        clockService.UtcNow,
                        linkedToField,
                        groupDetails.CentreId,
                        isJobGroup ? null : newGroupName,
                        isJobGroup ? id : (int?)null
                    );
                }
            }

            transaction.Complete();
        }

        private void EnrolDelegateOnGroupCourse(
            int delegateUserId,
            string? delegateUserEmailAddress,
            string delegateUserFullName,
            int? addedByAdminId,
            GroupCourse groupCourse,
            bool isAddCourseToGroup
        )
        {
            var completeByDate = groupCourse.CompleteWithinMonths != 0
                ? (DateTime?)clockService.UtcNow.AddMonths(groupCourse.CompleteWithinMonths)
                : null;

            var candidateProgressOnCourse =
                progressDataService.GetDelegateProgressForCourse(
                    delegateUserId,
                    groupCourse.CustomisationId
                );
            var existingRecordsToUpdate =
                candidateProgressOnCourse.Where(
                    p => ProgressShouldBeUpdatedOnEnrolment(p, isAddCourseToGroup)
                ).ToList();

            if (existingRecordsToUpdate.Any())
            {
                foreach (var progressRecord in existingRecordsToUpdate)
                {
                    var updatedSupervisorAdminId = groupCourse.SupervisorAdminId > 0 && !isAddCourseToGroup
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
                    delegateUserId,
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

            if (delegateUserEmailAddress != null)
            {
                var email = BuildEnrolmentEmail(
                    delegateUserEmailAddress,
                    delegateUserFullName,
                    groupCourse,
                    completeByDate
                );
                var addedByProcess =
                    isAddCourseToGroup ? AddCourseToGroupAddedByProcess : AddDelegateToGroupAddedByProcess;
                emailService.ScheduleEmail(email, addedByProcess);
            }
        }

        private IEnumerable<Group> GetSynchronisedGroupsForCentre(int centreId)
        {
            return groupsDataService.GetGroupsForCentre(centreId)
                .Where(g => g.ChangesToRegistrationDetailsShouldChangeGroupMembership);
        }

        private static bool GroupLabelMatchesAnswer(string groupLabel, string? answer, string? linkedFieldName)
        {
            return string.Equals(groupLabel, answer, StringComparison.CurrentCultureIgnoreCase) || string.Equals(
                groupLabel,
                linkedFieldName + " - " + answer,
                StringComparison.CurrentCultureIgnoreCase
            );
        }

        private static bool ProgressShouldBeUpdatedOnEnrolment(
            Progress progress,
            bool isAddCourseToGroup
        )
        {
            if (isAddCourseToGroup)
            {
                return progress.Completed == null && progress.RemovedDate == null && !progress.SystemRefreshed;
            }

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

        private (List<(int id, string name)>, string groupNamePrefix) GetJobGroupsAndPrefix()
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            const string groupNamePrefix = "Job group";
            return (jobGroups, groupNamePrefix);
        }

        private (List<(int id, string name)>, string groupNamePrefix) GetCustomPromptsAndPrefix(
            int centreId,
            int registrationFieldOptionId
        )
        {
            var registrationPrompt = centreRegistrationPromptsService
                .GetCentreRegistrationPromptsThatHaveOptionsByCentreId(centreId).Single(
                    cp => cp.RegistrationField.Id == registrationFieldOptionId
                );
            var customPromptOptions = registrationPrompt.Options.Select((option, index) => (index, option))
                .ToList<(int id, string name)>();
            var groupNamePrefix = registrationPrompt.PromptText;
            return (customPromptOptions, groupNamePrefix);
        }
    }
}
