namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using System.ComponentModel.DataAnnotations;
    public class AddCompetenciesSelectFrameworkFormData
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Select a linked framework")]
        public int? FrameworkId { get; set; }
    }
}
