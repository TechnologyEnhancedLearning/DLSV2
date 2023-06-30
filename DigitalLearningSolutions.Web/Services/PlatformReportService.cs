namespace DigitalLearningSolutions.Web.Services
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.PlatformReports;

    public interface IPlatformReportsService
    {
        PlatformUsageSummary GetPlatformUsageSummary();
    }

    public class PlatformReportsService : IPlatformReportsService
    {
        private readonly IPlatformReportsDataService platformReportsDataService;
        public PlatformReportsService(IPlatformReportsDataService platformReportsDataService)
        {
            this.platformReportsDataService = platformReportsDataService;
        }

        public PlatformUsageSummary GetPlatformUsageSummary()
        {
           return platformReportsDataService.GetPlatformUsageSummary();
        }
    }
}
