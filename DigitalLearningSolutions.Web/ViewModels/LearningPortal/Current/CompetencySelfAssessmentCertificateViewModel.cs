using AngleSharp.Attributes;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.Helpers;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    public class CompetencySelfAssessmentCertificateViewModel
    {
        public CompetencySelfAssessmentCertificateViewModel()
        {

        }
        public CompetencySelfAssessmentCertificateViewModel(CompetencySelfAssessmentCertificate competency,
            IEnumerable<CompetencyCountSelfAssessmentCertificate> competencies,
            int route, IEnumerable<Accessor> accessors,
           ActivitySummaryCompetencySelfAssesment activitySummaryCompetencySelfAssesment,
           int questionResponses,
           int confirmedResponses,
           int? loggedInSupervisorDelegateId
         )
        {
            Route = route;
            CompetencySelfAssessmentCertificates = competency;
            CompetencyCountSelfAssessmentCertificate = competencies;
            VocabPlural = FrameworkVocabularyHelper.VocabularyPlural(competency.Vocabulary);
            Accessors = accessors;
            ActivitySummaryCompetencySelfAssesment = activitySummaryCompetencySelfAssesment;
            QuestionResponses = questionResponses;
            ConfirmedResponses = confirmedResponses;
            LoggedInSupervisorDelegateId = loggedInSupervisorDelegateId;
        }

        public int Route { get; set; }
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
