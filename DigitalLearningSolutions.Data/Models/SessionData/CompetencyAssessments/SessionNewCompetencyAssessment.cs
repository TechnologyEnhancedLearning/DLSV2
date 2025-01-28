namespace DigitalLearningSolutions.Data.Models.SessionData.CompetencyAssessments
{
    using System;
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    public class SessionNewCompetencyAssessment
    {
        public SessionNewCompetencyAssessment()
        {
            Id = new Guid();
            CompetencyAssessmentBase = new CompetencyAssessmentBase();
        }
        public Guid Id { get; set; }
        public CompetencyAssessmentBase CompetencyAssessmentBase { get; set; }
    }
}
