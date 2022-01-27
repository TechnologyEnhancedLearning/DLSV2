namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Shared.Faqs;

    public class FaqsPageViewModel
    {
        public FaqsPageViewModel(IEnumerable<SearchableFaq> faqs)
        {
            Faqs = faqs.Select(f => new SearchableFaqViewModel(f));
            CurrentPage = SuperAdminSystemPage.FAQs;
        }

        public IEnumerable<SearchableFaqViewModel> Faqs { get; set; }
        public SuperAdminSystemPage CurrentPage { get; set; }
    }
}
