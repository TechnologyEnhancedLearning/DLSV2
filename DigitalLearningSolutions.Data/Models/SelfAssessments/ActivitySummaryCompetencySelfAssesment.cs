using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public  class ActivitySummaryCompetencySelfAssesment
    {
        public int CandidateAssessmentID { get; set; } = 0;
        public int SelfAssessmentID { get; set; } = 0;
        public string RoleName { get; set; } = string.Empty;
        public int CandidateAssessmentSupervisorVerificationId { get; set; }
        public int CompetencyAssessmentQuestionCount { get; set; } = 0;
        public int ResultCount { get; set; } = 0;
        public int VerifiedCount { get; set; } = 0;

    }
}
