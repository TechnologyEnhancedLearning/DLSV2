namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.Linq;

    public class VerificationPickResultsViewModel
    {
        public int SelfAssessmentId { get; set; }
        public string? Vocubulary { get; set; }
        public string? SelfAssessmentName { get; set; }
        public IEnumerable<IGrouping<string, Competency>> CompetencyGroups { get; set; }
        public List<int> ResultIds { get; set; }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(Vocubulary);
        }
    }
}
