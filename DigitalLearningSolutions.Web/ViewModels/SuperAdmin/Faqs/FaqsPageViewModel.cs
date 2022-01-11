namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class FaqsPageViewModel
    {
        public FaqsPageViewModel(IEnumerable<SearchableFaqModel> faqs)
        {
            Faqs = faqs.Select(f => new SearchableFaqViewModel(DlsSubApplication.SuperAdmin, f));
        }

        public IEnumerable<SearchableFaqViewModel> Faqs { get; set; }
    }
}
