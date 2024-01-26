namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class SelfAssessmentForPublish
    {
        public int Id { get; set; }
        public string? SelfAssessment { get; set; }
        public bool National { get; set; }
        public string? Provider { get; set; }
    }
}
