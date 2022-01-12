namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs
{
    using System.Collections.Generic;
    using System.Linq;

    public class FaqsPageViewModel
    {
        public FaqsPageViewModel(IEnumerable<SearchableFaqModel> faqs)
        {
            Faqs = faqs.Select(f => new SearchableFaqViewModel(f));
        }

        public IEnumerable<SearchableFaqViewModel> Faqs { get; set; }
    }
}
