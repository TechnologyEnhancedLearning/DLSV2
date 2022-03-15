namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public static class GroupTestHelper
    {
        public static GroupDelegate GetDefaultGroupDelegate(
            int groupDelegateId = 62,
            int groupId = 5,
            int delegateId = 245969,
            string? firstName = "xxxxx",
            string lastName = "xxxx",
            string? emailAddress = "gslectik.m@vao",
            string candidateNumber = "KT553",
            bool hasBeenPromptedForPrn = false,
            string? professionalRegistrationNumber = null
        )
        {
            var addedDate = new DateTime(2018, 11, 02, 10, 49, 35, 600);

            return new GroupDelegate
            {
                GroupDelegateId = groupDelegateId,
                GroupId = groupId,
                DelegateId = delegateId,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                CandidateNumber = candidateNumber,
                AddedDate = addedDate,
                HasBeenPromptedForPrn = hasBeenPromptedForPrn,
                ProfessionalRegistrationNumber = professionalRegistrationNumber,
            };
        }

        public static GroupCourse GetDefaultGroupCourse(
            int groupCustomisationId = 1,
            int groupId = 8,
            int customisationId = 25918,
            int courseCategoryId = 1,
            string applicationName = "Practice Nurse Clinical Supervision",
            string? customisationName = "Demo",
            bool isMandatory = false,
            bool isAssessed = false,
            DateTime? addedToGroup = null,
            int currentVersion = 2,
            int? supervisorAdminId = null,
            string? supervisorFirstName = null,
            string? supervisorLastName = null,
            bool supervisorActive = true,
            int completeWithinMonths = 12,
            int validityMonths = 0,
            bool active = true,
            DateTime? applicationArchivedDate = null,
            DateTime? inactivatedDate = null
        )
        {
            return new GroupCourse
            {
                GroupCustomisationId = groupCustomisationId,
                GroupId = groupId,
                CustomisationId = customisationId,
                CourseCategoryId = courseCategoryId,
                ApplicationName = applicationName,
                CustomisationName = customisationName,
                IsMandatory = isMandatory,
                IsAssessed = isAssessed,
                AddedToGroup = addedToGroup ?? DateTime.Now,
                CurrentVersion = currentVersion,
                SupervisorAdminId = supervisorAdminId,
                SupervisorFirstName = supervisorFirstName,
                SupervisorLastName = supervisorLastName,
                SupervisorAdminActive = supervisorActive,
                CompleteWithinMonths = completeWithinMonths,
                ValidityMonths = validityMonths,
                Active = active,
                ApplicationArchivedDate = applicationArchivedDate,
                InactivatedDate = inactivatedDate,
            };
        }

        public static async Task<(int, DateTime?)> GetProgressRemovedFields(
            this DbConnection connection,
            int progressId
        )
        {
            var progress = await connection.QueryAsync<(int, DateTime?)>(
                @"SELECT
                        RemovalMethodID,
                        RemovedDate
                    FROM Progress
                    WHERE ProgressID = @progressId",
                new { progressId }
            );

            return progress.Single();
        }

        public static Group GetDefaultGroup(
            int groupId = 34,
            string groupLabel = "Social care - unspecified",
            string? groupDescription = null,
            int delegateCount = 1,
            int coursesCount = 0,
            int addedByAdminId = 1,
            string addedByFirstName = "Kevin",
            string addedByLastName = "Whittaker (Developer)",
            bool addedByAdminActive = true,
            int linkedToField = 4,
            string linkedToFieldName = "Job group",
            bool shouldAddNewRegistrantsToGroup = true,
            bool changesToRegistrationDetailsShouldChangeGroupMembership = true
        )
        {
            return new Group
            {
                GroupId = groupId,
                GroupLabel = groupLabel,
                GroupDescription = groupDescription,
                DelegateCount = delegateCount,
                CoursesCount = coursesCount,
                AddedByAdminId = addedByAdminId,
                AddedByFirstName = addedByFirstName,
                AddedByLastName = addedByLastName,
                AddedByAdminActive = addedByAdminActive,
                LinkedToField = linkedToField,
                LinkedToFieldName = linkedToFieldName,
                ShouldAddNewRegistrantsToGroup = shouldAddNewRegistrantsToGroup,
                ChangesToRegistrationDetailsShouldChangeGroupMembership =
                    changesToRegistrationDetailsShouldChangeGroupMembership,
            };
        }

        public static async Task<IEnumerable<int>> GetCandidatesForGroup(this DbConnection connection, int groupId)
        {
            return await connection.QueryAsync<int>(
                @"SELECT DelegateID
                    FROM GroupDelegates
                    WHERE GroupID = @groupId",
                new { groupId }
            );
        }

        public static async Task<IEnumerable<int>> GetGroupCustomisationIdsForGroup(
            this DbConnection connection,
            int groupId
        )
        {
            return await connection.QueryAsync<int>(
                @"SELECT GroupCustomisationID
                    FROM GroupCustomisations
                    WHERE GroupID = @groupId",
                new { groupId }
            );
        }
    }
}
