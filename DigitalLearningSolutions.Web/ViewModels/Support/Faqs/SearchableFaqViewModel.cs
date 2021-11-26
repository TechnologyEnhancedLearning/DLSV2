namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableFaqViewModel : BaseFilterableViewModel
    {
        public SearchableFaqViewModel(DlsSubApplication dlsSubApplication, FaqViewModel faq)
        {
            DlsSubApplication = dlsSubApplication;
            Faq = faq;
        }

        public FaqViewModel Faq { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
