﻿namespace DigitalLearningSolutions.Data.DataServices
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

        void RemoveRelatedProgressRecordsForGroupDelegate(int groupId, int delegateId, DateTime removedDate);

        int? GetRelatedProgressIdForGroupDelegate(int groupId, int delegateId);

        void DeleteGroupDelegatesRecordForDelegate(int groupId, int delegateId);

        Group? GetGroupAtCentreById(int groupId, int centreId);

        bool TryUpdateGroupDescription(int groupId, int centreId, string groupDescription);

        int AddDelegateGroup(GroupDetails groupDetails);

        void AddDelegateToGroup(int delegateId, int groupId, DateTime addedDate, int addedByFieldLink);
    }

    public class GroupsDataService : IGroupsDataService
    {
        private readonly IDbConnection connection;

        public GroupsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Group> GetGroupsForCentre(int centreId)
        {
            const string courseCountSql = @"SELECT COUNT(*)
                FROM GroupCustomisations AS gc
                JOIN Customisations AS c ON c.CustomisationID = gc.CustomisationID
                INNER JOIN dbo.CentreApplications AS ca ON ca.ApplicationID = c.ApplicationID
                INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = ca.ApplicationID
                WHERE gc.GroupID = g.GroupID
                AND ca.CentreId = @centreId
                AND gc.InactivatedDate IS NULL
                AND ap.ArchivedDate IS NULL
                AND c.Active = 1";

            return connection.Query<Group>(
                @$"SELECT
	                    GroupID,
	                    GroupLabel,
	                    GroupDescription,
	                    (SELECT COUNT(*) FROM GroupDelegates AS gd WHERE gd.GroupID = g.GroupID) AS DelegateCount,
	                    ({courseCountSql}) AS CoursesCount,
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
                        AND c.Active = 1",
                new { groupId, centreId }
            );
        }

        public void RemoveRelatedProgressRecordsForGroupDelegate(int groupId, int delegateId, DateTime removedDate)
        {
            const string numberOfGroupsWhereDelegateIsEnrolledOnThisCourse =
                @"SELECT COUNT(DISTINCT(gd.GroupId))
                    FROM dbo.Progress AS pr
                    INNER JOIN dbo.GroupCustomisations AS gc ON gc.CustomisationID = pr.CustomisationID
                    INNER JOIN dbo.GroupDelegates AS gd ON gd.DelegateID = pr.CandidateID AND gd.GroupID = gc.GroupID
                    WHERE pr.CustomisationID = Progress.CustomisationID AND pr.CandidateID = Progress.CandidateID";

            connection.Execute(
                $@"UPDATE Progress
                        SET
                            RemovedDate = @removedDate,
                            RemovalMethodID = 3
                        WHERE ProgressID IN
                              (SELECT ProgressID
                                FROM Progress AS P
                                INNER JOIN GroupCustomisations AS GC ON P.CustomisationID = GC.CustomisationID
                                WHERE p.Completed IS NULL
                                    AND p.EnrollmentMethodID  = 3
                                    AND GC.GroupID = @groupId
                                    AND p.CandidateID = @delegateId
                                    AND P.RemovedDate IS NULL
                                    AND p.LoginCount = 0)
                            AND ({numberOfGroupsWhereDelegateIsEnrolledOnThisCourse}) = 1",
                new { groupId, delegateId, removedDate }
            );
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

        public Group? GetGroupAtCentreById(int groupId, int centreId)
        {
            const string courseCountSql = @"SELECT COUNT(*)
                FROM GroupCustomisations AS gc
                JOIN Customisations AS c ON c.CustomisationID = gc.CustomisationID
                INNER JOIN dbo.CentreApplications AS ca ON ca.ApplicationID = c.ApplicationID
                INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = ca.ApplicationID
                WHERE gc.GroupID = g.GroupID
                AND ca.CentreId = @centreId
                AND gc.InactivatedDate IS NULL
                AND ap.ArchivedDate IS NULL
                AND c.Active = 1";

            return connection.Query<Group>(
                @$"SELECT
	                    GroupID,
	                    GroupLabel,
	                    GroupDescription,
	                    (SELECT COUNT(*) FROM GroupDelegates AS gd WHERE gd.GroupID = g.GroupID) AS DelegateCount,
	                    ({courseCountSql}) AS CoursesCount,
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

        public bool TryUpdateGroupDescription(int groupId, int centreId, string groupDescription)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Groups
                    SET
                        GroupDescription = @groupDescription
                    WHERE GroupID = @groupId AND CentreId = @centreId",
                new { groupDescription, groupId, centreId }
            );
            return numberOfAffectedRows > 0;
        }
    }
}
