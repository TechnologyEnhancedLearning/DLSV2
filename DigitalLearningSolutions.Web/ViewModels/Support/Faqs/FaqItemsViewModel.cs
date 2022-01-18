namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class FaqItemsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableFaqViewModel> Faqs;

        public FaqItemsViewModel(DlsSubApplication dlsSubApplication, IEnumerable<SearchableFaq> faqs)
        {
            Faqs = faqs.Select(f => new SearchableFaqViewModel(dlsSubApplication, f));
        }
    }
}
