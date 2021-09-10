namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public interface IGroupsDataService
    {
        IEnumerable<Group> GetGroupsForCentre(int centreId);

        IEnumerable<GroupDelegate> GetGroupDelegates(int groupId);

        IEnumerable<GroupCourse> GetGroupCourses(int groupId, int centreId);

        string? GetGroupName(int groupId, int centreId);

        void RemoveRelatedProgressRecordsForGroupDelegate(int groupId, int delegateId, DateTime removedDate);

        int? GetRelatedProgressIdForGroupDelegate(int groupId, int delegateId);

        void DeleteGroupDelegatesRecordForDelegate(int groupId, int delegateId);

        IEnumerable<Progress> GetDelegateProgressForCourse(int delegateId, int customisationId);

        void UpdateProgressSupervisorAndCompleteByDate(int progressId, int supervisorAdminId, DateTime? completeByDate);

        void AddDelegateToGroup(int delegateId, int groupId, DateTime addedDate, int addedByFieldLink);

        int CreateNewDelegateProgress(
            int delegateId,
            int customisationId,
            int customisationVersion,
            DateTime submittedTime,
            int enrollmentMethodId,
            int? enrolledByAdminId,
            DateTime? completeByDate,
            int supervisorAdminId
        );

        void CreateNewAspProgress(int tutorialId, int progressId);
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

        public void RemoveRelatedProgressRecordsForGroupDelegate(int groupId, int delegateId, DateTime removedDate)
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
                            WHERE p.Completed IS NULL
                                AND p.EnrollmentMethodID  = 3
                                AND GC.GroupID = @groupId
                                AND p.CandidateID = @delegateId
                                AND P.RemovedDate IS NULL
                                AND p.LoginCount = 0)",
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

        public IEnumerable<Progress> GetDelegateProgressForCourse(
            int delegateId,
            int customisationId
        )
        {
            return connection.Query<Progress>(
                @"SELECT ProgressId, 
	                    CandidateID, 
	                    CustomisationID, 
	                    Completed, 
	                    RemovedDate, 
	                    SystemRefreshed, 
	                    SupervisorAdminID,
                        CompleteByDate,
                        EnrollmentMethodID,
                        EnrolledByAdminID,
                        SubmittedTime,
                        CustomisationVersion
                    FROM Progress 
                    WHERE CandidateID = @delegateId
                        AND CustomisationID = @customisationId",
                new { delegateId, customisationId }
            );
        }

        public void UpdateProgressSupervisorAndCompleteByDate(
            int progressId,
            int supervisorAdminId,
            DateTime? completeByDate
        )
        {
            connection.Execute(
                @"UPDATE Progress SET
                        SupervisorAdminID = @supervisorAdminId,
                        CompleteByDate = @completeByDate
                    WHERE ProgressID = @progressId",
                new { progressId, supervisorAdminId, completeByDate }
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

        public int CreateNewDelegateProgress(
            int delegateId,
            int customisationId,
            int customisationVersion,
            DateTime submittedTime,
            int enrollmentMethodId,
            int? enrolledByAdminId,
            DateTime? completeByDate,
            int supervisorAdminId
        )
        {
            var progressId = connection.QuerySingle<int>(
                @"INSERT INTO Progress(
                        CandidateID,
                        CustomisationID,
                        CustomisationVersion,
                        SubmittedTime,
                        EnrollmentMethodID,
                        EnrolledByAdminID,
                        CompleteByDate,
                        SupervisorAdminID)
                    OUTPUT Inserted.ProgressID
                    VALUES (
                        @delegateId,
                        @customisationId,
                        @customisationVersion,
                        @submittedTime,
                        @enrollmentMethodId,
                        @enrolledByAdminId,
                        @completeByDate,
                        @supervisorAdminId)",
                new
                {
                    delegateId,
                    customisationId,
                    customisationVersion,
                    submittedTime,
                    enrollmentMethodId,
                    enrolledByAdminId,
                    completeByDate,
                    supervisorAdminId
                }
            );

            return progressId;
        }

        public void CreateNewAspProgress(int tutorialId, int progressId)
        {
            connection.Execute(
                @"INSERT INTO aspProgress (TutorialId, ProgressId)
                    VALUES (@tutorialId, @progressId)",
                new { tutorialId, progressId }
            );
        }
    }
}
