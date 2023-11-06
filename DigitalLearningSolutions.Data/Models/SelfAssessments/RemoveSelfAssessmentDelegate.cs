using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class RemoveSelfAssessmentDelegate
    {
        public int CandidateAssessmentsId { get; set; }
        public int SelfAssessmentID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? SelfAssessmentsName { get; set; }

    }
}
