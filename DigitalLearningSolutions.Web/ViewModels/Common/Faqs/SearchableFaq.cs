namespace DigitalLearningSolutions.Web.ViewModels.Common.Faqs
{
    using System;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Helpers;

    public class SearchableFaq : BaseSearchableItem
    {
        public SearchableFaq() { }

        public SearchableFaq(Faq model)
        {
            FaqId = model.FaqId;
            AHtml = model.AHtml;
            CreatedDate = model.CreatedDate;
            Published = model.Published;
            QAnchor = model.QAnchor;
            QText = model.QText;
            TargetGroup = model.TargetGroup;
            Weighting = model.Weighting;
        }

        public int FaqId { get; set; }
        public string AHtml { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Published { get; set; }
        public string QAnchor { get; set; }
        public string QText { get; set; }
        public int TargetGroup { get; set; }
        public int Weighting { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? QText;
            set => SearchableNameOverrideForFuzzySharp = value;
        }

        public string SearchableFaqAnswer => DisplayStringHelper.ReplaceNonAlphaNumericSpaceChars(AHtml, " ")!;

        public override string?[] SearchableContent => new [] { SearchableName, SearchableFaqAnswer };
    }
}
