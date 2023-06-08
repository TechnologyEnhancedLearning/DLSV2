namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System;

    public class EnrolDelegateSummaryViewModel
    {
        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public RoleProfile RoleProfile { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public string SupervisorRoleName { get; set; }
        public int SupervisorRoleCount { get; set; }
        public bool AllowSupervisorRoleSelection { get; set; }
    }
}
