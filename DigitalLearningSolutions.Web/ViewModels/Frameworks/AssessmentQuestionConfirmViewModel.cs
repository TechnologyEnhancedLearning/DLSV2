namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System.Collections.Generic;

    public class AssessmentQuestionConfirmViewModel
    {
        public int FrameworkId { get; set; }
        public int FrameworkCompetencyId { get; set; }
        public string? Name { get; set; }
        public int AssessmentQuestionInputTypeID { get; set; }
        public Data.Models.SelfAssessments.AssessmentQuestion AssessmentQuestion { get; set; }
    }
}
