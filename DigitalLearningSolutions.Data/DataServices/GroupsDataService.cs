namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public interface IGroupsDataService
    {
        IEnumerable<Group> GetGroupsForCentre(int centreId);

        IEnumerable<GroupDelegate> GetGroupDelegates(int groupId);

        string GetGroupNameForGroupId(int groupId);
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
            return connection.Query<Group>(
                @"SELECT 
	                    GroupID,
	                    GroupLabel,
	                    GroupDescription,
	                    (SELECT COUNT(*) FROM GroupDelegates AS gd WHERE gd.GroupID = g.GroupID) AS DelegateCount,
	                    (SELECT COUNT(*) FROM GroupCustomisations AS gc WHERE gc.GroupID = g.GroupID AND InactivatedDate IS NULL) AS CoursesCount,
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
                        GroupDelegateID
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

        public string GetGroupNameForGroupId(int groupId)
        {
            return connection.Query<string>(
                @"SELECT 
	                    GroupLabel
                    FROM Groups
                    WHERE GroupID = @groupId",
                new { groupId }
            ).Single();
        }
    }
}
