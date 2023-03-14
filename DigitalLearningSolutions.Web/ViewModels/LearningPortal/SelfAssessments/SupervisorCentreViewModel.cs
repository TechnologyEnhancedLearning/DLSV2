using DigitalLearningSolutions.Data.Models.Centres;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    public class SupervisorCentresViewModel
    {
        public int SelfAssessmentID { get; set; }
        public string? SelfAssessmentName { get; set; }
        public List<Centre>? Centres { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a centre")]
        public int CentreID { get; set; }
    }
}
