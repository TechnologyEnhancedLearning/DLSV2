namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using Microsoft.Extensions.Logging;

    public interface IJobGroupsService
    {
        IEnumerable<(int, string)> GetJobGroups();
    }

    public class JobGroupsService : IJobGroupsService
    {
        private readonly IDbConnection connection;

        public JobGroupsService(IDbConnection connection)
        {
            this.connection = connection;
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
