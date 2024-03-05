using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public  class CompetencySelfAssessmentCertificate
    {
        public int Id { get; set; }
        public string SelfAssessment { get; set; } = string.Empty;
        public string LearnerName { get; set; } = string.Empty;
        public string LearnerPRN { get; set; } = string.Empty;
        public int LearnerId { get; set; }
        public int LearnerDelegateAccountId { get; set; }
        public DateTime Verified { get; set; }
        public string CentreName { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = string.Empty;
        public string SupervisorCentreName { get; set; } = string.Empty;
        public string? SupervisorPRN { get; set; }
        public int CandidateAssessmentID { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public byte[]? BrandImage { get; set; }
        public int SelfAssessmentID { get; set; }
        public string? Vocabulary { get; set; }
        public int SupervisorDelegateId { get; set; }
        public string FormattedDate { get; set; } = string.Empty;
    }
}
