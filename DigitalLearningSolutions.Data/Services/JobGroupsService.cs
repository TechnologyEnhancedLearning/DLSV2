namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using Microsoft.Extensions.Logging;

    public interface IJobGroupsService
    {
        string? GetJobGroupName(int jobGroupId);
        IEnumerable<(int, string)> GetJobGroups();
    }

    public class JobGroupsService : IJobGroupsService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<JobGroupsService> logger;

        public JobGroupsService(IDbConnection connection, ILogger<JobGroupsService> logger)
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
