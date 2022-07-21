namespace DigitalLearningSolutions.Web.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;

    public interface IJobGroupsService
    {
        IEnumerable<(int, string)> GetJobGroupsAlphabetical();
    }

    public class JobGroupsService : IJobGroupsService
    {
        private readonly IJobGroupsDataService jobGroupsDataService;

        public JobGroupsService(IJobGroupsDataService jobGroupsDataService)
        {
            this.jobGroupsDataService = jobGroupsDataService;
        }

        public IEnumerable<(int, string)> GetJobGroupsAlphabetical()
        {
            return jobGroupsDataService.GetJobGroupsAlphabetical();
        }
    }
}
