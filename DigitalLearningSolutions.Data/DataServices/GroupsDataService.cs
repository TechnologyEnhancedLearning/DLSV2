namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public interface IGroupsDataService
    {
        IEnumerable<Group> GetGroupsForCentre(int centreId);

        IEnumerable<GroupDelegate> GetGroupDelegates(int groupId);

        IEnumerable<GroupCourse> GetGroupCoursesVisibleToCentre(int centreId);

        GroupCourse? GetGroupCourseIfVisibleToCentre(int groupCustomisationId, int centreId);

        string? GetGroupName(int groupId, int centreId);

        int? GetGroupCentreId(int groupId);

        int? GetRelatedProgressIdForGroupDelegate(int groupId, int delegateId);

        void DeleteGroupDelegatesRecordForDelegate(int groupId, int delegateId);

        int AddDelegateGroup(GroupDetails groupDetails);

        void AddDelegateToGroup(int delegateId, int groupId, DateTime addedDate, int addedByFieldLink);

        void RemoveRelatedProgressRecordsForGroupCourse(
            int groupId,
            int groupCustomisationId,
            bool deleteStartedEnrolment,
            DateTime removedDate
        );

        void RemoveRelatedProgressRecordsForGroup(
            int groupId,
            int? delegateId,
            bool removeStartedEnrolments,
            DateTime removedDate
        );

        void RemoveRelatedProgressRecordsForGroup(int groupId, bool deleteStartedEnrolment, DateTime removedDate);

        void DeleteGroupDelegates(int groupId);

        void DeleteGroupCustomisation(int groupCustomisationId);

        void DeleteGroupCustomisations(int groupId);

        void DeleteGroup(int groupId);

        Group? GetGroupAtCentreById(int groupId, int centreId);

        void UpdateGroupDescription(int groupId, int centreId, string? groupDescription);

        void UpdateGroupName(int groupId, int centreId, string groupName);

        int InsertGroupCustomisation(
            int groupId,
            int customisationId,
            int completeWithinMonths,
            int addedByAdminUserId,
            bool cohortLearners,
            int? supervisorAdminId
        );

        void AddDelegatesWithMatchingAnswersToGroup(
            int groupId,
            DateTime addedDate,
            int linkedToField,
            int centreId,
            string? option,
            int? jobGroupId
        );
    }

    public class GroupsDataService : IGroupsDataService
    {
        private const string CourseCountSql = @"SELECT COUNT(*)
                FROM GroupCustomisations AS gc
                JOIN Customisations AS c ON c.CustomisationID = gc.CustomisationID
                INNER JOIN dbo.CentreApplications AS ca ON ca.ApplicationID = c.ApplicationID
                INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = ca.ApplicationID
                WHERE gc.GroupID = g.GroupID
                AND ca.CentreId = @centreId
                AND gc.InactivatedDate IS NULL
                AND ap.ArchivedDate IS NULL
                AND c.Active = 1
                AND ap.DefaultContentTypeID <> 4";

        private const string GroupCourseSql = @"SELECT
                        GroupCustomisationID,
                        GroupID,
                        gc.CustomisationID,
                        ap.ApplicationName,
                        ap.CourseCategoryID,
                        CustomisationName,
                        Mandatory AS IsMandatory,
                        IsAssessed,
                        AddedDate AS AddedToGroup,
                        c.CurrentVersion,
                        gc.SupervisorAdminID,
                        au.Forename AS SupervisorFirstName,
                        au.Surname AS SupervisorLastName,
                        au.Active AS SupervisorAdminActive,
                        gc.CompleteWithinMonths,
                        ValidityMonths,
                        c.Active,
                        ap.ArchivedDate AS ApplicationArchivedDate,
                        gc.InactivatedDate
                    FROM GroupCustomisations AS gc
                    JOIN Customisations AS c ON c.CustomisationID = gc.CustomisationID
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = c.ApplicationID
                    LEFT JOIN AdminUsers AS au ON au.AdminID = gc.SupervisorAdminID
                    WHERE ap.DefaultContentTypeID <> 4
                        AND (c.CentreID = @centreId OR c.AllCentres = 1)
                        AND EXISTS (
                            SELECT CentreApplicationID
                            FROM CentreApplications
                            WHERE (ApplicationID = c.ApplicationID)
                                AND (CentreID = @centreID) AND (Active = 1))";

        private const string SelectIdsOfGroupProgressRecordsSuitableForRemoval =
            @"SELECT ProgressID
            FROM Progress AS P
            INNER JOIN GroupCustomisations AS GC ON P.CustomisationID = GC.CustomisationID
            INNER JOIN GroupDelegates AS GD ON GD.DelegateID = P.CandidateID AND GD.GroupID = GC.GroupID
            WHERE P.Completed IS NULL
            AND P.EnrollmentMethodID = 3
            AND GC.GroupID = @groupId
            AND P.RemovedDate IS NULL
            AND (P.LoginCount = 0 OR @deleteStartedEnrolment = 1)
            AND NOT EXISTS (SELECT * FROM GroupCustomisations AS GCInner
                            INNER JOIN GroupDelegates AS GDInner ON GCInner.GroupID = GDInner.GroupID
                            WHERE GCInner.CustomisationID = P.CustomisationID
                            AND GDInner.DelegateID = P.CandidateID
                            AND GCInner.GroupID != GC.GroupID)";

        private readonly IDbConnection connection;

        private readonly string groupsSql = @$"SELECT
                        GroupID,
                        GroupLabel,
                        GroupDescription,
                        (SELECT COUNT(*) FROM GroupDelegates AS gd WHERE gd.GroupID = g.GroupID) AS DelegateCount,
                        ({CourseCountSql}) AS CoursesCount,
                        g.CreatedByAdminUserID AS AddedByAdminId,
                        au.Forename AS AddedByFirstName,
                        au.Surname AS AddedByLastName,
                        au.Active AS AddedByAdminActive,
                        LinkedToField,
                        CASE
                            WHEN LinkedToField = 0 THEN 'None'
                            WHEN LinkedToField = 1 THEN (SELECT cp.CustomPrompt FROM CustomPrompts AS cp WHERE cp.CustomPromptID = c.CustomField1PromptID)
                            WHEN LinkedToField = 2 THEN (SELECT cp.CustomPrompt FROM CustomPrompts AS cp WHERE cp.CustomPromptID = c.CustomField2PromptID)
                            WHEN LinkedToField = 3 THEN (SELECT cp.CustomPrompt FROM CustomPrompts AS cp WHERE cp.CustomPromptID = c.CustomField3PromptID)
                            WHEN LinkedToField = 4 THEN 'Job group'
                            WHEN LinkedToField = 5 THEN (SELECT cp.CustomPrompt FROM CustomPrompts AS cp WHERE cp.CustomPromptID = c.CustomField4PromptID)
                            WHEN LinkedToField = 6 THEN (SELECT cp.CustomPrompt FROM CustomPrompts AS cp WHERE cp.CustomPromptID = c.CustomField5PromptID)
                            WHEN LinkedToField = 7 THEN (SELECT cp.CustomPrompt FROM CustomPrompts AS cp WHERE cp.CustomPromptID = c.CustomField6PromptID)
                        END AS LinkedToFieldName,
                        AddNewRegistrants,
                        SyncFieldChanges
                        FROM Groups AS g
                    JOIN AdminUsers AS au ON au.AdminID = g.CreatedByAdminUserID
                    JOIN Centres AS c ON c.CentreID = g.CentreID
                    WHERE RemovedDate IS NULL";

        public GroupsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Group> GetGroupsForCentre(int centreId)
        {
            return connection.Query<Group>(
                @$"{groupsSql} AND g.CentreID = @centreId",
                new { centreId }
            );
        }

        public IEnumerable<GroupDelegate> GetGroupDelegates(int groupId)
        {
            return connection.Query<GroupDelegate>(
                @"SELECT
                        gd.GroupDelegateID,
                        gd.GroupID,
                        gd.DelegateID,
                        gd.AddedDate,
                        da.CandidateNumber,
                        u.FirstName,
                        u.LastName,
                        u.HasBeenPromptedForPrn,
                        u.ProfessionalRegistrationNumber,
                        u.PrimaryEmail,
                        ucd.Email AS CentreEmail,
                        ucd.EmailVerified AS CentreEmailVerified
                    FROM GroupDelegates AS gd
                    JOIN DelegateAccounts AS da ON da.ID = gd.DelegateID
                    JOIN Users AS u ON u.ID = da.UserID
                    LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = u.ID AND ucd.CentreId = da.CentreID
                    WHERE gd.GroupID = @groupId",
                new { groupId }
            );
        }

        public IEnumerable<GroupCourse> GetGroupCoursesVisibleToCentre(int centreId)
        {
            return connection.Query<GroupCourse>(
                @$"{GroupCourseSql}",
                new { centreId }
            );
        }

        public GroupCourse? GetGroupCourseIfVisibleToCentre(int groupCustomisationId, int centreId)
        {
            return connection.Query<GroupCourse>(
                @$"{GroupCourseSql} AND gc.GroupCustomisationID = @groupCustomisationId",
                new { groupCustomisationId, centreId }
            ).FirstOrDefault();
        }

        public string? GetGroupName(int groupId, int centreId)
        {
            return connection.Query<string>(
                @"SELECT
                        GroupLabel
                    FROM Groups
                    WHERE GroupID = @groupId AND CentreId = @centreId",
                new { groupId, centreId }
            ).SingleOrDefault();
        }

        public int? GetGroupCentreId(int groupId)
        {
            return connection.Query<int>(
                @"SELECT CentreID
                        FROM Groups
                        WHERE GroupID = @groupId",
                new { groupId }
            ).SingleOrDefault();
        }

        public int? GetRelatedProgressIdForGroupDelegate(int groupId, int delegateId)
        {
            return connection.Query<int?>(
                @"SELECT ProgressID
                    FROM Progress AS P
                    INNER JOIN GroupCustomisations AS GC ON P.CustomisationID = GC.CustomisationID
                    WHERE p.Completed IS NULL
                        AND p.EnrollmentMethodID = 3
                        AND GC.GroupID = @groupId
                        AND p.CandidateID = @delegateId
                        AND P.RemovedDate IS NULL",
                new { groupId, delegateId }
            ).FirstOrDefault();
        }

        public void DeleteGroupDelegatesRecordForDelegate(int groupId, int delegateId)
        {
            connection.Execute(
                @"DELETE FROM GroupDelegates
                    WHERE GroupID = @groupId
                      AND DelegateID = @delegateId",
                new { groupId, delegateId }
            );
        }

        public int AddDelegateGroup(GroupDetails groupDetails)
        {
            return connection.QuerySingle<int>(
                @"INSERT INTO Groups (CentreID, GroupLabel, GroupDescription, LinkedToField, SyncFieldChanges, AddNewRegistrants, PopulateExisting, CreatedDate, CreatedByAdminUserID)
                        OUTPUT inserted.GroupID
                        VALUES (@CentreId, @GroupLabel, @GroupDescription, @LinkedToField, @SyncFieldChanges, @AddNewRegistrants, @PopulateExisting, @CreatedDate, @AdminUserId)",
                groupDetails
            );
        }

        public void AddDelegateToGroup(int delegateId, int groupId, DateTime addedDate, int addedByFieldLink)
        {
            connection.Execute(
                @"INSERT INTO GroupDelegates (GroupID, DelegateID, AddedDate, AddedByFieldLink)
                    VALUES (
                        @groupId,
                        @delegateId,
                        @addedDate,
                        @addedByFieldLink)",
                new { groupId, delegateId, addedDate, addedByFieldLink }
            );
        }

        public void DeleteGroupDelegates(int groupId)
        {
            connection.Execute(
                @"DELETE FROM GroupDelegates
                     WHERE GroupID = @groupId",
                new { groupId }
            );
        }

        public void DeleteGroupCustomisation(int groupCustomisationId)
        {
            connection.Execute(
                @"DELETE FROM GroupCustomisations
                     WHERE GroupCustomisationID = @groupCustomisationId",
                new { groupCustomisationId }
            );
        }

        public void DeleteGroupCustomisations(int groupId)
        {
            connection.Execute(
                @"DELETE FROM GroupCustomisations
                     WHERE GroupID = @groupId",
                new { groupId }
            );
        }

        public void DeleteGroup(int groupId)
        {
            connection.Execute(
                @"DELETE FROM Groups
                     WHERE GroupID = @groupId",
                new { groupId }
            );
        }

        public void RemoveRelatedProgressRecordsForGroup(int groupId, bool deleteStartedEnrolment, DateTime removedDate)
        {
            RemoveRelatedProgressRecordsForGroup(groupId, null, deleteStartedEnrolment, removedDate);
        }

        public void RemoveRelatedProgressRecordsForGroup(
            int groupId,
            int? delegateId,
            bool deleteStartedEnrolment,
            DateTime removedDate
        )
        {
            connection.Execute(
                $@"UPDATE Progress
                        SET
                            RemovedDate = @removedDate,
                            RemovalMethodID = 3
                        WHERE ProgressID IN
                            ({SelectIdsOfGroupProgressRecordsSuitableForRemoval}
                             AND (P.CandidateID = @delegateId OR @delegateId IS NULL))",
                new { groupId, removedDate, deleteStartedEnrolment, delegateId }
            );
        }

        public void RemoveRelatedProgressRecordsForGroupCourse(
            int groupId,
            int groupCustomisationId,
            bool deleteStartedEnrolment,
            DateTime timeOfRemoval
        )
        {
            connection.Execute(
                $@"UPDATE Progress
                        SET
                            RemovedDate = @timeOfRemoval,
                            RemovalMethodID = 3
                        WHERE ProgressID IN
                            ({SelectIdsOfGroupProgressRecordsSuitableForRemoval}
                             AND GC.GroupCustomisationID = @groupCustomisationId)",
                new { groupId, timeOfRemoval, deleteStartedEnrolment, groupCustomisationId }
            );
        }

        public Group? GetGroupAtCentreById(int groupId, int centreId)
        {
            return connection.Query<Group>(
                @$"{groupsSql} AND g.CentreID = @centreId AND GroupID = @groupId",
                new { groupId, centreId }
            ).SingleOrDefault();
        }

        public void UpdateGroupDescription(int groupId, int centreId, string? groupDescription)
        {
            connection.Execute(
                @"UPDATE Groups
                    SET
                        GroupDescription = @groupDescription
                    WHERE GroupID = @groupId AND CentreId = @centreId",
                new { groupDescription, groupId, centreId }
            );
        }

        public void UpdateGroupName(int groupId, int centreId, string groupName)
        {
            connection.Execute(
                @"UPDATE Groups
                    SET
                        GroupLabel = @groupName
                    WHERE GroupID = @groupId AND CentreId = @centreId",
                new { groupName, groupId, centreId }
            );
        }

        public int InsertGroupCustomisation(
            int groupId,
            int customisationId,
            int completeWithinMonths,
            int addedByAdminUserId,
            bool cohortLearners,
            int? supervisorAdminId
        )
        {
            return connection.QuerySingle<int>(
                @"INSERT INTO GroupCustomisations
                        (GroupID, CustomisationID, CompleteWithinMonths, AddedByAdminUserID, CohortLearners, SupervisorAdminID)
                    OUTPUT Inserted.GroupCustomisationId
                    VALUES
                        (@groupId, @customisationId, @completeWithinMonths, @addedByAdminUserId, @cohortLearners, @supervisorAdminID)",
                new
                {
                    groupId, customisationId, completeWithinMonths,
                    addedByAdminUserId, cohortLearners, supervisorAdminId,
                }
            );
        }

        public void AddDelegatesWithMatchingAnswersToGroup(
            int groupId,
            DateTime addedDate,
            int linkedToField,
            int centreId,
            string? option,
            int? jobGroupId
        )
        {
            connection.Execute(
                @"INSERT INTO GroupDelegates (GroupID, DelegateID, AddedDate, AddedByFieldLink)
                        SELECT @groupId, CandidateID, @addedDate, 1
                        FROM Candidates
                        WHERE (CentreID = @centreID)
                          AND (Active = 1)
                          AND ((Answer1 = @option AND @linkedToField = 1)
                            OR (Answer2 = @option AND @linkedToField = 2)
                            OR (Answer3 = @option AND @linkedToField = 3)
                            OR (JobGroupID = @jobGroupId AND @linkedToField = 4)
                            OR (Answer4 = @option AND @linkedToField = 5)
                            OR (Answer5 = @option AND @linkedToField = 6)
                            OR (Answer6 = @option AND @linkedToField = 7))",
                new { groupId, addedDate, linkedToField, centreId, option, jobGroupId }
            );
        }
    }
}
