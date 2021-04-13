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
        public string? frameworkConfig { get; set; }
        public string vocabSingular()
        {
            if (frameworkConfig == null)
            {
                return "Capability";
            }
            else
            {
                return frameworkConfig;
            }
        }
        public string vocabPlural()
        {
            if (frameworkConfig == null)
            {
                return "Capabilities";
            }
            else
            {
                if (frameworkConfig.EndsWith("y"))
                {
                    return frameworkConfig.Substring(0, frameworkConfig.Length - 1) + "ies";
                }
                else
                {
                    return frameworkConfig + "s";
                }
            }
        }
    }
}
