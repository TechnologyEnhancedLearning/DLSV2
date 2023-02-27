namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using Microsoft.Extensions.Logging;

    public interface IJobGroupsDataService
    {
        string? GetJobGroupName(int jobGroupId);
        IEnumerable<(int id, string name)> GetJobGroupsAlphabetical();
    }

    public class JobGroupsDataService : IJobGroupsDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<JobGroupsDataService> logger;

        public JobGroupsDataService(IDbConnection connection, ILogger<JobGroupsDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public string? GetJobGroupName(int jobGroupId)
        {
            var name = connection.QueryFirstOrDefault<string?>(
                @"SELECT JobGroupName
                        FROM JobGroups
                        WHERE JobGroupID = @jobGroupId",
                new { jobGroupId }
            );
            if (name == null)
            {
                logger.LogWarning(
                    $"No job group found for job group id {jobGroupId}"
                );
            }

            return name;
        }

        public IEnumerable<(int, string)> GetJobGroupsAlphabetical()
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
