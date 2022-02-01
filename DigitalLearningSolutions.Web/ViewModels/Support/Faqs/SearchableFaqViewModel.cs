namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.Faqs;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableFaqViewModel : BaseFilterableViewModel
    {
        public SearchableFaqViewModel(DlsSubApplication dlsSubApplication, SearchableFaq faq)
        {
            DlsSubApplication = dlsSubApplication;
            Faq = faq;
        }

        public SearchableFaq Faq { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
