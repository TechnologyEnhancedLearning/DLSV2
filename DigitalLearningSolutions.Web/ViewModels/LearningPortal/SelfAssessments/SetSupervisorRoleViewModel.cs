namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Common.Users;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Attributes;

    public class SetSupervisorRoleViewModel
    {
        public int SelfAssessmentID { get; set; }
        public int? SupervisorDelegateId { get; set; }
        public string SelfAssessmentName { get; set; }
        public Administrator? Supervisor { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a supervisor role")]
        public int SelfAssessmentSupervisorRoleId { get; set; }
        public IEnumerable<SelfAssessmentSupervisorRole>? SelfAssessmentSupervisorRoles { get; set; }
        public int? CentreID { get; set; }
    }
}
