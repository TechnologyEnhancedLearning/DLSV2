using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class SelfAssessmentSupervisor
    {
        public int ID { get; set; }
        public int SupervisorDelegateID { get; set; }
        public int? SupervisorAdminID { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorEmail { get; set; }
        public DateTime NotificationSent { get; set; }
        public string RoleName { get; set; }
        public bool ReviewResults { get; set; }
        public bool SelfAssessmentReview { get; set; }
        public bool AddedByDelegate { get; set; }
        public DateTime? Confirmed { get; set; }
        public string CentreName { get; set; }
    }
}
