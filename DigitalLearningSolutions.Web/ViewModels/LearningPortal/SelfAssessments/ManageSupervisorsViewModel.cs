namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using System.Collections.Generic;
    public class ManageSupervisorsViewModel
    {
        public CurrentSelfAssessment SelfAssessment { get; set; }
        public List<SelfAssessmentSupervisor> Supervisors { get; set; }
        public List<SelfAssessmentSupervisor> SuggestedSupervisors { get; set; }
    }
}
