namespace DigitalLearningSolutions.Data.Models.SuperAdmin
{
    public class CentreSelfAssessment
    {
        public int SelfAssessmentId { get; set; }
        public int CentreId { get; set; }
        public string? CentreName { get; set; }
        public string? SelfAssessmentName { get; set; }
        public int DelegateCount { get; set; }
        public bool SelfEnrol { get; set; }
    }
}
