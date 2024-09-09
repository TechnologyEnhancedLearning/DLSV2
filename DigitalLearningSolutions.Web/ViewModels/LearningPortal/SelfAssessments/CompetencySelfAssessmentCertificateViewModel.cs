using AngleSharp.Attributes;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.Helpers;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    public class CompetencySelfAssessmentCertificateViewModel
    {
        public CompetencySelfAssessmentCertificateViewModel()
        {

        }
        public CompetencySelfAssessmentCertificateViewModel(CompetencySelfAssessmentCertificate competency,
            IEnumerable<CompetencyCountSelfAssessmentCertificate> competencies,
            string vocabulary, IEnumerable<Accessor> accessors,
           ActivitySummaryCompetencySelfAssesment activitySummaryCompetencySelfAssesment,
           int questionResponses,
           int confirmedResponses,
           int? loggedInSupervisorDelegateId
         )
        {
            Vocabulary = vocabulary;
            CompetencySelfAssessmentCertificates = competency;
            CompetencyCountSelfAssessmentCertificate = competencies;
            VocabPlural = FrameworkVocabularyHelper.VocabularyPlural(competency.Vocabulary);
            Accessors = accessors;
            ActivitySummaryCompetencySelfAssesment = activitySummaryCompetencySelfAssesment;
            QuestionResponses = questionResponses;
            ConfirmedResponses = confirmedResponses;
            LoggedInSupervisorDelegateId = loggedInSupervisorDelegateId;
        }

        public string Vocabulary { get; set; }
        public string? VocabPlural { get; set; }
        public ActivitySummaryCompetencySelfAssesment ActivitySummaryCompetencySelfAssesment { get; set; }
        public CompetencySelfAssessmentCertificate CompetencySelfAssessmentCertificates { get; set; }
        public IEnumerable<CompetencyCountSelfAssessmentCertificate> CompetencyCountSelfAssessmentCertificate { get; set; }
        public IEnumerable<Accessor> Accessors { get; set; }
        public int QuestionResponses { get; set; }
        public int ConfirmedResponses { get; set; }
        public int? LoggedInSupervisorDelegateId { get; set; }

    }
}
