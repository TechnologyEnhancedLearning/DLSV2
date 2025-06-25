namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using System.ComponentModel.DataAnnotations;
    public class AddCompetenciesSelectFrameworkFormData
    {
        [Required]
        public int ID { get; set; }
        public int FrameworkId { get; set; }
    }
}
