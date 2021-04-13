namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public class CompetencyGroupViewModel
    {
        public int FrameworkId { get; set; }
        public CompetencyGroupBase CompetencyGroupBase { get; set; }
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
