namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;

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
            int delegateId,
            int? addedByAdminId = null
        );

        void SynchroniseUserChangesWithSynchronisedGroups(
            int delegateId,
            AccountDetailsData accountDetailsData,
            RegistrationFieldAnswers registrationFieldAnswers,
            RegistrationFieldAnswers oldRegistrationFieldAnswers,
            string? centreEmail
        );

        void AddNewlyRegisteredUserToAddNewRegistrantGroups(
            int delegateId,
            DelegateRegistrationModel delegateRegistrationModel
        );

        void SynchroniseUserChangesWithGroups(
            int delegateId,
            AccountDetailsData accountDetailsData,
            RegistrationFieldAnswers newDelegateDetails,
            RegistrationFieldAnswers oldRegistrationFieldAnswers,
            string? centreEmail,
            List<Group> groupsToSynchronise
        );

        void EnrolDelegateOnGroupCourses(
            int delegateId,
            int centreId,
            AccountDetailsData newDetails,
            string? centreEmail,
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
        private readonly IClockUtility clockUtility;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly IGroupsDataService groupsDataService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly ILogger<IGroupsService> logger;
        private readonly IProgressDataService progressDataService;
        private readonly ITutorialContentDataService tutorialContentDataService;
        private readonly IUserDataService userDataService;

        public GroupsService(
            IGroupsDataService groupsDataService,
            IClockUtility clockUtility,
            ITutorialContentDataService tutorialContentDataService,
            IEmailService emailService,
            IJobGroupsDataService jobGroupsDataService,
            IProgressDataService progressDataService,
            IConfiguration configuration,
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            ILogger<IGroupsService> logger,
            IUserDataService userDataService
        )
        {
            this.groupsDataService = groupsDataService;
            this.clockUtility = clockUtility;
            this.tutorialContentDataService = tutorialContentDataService;
            this.emailService = emailService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.progressDataService = progressDataService;
            this.configuration = configuration;
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.logger = logger;
            this.userDataService = userDataService;
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
                CreatedDate = clockUtility.UtcNow,
                LinkedToField = linkedToField,
                SyncFieldChanges = syncFieldChanges,
                AddNewRegistrants = addNewRegistrants,
                PopulateExisting = populateExisting,
            };

            return groupsDataService.AddDelegateGroup(groupDetails);
        }

        public void SynchroniseUserChangesWithSynchronisedGroups(
            int delegateId,
            AccountDetailsData accountDetailsData,
            RegistrationFieldAnswers registrationFieldAnswers,
            RegistrationFieldAnswers oldRegistrationFieldAnswers,
            string? centreEmail
        )
        {
            var synchronisedGroupsForCentre =
                GetSynchronisedGroupsForCentre(registrationFieldAnswers.CentreId).ToList();

            SynchroniseUserChangesWithGroups(
                delegateId,
                accountDetailsData,
                registrationFieldAnswers,
                oldRegistrationFieldAnswers,
                centreEmail,
                synchronisedGroupsForCentre
            );
        }

        public void AddNewlyRegisteredUserToAddNewRegistrantGroups(
            int delegateId,
            DelegateRegistrationModel delegateRegistrationModel
        )
        {
            var addNewRegistrantGroups =
                GetAddNewRegistrantGroupsForCentre(delegateRegistrationModel.Centre).ToList();

            SynchroniseUserChangesWithGroups(
                delegateId,
                new AccountDetailsData(
                    delegateRegistrationModel.FirstName,
                    delegateRegistrationModel.LastName,
                    delegateRegistrationModel.PrimaryEmail
                ),
                delegateRegistrationModel.GetRegistrationFieldAnswers(),
                new RegistrationFieldAnswers(
                    delegateRegistrationModel.Centre,
                    0,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                ),
                delegateRegistrationModel.CentreSpecificEmail,
                addNewRegistrantGroups
            );
        }

        public void SynchroniseUserChangesWithGroups(
            int delegateId,
            AccountDetailsData accountDetailsData,
            RegistrationFieldAnswers registrationFieldAnswers,
            RegistrationFieldAnswers oldRegistrationFieldAnswers,
            string? centreEmail,
            List<Group> groupsToSynchronise
        )
        {
            using var transaction = new TransactionScope();
            var changedLinkedFields = LinkedFieldHelper.GetLinkedFieldChanges(
                oldRegistrationFieldAnswers,
                registrationFieldAnswers,
                jobGroupsDataService,
                centreRegistrationPromptsService
            );

            foreach (var changedAnswer in changedLinkedFields)
            {
                var groupsToRemoveDelegateFrom = groupsToSynchronise.Where(
                    g => g.LinkedToField == changedAnswer.LinkedFieldNumber &&
                         GroupLabelMatchesAnswer(g.GroupLabel, changedAnswer.OldValue, changedAnswer.LinkedFieldName)
                );

                var groupsToAddDelegateTo = groupsToSynchronise.Where(
                    g => g.LinkedToField == changedAnswer.LinkedFieldNumber &&
                         GroupLabelMatchesAnswer(g.GroupLabel, changedAnswer.NewValue, changedAnswer.LinkedFieldName)
                );

                foreach (var groupToRemoveDelegateFrom in groupsToRemoveDelegateFrom)
                {
                    RemoveDelegateFromGroup(delegateId, groupToRemoveDelegateFrom.GroupId);
                }

                foreach (var groupToAddDelegateTo in groupsToAddDelegateTo)
                {
                    groupsDataService.AddDelegateToGroup(
                        delegateId,
                        groupToAddDelegateTo.GroupId,
                        clockUtility.UtcNow,
                        1
                    );

                    EnrolDelegateOnGroupCourses(
                        delegateId,
                        registrationFieldAnswers.CentreId,
                        accountDetailsData,
                        centreEmail,
                        groupToAddDelegateTo.GroupId
                    );
                }
            }

            transaction.Complete();
        }

        public void EnrolDelegateOnGroupCourses(
            int delegateId,
            int centreId,
            AccountDetailsData newDetails,
            string? centreEmail,
            int groupId,
            int? addedByAdminId = null
        )
        {
            var groupCourses = GetUsableGroupCoursesForCentre(groupId, centreId);
            var fullName = newDetails.FirstName + " " + newDetails.Surname;

            foreach (var groupCourse in groupCourses)
            {
                EnrolDelegateOnGroupCourse(
                    delegateId,
                    centreEmail ?? newDetails.Email,
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
                clockUtility.UtcNow
            );
            groupsDataService.DeleteGroupDelegates(groupId);
            groupsDataService.DeleteGroupCustomisations(groupId);
            groupsDataService.DeleteGroup(groupId);

            transaction.Complete();
        }

        public void AddDelegateToGroupAndEnrolOnGroupCourses(
            int groupId,
            int delegateId,
            int? addedByAdminId = null
        )
        {
            var delegateUser = userDataService.GetDelegateUserById(delegateId)!;
            var delegateEntity = userDataService.GetDelegateById(delegateId)!;

            using var transaction = new TransactionScope();

            groupsDataService.AddDelegateToGroup(delegateId, groupId, clockUtility.UtcNow, 0);

            var accountDetailsData = new EditAccountDetailsData(
                delegateId,
                delegateUser.FirstName!,
                delegateUser.LastName,
                delegateUser.EmailAddress!,
                delegateUser.JobGroupId,
                delegateUser.ProfessionalRegistrationNumber,
                delegateUser.HasBeenPromptedForPrn,
                delegateUser.ProfileImage
            );

            EnrolDelegateOnGroupCourses(
                delegateUser.Id,
                delegateUser.CentreId,
                accountDetailsData,
                delegateEntity.EmailForCentreNotifications,
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

            var currentDate = clockUtility.UtcNow;
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
                clockUtility.UtcNow
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
                    groupDelegate.EmailForCentreNotifications,
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

            var (newGroupNames, groupNamePrefix) = isJobGroup
                ? GetJobGroupsAndPrefix()
                : GetCentreRegistrationPromptsAndPrefix(groupDetails.CentreId, groupDetails.RegistrationField.Id);

            var groupsAtCentre = GetGroupsForCentre(groupDetails.CentreId).Select(g => g.GroupLabel).ToList();

            using var transaction = new TransactionScope();
            foreach (var (id, newGroupName) in newGroupNames)
            {
                var groupName = groupDetails.PrefixGroupName
                    ? GetGroupNameWithPrefix(groupNamePrefix, newGroupName)
                    : newGroupName;

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
                        clockUtility.UtcNow,
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
            string delegateUserEmailAddress,
            string delegateUserFullName,
            int? addedByAdminId,
            GroupCourse groupCourse,
            bool isAddCourseToGroup
        )
        {
            var completeByDate = groupCourse.CompleteWithinMonths != 0
                ? (DateTime?)clockUtility.UtcNow.AddMonths(groupCourse.CompleteWithinMonths)
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
                    clockUtility.UtcNow,
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

        private IEnumerable<Group> GetSynchronisedGroupsForCentre(int centreId)
        {
            return groupsDataService.GetGroupsForCentre(centreId)
                .Where(g => g.ChangesToRegistrationDetailsShouldChangeGroupMembership);
        }

        private IEnumerable<Group> GetAddNewRegistrantGroupsForCentre(int centreId)
        {
            return groupsDataService.GetGroupsForCentre(centreId)
                .Where(g => g.ShouldAddNewRegistrantsToGroup);
        }

        private static bool GroupLabelMatchesAnswer(string groupLabel, string? answer, string linkedFieldName)
        {
            return !string.IsNullOrEmpty(answer) && string.Equals(
                groupLabel,
                answer,
                StringComparison.CurrentCultureIgnoreCase
            ) || string.Equals(
                groupLabel,
                GetGroupNameWithPrefix(linkedFieldName, answer!),
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
                clockUtility.UtcNow
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
            var baseUrl = ConfigurationExtensions.GetAppRootPath(configuration);
            var linkToLearningPortal = baseUrl + "/LearningPortal/Current";
            var linkToCourse = baseUrl + "/LearningMenu/" + course.CustomisationId;
            var emailBodyText = $@"
                Dear {fullName}
                This is an automated message to notify you that you have been enrolled on the course
                {course.CourseName}
                by the system because a previous course completion has expired.
                To login to the course directly click here:{linkToCourse}.
                To login to the Learning Portal to access and complete your course click here:
                {linkToLearningPortal}.";
            var emailBodyHtml = $@"
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

        private (List<(int id, string name)>, string groupNamePrefix) GetCentreRegistrationPromptsAndPrefix(
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

        private static string GetGroupNameWithPrefix(string prefix, string groupName)
        {
            return $"{prefix} - {groupName}";
        }
    }
}
