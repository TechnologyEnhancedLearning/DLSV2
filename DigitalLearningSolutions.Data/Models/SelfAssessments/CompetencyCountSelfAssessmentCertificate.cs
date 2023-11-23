using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class CompetencyCountSelfAssessmentCertificate
    {
        public int CompetencyGroupID { get; set; }
        public int OptionalCompetencyCount { get; set; }
        public string CompetencyGroup { get; set; } = string.Empty;
    }
}
