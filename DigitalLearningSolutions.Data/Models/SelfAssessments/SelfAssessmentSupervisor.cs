﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class SelfAssessmentSupervisor
    {
        public int ID { get; set; }
        public int SupervisorDelegateID { get; set; }
        public int? SupervisorAdminID { get; set; }
        public string SupervisorName { get; set; } = string.Empty;
        public string SupervisorEmail { get; set; } = string.Empty;
        public DateTime NotificationSent { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool ReviewResults { get; set; }
        public bool SelfAssessmentReview { get; set; }
        public bool AddedByDelegate { get; set; }
        public DateTime? Confirmed { get; set; }
        public string CentreName { get; set; } = string.Empty;
        public bool AllowDelegateNomination { get; set; }
        public bool AllowSupervisorRoleSelection { get; set; }
}
}
