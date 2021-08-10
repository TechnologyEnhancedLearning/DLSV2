namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using System.Collections.Generic;
    public class ManageSupervisorsViewModel
    {
        public CurrentSelfAssessment SelfAssessment { get; set; }
        public List<Supervisor> Supervisors { get; set; }
        public List<Supervisor> SuggestedSupervisors { get; set; }
    }
}
