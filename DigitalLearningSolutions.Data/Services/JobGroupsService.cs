namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using Microsoft.Extensions.Logging;

    public interface IJobGroupsService
    {
        string? GetJobGroupName(int jobGroupId);
        IEnumerable<(int id, string name)> GetJobGroupsAlphabetical();
    }

    public class JobGroupsService : IJobGroupsService
    {
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly ILogger<JobGroupsService> logger;

        public JobGroupsService(IJobGroupsDataService jobGroupsDataService, ILogger<JobGroupsService> logger)
        {
            this.jobGroupsDataService = jobGroupsDataService;
            this.logger = logger;
        }

        public string? GetJobGroupName(int jobGroupId)
        {
            var name = jobGroupsDataService.GetJobGroupName(jobGroupId);
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
            return jobGroupsDataService.GetJobGroupsAlphabetical();
        }
    }
}
