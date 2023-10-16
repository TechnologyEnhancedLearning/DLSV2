namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Web.Models.Enums;
    public class PlatformReportsViewModel
    {
        public SuperAdminReportsPage CurrentPage => SuperAdminReportsPage.PlatformUsage;
        public PlatformUsageSummary PlatformUsageSummary { get; set; }
    }
}
