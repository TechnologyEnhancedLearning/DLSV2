namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Centres;

    public class CentresViewModel
    {
        public CentresViewModel(IEnumerable<CentreSummaryForSuperAdmin> centreSummaries)
        {
            // TODO: HEEDLS-641: add filters/sort/pagination
            // .Take(10) should be removed in HEEDLS-641 in favour of the standard pagination functionality.
            Centres = centreSummaries.OrderBy(c => c.CentreName).Take(10).Select(c => new CentreSummaryViewModel(c));
        }

        public IEnumerable<CentreSummaryViewModel> Centres { get; set; }
    }
}
