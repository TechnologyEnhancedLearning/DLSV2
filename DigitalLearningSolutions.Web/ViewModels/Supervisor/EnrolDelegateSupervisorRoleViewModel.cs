namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EnrolDelegateSupervisorRoleViewModel
    {
        public SupervisorDelegateDetail? SupervisorDelegateDetail { get; set; }
        public CompetencyAssessment? CompetencyAssessment { get; set; }
        [Required(ErrorMessage = "Please choose a supervisor role")]
        public int? SelfAssessmentSupervisorRoleId { get; set; }
        public IEnumerable<SelfAssessmentSupervisorRole>? SelfAssessmentSupervisorRoles { get; set; }
    }
}
