using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class CompetencySummary
    {
        public int VerifiedCount { get; set; }
        public int QuestionsCount { get; set; }
        public bool CanViewCertificate { get; set; }
    }
}
