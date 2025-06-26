namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using System.ComponentModel.DataAnnotations;
    public class SelectFrameworkSourcesFormData
    {
        [Required(ErrorMessage = "Select a framework")]
        public int FrameworkId { get; set; }
        public int CompetencyAssessmentId { get; set; }
        public bool? TaskStatus { get; set; }
        public string? ActionName { get; set; }
    }
}
