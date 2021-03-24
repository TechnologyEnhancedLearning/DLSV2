namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AssessmentQuestionViewModel
    {
        public int FrameworkId { get; set; }
        public int FrameworkCompetencyId { get; set; }
        public string? Name { get; set; }
        public AssessmentQuestionDetail AssessmentQuestionDetail { get; set; }
        public SelectList? InputTypeSelectList { get; set; }
    }
}
