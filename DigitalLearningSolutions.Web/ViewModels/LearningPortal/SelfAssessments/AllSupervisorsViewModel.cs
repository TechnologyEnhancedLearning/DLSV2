namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Common.Users;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class AllSupervisorsViewModel
    {
        public int SelfAssessmentID { get; set; }
        public string? SelfAssessmentName { get; set; }
        public IEnumerable<Administrator>? Supervisors { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a supervisor")]
        public int SupervisorAdminID { get; set; }
    }
}
