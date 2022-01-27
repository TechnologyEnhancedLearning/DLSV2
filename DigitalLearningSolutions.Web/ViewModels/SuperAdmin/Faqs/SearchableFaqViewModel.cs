namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.Shared.Faqs;

    public class SearchableFaqViewModel : BaseFilterableViewModel
    {
        public SearchableFaqViewModel(SearchableFaq faq)
        {
            FaqId = faq.FaqId;
            AHtml = faq.AHtml;
            CreatedDate = faq.CreatedDate.ToShortDateString();
            Published = faq.Published ? "Published" : "Unpublished";
            QAnchor = faq.QAnchor;
            QText = faq.QText;
            TargetGroup = Enumeration.GetAll<FaqsTargetGroup>()
                .Where(x => x.Id == faq.TargetGroup)
                .Select(x => x.DisplayName).First();
            Weighting = faq.Weighting;
        }

        public int FaqId { get; set; }
        public string AHtml { get; set; }
        public string CreatedDate { get; set; }
        public string Published { get; set; }
        public string QAnchor { get; set; }
        public string QText { get; set; }
        public string TargetGroup { get; set; }
        public int Weighting { get; set; }
    }
}
