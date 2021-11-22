namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableFaqViewModel : BaseFilterableViewModel
    {
        public SearchableFaqViewModel(Faq faq)
        {
            Faq = faq;
        }

        public Faq Faq { get; set; }
    }
}
