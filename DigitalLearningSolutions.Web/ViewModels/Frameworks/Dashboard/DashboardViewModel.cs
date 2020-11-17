namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System.Collections.Generic;
    public class DashboardViewModel
    {
        public readonly IEnumerable<BaseFramework> BaseFrameworks;
        public DashboardViewModel(IEnumerable<BaseFramework> baseFramework)
        {
            BaseFrameworks = baseFramework;
        }
    }
}
