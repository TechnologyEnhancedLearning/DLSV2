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

        IEnumerable<GroupCourse> GetGroupCourses(int groupId, int centreId);

        string? GetGroupName(int groupId, int centreId);

        void RemoveRelatedProgressRecordsForGroup(
            int groupId,
            int? delegateId,
            bool removeStartedEnrolments,
            DateTime removedDate
        );

        int? GetGroupCentreId(int groupId);

        int? GetRelatedProgressIdForGroupDelegate(int groupId, int delegateId);

        void DeleteGroupDelegatesRecordForDelegate(int groupId, int delegateId);

        int AddDelegateGroup(GroupDetails groupDetails);

        void AddDelegateToGroup(int delegateId, int groupId, DateTime addedDate, int addedByFieldLink);

        void RemoveRelatedProgressRecordsForGroup(int groupId, bool deleteStartedEnrolment, DateTime removedDate);

        void DeleteGroupDelegates(int groupId);

        void DeleteGroupCustomisations(int groupId);

        void DeleteGroup(int groupId);

        Group? GetGroupAtCentreById(int groupId, int centreId);

        void UpdateGroupDescription(int groupId, int centreId, string? groupDescription);

        void UpdateGroupName(int groupId, int centreId, string groupName);
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
                AND c.Active = 1";

        private readonly IDbConnection connection;

        public GroupsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Group> GetGroupsForCentre(int centreId)
        {
            return connection.Query<Group>(
                @$"SELECT
	                    GroupID,
	                    GroupLabel,
	                    GroupDescription,
	                    (SELECT COUNT(*) FROM GroupDelegates AS gd WHERE gd.GroupID = g.GroupID) AS DelegateCount,
	                    ({CourseCountSql}) AS CoursesCount,
                        g.CreatedByAdminUserID As AddedByAdminId,
	                    au.Forename AS AddedByFirstName,
	                    au.Surname AS AddedByLastName,
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
                    WHERE RemovedDate IS NULL AND g.CentreID = @centreId",
                new { centreId }
            );
        }

        public IEnumerable<GroupDelegate> GetGroupDelegates(int groupId)
        {
            return connection.Query<GroupDelegate>(
                @"SELECT
                        GroupDelegateID,
                        GroupID,
                        DelegateID,
                        FirstName,
                        LastName,
                        EmailAddress,
                        CandidateNumber
                    FROM GroupDelegates AS gd
                    JOIN Candidates AS c ON c.CandidateID = gd.DelegateID
                    WHERE gd.GroupID = @groupId",
                new { groupId }
            );
        }

        public IEnumerable<GroupCourse> GetGroupCourses(int groupId, int centreId)
        {
            return connection.Query<GroupCourse>(
                @"SELECT
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
                        gc.CompleteWithinMonths,
                        ValidityMonths
                    FROM GroupCustomisations AS gc
                    JOIN Customisations AS c ON c.CustomisationID = gc.CustomisationID
                    INNER JOIN dbo.CentreApplications AS ca ON ca.ApplicationID = c.ApplicationID
                    INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = ca.ApplicationID
                    LEFT JOIN AdminUsers AS au ON au.AdminID = gc.SupervisorAdminID
                    WHERE gc.GroupID = @groupId
                        AND ca.CentreId = @centreId
                        AND gc.InactivatedDate IS NULL
                        AND ap.ArchivedDate IS NULL
                        AND c.Active = 1
                        AND ap.DefaultContentTypeID <> 4",
                new { groupId, centreId }
            );
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

        public void RemoveRelatedProgressRecordsForGroupDelegate(int groupId, int delegateId, DateTime removedDate)
        {
            RemoveRelatedProgressRecordsForGroup(groupId, delegateId, false, removedDate);
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

        public void RemoveRelatedProgressRecordsForGroup(int groupId, bool deleteStartedEnrolment, DateTime removedDate)
        {
            RemoveRelatedProgressRecordsForGroup(groupId, null, deleteStartedEnrolment, removedDate);
        }

        public void DeleteGroupDelegates(int groupId)
        {
            connection.Execute(
                @"DELETE FROM GroupDelegates
                     WHERE GroupID = @groupId",
                new { groupId }
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

        public void RemoveRelatedProgressRecordsForGroup(
            int groupId,
            int? delegateId,
            bool deleteStartedEnrolment,
            DateTime removedDate
        )
        {
            connection.Execute(
                @"UPDATE Progress
                        SET
                            RemovedDate = @removedDate,
                            RemovalMethodID = 3
                        WHERE ProgressID IN
                            (SELECT ProgressID
                             FROM Progress AS P
                             INNER JOIN GroupCustomisations AS GC ON P.CustomisationID = GC.CustomisationID
                             WHERE P.Completed IS NULL
                             AND P.EnrollmentMethodID = 3
                             AND GC.GroupID = @groupId
                             AND (P.CandidateID = @delegateId OR @delegateId IS NULL)
                             AND P.RemovedDate IS NULL
                             AND (P.LoginCount = 0 OR @deleteStartedEnrolment = 1)
                             AND NOT EXISTS (SELECT * FROM GroupCustomisations AS GCInner
                                                INNER JOIN GroupDelegates AS GD ON GCInner.GroupID = GD.GroupID
                                                WHERE GCInner.CustomisationID = P.CustomisationID
                                                AND GD.DelegateID = P.CandidateID
                                                AND GCInner.GroupID != GC.GroupID))",
                new { groupId, removedDate, deleteStartedEnrolment, delegateId }
            );
        }

        public Group? GetGroupAtCentreById(int groupId, int centreId)
        {
            return connection.Query<Group>(
                @$"SELECT
	                    GroupID,
	                    GroupLabel,
	                    GroupDescription,
	                    (SELECT COUNT(*) FROM GroupDelegates AS gd WHERE gd.GroupID = g.GroupID) AS DelegateCount,
	                    ({CourseCountSql}) AS CoursesCount,
                        g.CreatedByAdminUserID As AddedByAdminId,
	                    au.Forename AS AddedByFirstName,
	                    au.Surname AS AddedByLastName,
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
                    WHERE RemovedDate IS NULL AND g.CentreID = @centreId AND GroupID = @groupId",
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
    }
}
