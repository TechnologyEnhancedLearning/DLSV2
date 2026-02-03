using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class PublishReviewViewModel
    {
        public PublishReviewViewModel()
        {}
        public PublishReviewViewModel(int competencyAssessmentID, string competencyAssessmentName, IEnumerable<SelfAssessmentReview>? selfAssessmentReviews, CompetencyAssessmentBase competencyAssessmentBase)
        {
            CompetencyAssessmentID = competencyAssessmentID;
            CompetencyAssessmentName = competencyAssessmentName;
            SelfAssessmentReviews = selfAssessmentReviews;
            CompetencyAssessmentBase = competencyAssessmentBase;
            CanPublish = selfAssessmentReviews?.All(x => x.SignedOff) ?? true;
        }
        public int CompetencyAssessmentID { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public IEnumerable<SelfAssessmentReview>? SelfAssessmentReviews { get; set; }
        public CompetencyAssessmentBase CompetencyAssessmentBase { get; set; }
        public bool CanPublish { get; set; }
        public string VocabSingular()
        {
            return FrameworkVocabularyHelper.VocabularySingular(CompetencyAssessmentBase.Vocabulary);
        }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(CompetencyAssessmentBase.Vocabulary);
        }
    }
}
