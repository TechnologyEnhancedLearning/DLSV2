using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    public abstract class BaseJavascriptFilterableViewModel
    {
        protected BaseJavascriptFilterableViewModel()
        {
            Filters = new List<AppliedFilterViewModel>();
        }

        public IEnumerable<AppliedFilterViewModel> Filters { get; set; }
    }
}
