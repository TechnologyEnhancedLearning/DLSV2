namespace DigitalLearningSolutions.Data.Models.Support
{
    using System;

    public class Faq
    {
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
