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
