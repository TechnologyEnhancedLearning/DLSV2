namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    public class CompetencyAssessmentQuestionsViewModel
    {
        public int FrameworkId { get; set; }
        public int FrameworkCompetencyId { get; set; }
        public string? CompetencyName { get; set; }
        public SelectList? QuestionSelectList { get; set; }
        public IEnumerable<AssessmentQuestion>? AssessmentQuestions { get; set; }
        public int assessmentQuestionId { get; set; }
    }
}
