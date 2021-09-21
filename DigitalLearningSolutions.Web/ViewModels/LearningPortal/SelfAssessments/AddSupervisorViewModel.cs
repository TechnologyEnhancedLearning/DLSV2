namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.Common.Users;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class AddSupervisorViewModel
    {
        public int SelfAssessmentID { get; set; }
        public string? SelfAssessmentName { get; set; }
        public IEnumerable<Administrator>? Supervisors { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a supervisor")]
        public int SupervisorAdminID { get; set; }
    }
}
