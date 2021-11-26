namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableFaqViewModel : BaseFilterableViewModel
    {
        public SearchableFaqViewModel(DlsSubApplication dlsSubApplication, Faq faq)
        {
            DlsSubApplication = dlsSubApplication;
            Faq = faq;
        }

        public Faq Faq { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
