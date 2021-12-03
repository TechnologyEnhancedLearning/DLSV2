namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Helpers;

    public class FaqViewModel : BaseSearchableItem
    {
        public FaqViewModel() { }

        public FaqViewModel(Faq model)
        {
            FaqId = model.FaqId;
            Ahtml = model.Ahtml;
            CreatedDate = model.CreatedDate;
            Published = model.Published;
            Qanchor = model.Qanchor;
            Qtext = model.Qtext;
            TargetGroup = model.TargetGroup;
            Weighting = model.Weighting;
        }

        public int FaqId { get; set; }
        public string Ahtml { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Published { get; set; }
        public string Qanchor { get; set; }
        public string Qtext { get; set; }
        public int TargetGroup { get; set; }
        public int Weighting { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ??
                   $"{Qtext} {StringHelper.ReplaceNoneAlphaNumericSpaceChars(Ahtml, " ")}";
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
