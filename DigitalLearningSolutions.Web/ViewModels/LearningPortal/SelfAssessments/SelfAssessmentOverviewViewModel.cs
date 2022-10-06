namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;

    public class SelfAssessmentOverviewViewModel
    {
        public CurrentSelfAssessment SelfAssessment { get; set; }
        public IEnumerable<IGrouping<string, Competency>> CompetencyGroups { get; set; }
        public IEnumerable<SupervisorSignOff>? SupervisorSignOffs { get; set; }
        public int PreviousCompetencyNumber { get; set; }
        public int NumberOfOptionalCompetencies { get; set; }
        public bool AllQuestionsVerifiedOrNotRequired { get; set; }
        public SearchSelfAssessmentOvervieviewViewModel SearchViewModel { get; set; }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(SelfAssessment.Vocabulary);
        }
        public void Initialise(List<Competency> unfilteredCompetencies)
        {
            AllQuestionsVerifiedOrNotRequired = true;
            foreach (var assessmentQuestion in unfilteredCompetencies.SelectMany(c => c.AssessmentQuestions))
            {
                if ((assessmentQuestion.Result == null || assessmentQuestion.Verified == null || assessmentQuestion.SignedOff != true) && assessmentQuestion.Required)
                {
                    AllQuestionsVerifiedOrNotRequired = false;
                    break;
                }

                if (SelfAssessment.EnforceRoleRequirementsForSignOff &&
                    (assessmentQuestion.ResultRAG == 1 || assessmentQuestion.ResultRAG == 2))
                {
                    AllQuestionsVerifiedOrNotRequired = false;
                    break;
                }
            }
        }
    }
}
