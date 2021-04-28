namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;

    public interface IJobGroupsDataService
    {
        string? GetJobGroupName(int jobGroupId);
        IEnumerable<(int, string)> GetJobGroups();
    }

    public class JobGroupsDataService: IJobGroupsDataService
    {
        private readonly IDbConnection connection;

        public JobGroupsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public string? GetJobGroupName(int jobGroupId)
        {
            var name = connection.QueryFirstOrDefault<string?>(
                @"SELECT JobGroupName
                        FROM JobGroups
                        WHERE JobGroupID = @jobGroupId",
                new { jobGroupId }
            );

            return name;
        }

        public IEnumerable<(int, string)> GetJobGroups()
        {
            var jobGroups = connection.Query<(int, string)>(
                @"SELECT JobGroupID, JobGroupName
                        FROM JobGroups
                        ORDER BY JobGroupName"
            );
            return jobGroups;
        }
    }
}
