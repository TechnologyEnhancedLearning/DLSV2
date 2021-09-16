namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;

    public class SelfAssessmentOverviewViewModel
    {
        public CurrentSelfAssessment SelfAssessment { get; set; }
        public IEnumerable<IGrouping<string, Competency>> CompetencyGroups { get; set; }
        public int PreviousCompetencyNumber { get; set; }
        public int NumberOfOptionalCompetencies()
        {
            int optionalCompetencyCount = 0;
            foreach(var competencyGroup in CompetencyGroups)
            {
                foreach(var competency in competencyGroup)
                {
                    if (competency.Optional)
                    {
                        optionalCompetencyCount++;
                    }
                }
            }
            return optionalCompetencyCount;
        }
        public string VocabSingular()
        {
            return CompetencyGroups.FirstOrDefault().First().Vocabulary.ToLower();
        }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(VocabSingular());
        }
    }
}
