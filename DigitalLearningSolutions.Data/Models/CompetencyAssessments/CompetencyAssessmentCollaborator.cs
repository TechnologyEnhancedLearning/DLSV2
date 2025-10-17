namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    public class CompetencyAssessmentCollaborator
    {
        public int ID { get; set; }
        public int SelfAssessmentID { get; set; }
        public int? AdminID { get; set; }
        public bool CanModify { get; set; }
    }
}
