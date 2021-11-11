namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class CentresViewModel
    {
        public CentresViewModel(IEnumerable<Centre> centres)
        {
            // TODO: HEEDLS-641: add filters/sort/pagination
            // .Take(10) should be removed in HEEDLS-641 in favour of the standard pagination functionality.
            Centres = centres.OrderBy(c => c.CentreName).Take(10).Select(c => new CentreSummaryViewModel(c));
        }

        public IEnumerable<CentreSummaryViewModel> Centres { get; set; }
    }
}
