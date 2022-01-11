namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableFaqViewModel : BaseFilterableViewModel
    {
        public SearchableFaqViewModel(DlsSubApplication dlsSubApplication, SearchableFaqModel faq)
        {
            DlsSubApplication = dlsSubApplication; //todo this is just copied from support I dont think we need them here
            Faq = faq;
        }

        public SearchableFaqModel Faq { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
