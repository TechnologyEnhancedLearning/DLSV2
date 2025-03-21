namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using System.ComponentModel.DataAnnotations;
    public class SelectFrameworkSourcesFormData
    {
        [Required]
        public int[] SelectedFrameworkIds { get; set; }
        public int CompetencyAssessmentId { get; set; }
    }
}
