using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    public class SelfAssessmentSupervisorRole
    {
        public int ID { get; set; }
        public int SelfAssessmentID { get; set; }
        public string RoleName { get; set; }
        public string? RoleDescription { get; set; }
        public bool SelfAssessmentReview { get; set; }
        public bool ResultsReview { get; set; }
        public bool AllowSupervisorRoleSelection { get; set; }
    }
}
