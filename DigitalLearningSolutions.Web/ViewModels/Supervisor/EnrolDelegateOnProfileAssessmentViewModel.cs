namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using DigitalLearningSolutions.Data.Models.SessionData.Supervisor;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System.Collections.Generic;
    public class EnrolDelegateOnProfileAssessmentViewModel
    {
        public SessionEnrolOnRoleProfile SessionEnrolOnRoleProfile { get; set; }
        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public IEnumerable<RoleProfile> RoleProfiles { get; set; }
    }
}
