namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class FaqItemsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableFaqViewModel> Faqs;

        public FaqItemsViewModel(IEnumerable<Faq> faqs)
        {
            Faqs = faqs.Select(f => new SearchableFaqViewModel(f));
        }
    }
}
