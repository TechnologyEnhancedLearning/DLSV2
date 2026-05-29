using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class AddCompetenciesFormData
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Select at least one competency")]
        public int[] SelectedCompetencyIds { get; set; }
        public int FrameworkId { get; set; }
    }
}
