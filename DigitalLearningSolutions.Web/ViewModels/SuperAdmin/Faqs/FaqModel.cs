namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs
{
    using System;
    using DigitalLearningSolutions.Data.Models.Support;

    public class FaqModel
    {
        public FaqModel() {}

        public FaqModel(Faq model)
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
    }
}
