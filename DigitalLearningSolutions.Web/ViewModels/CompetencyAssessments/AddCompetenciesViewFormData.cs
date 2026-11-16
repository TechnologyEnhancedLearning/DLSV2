namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class AddCompetenciesFormData
    {
        public int ID { get; set; }
        public int[] SelectedFrameworkCompetencyIds { get; set; } = [];
        public int FrameworkId { get; set; }
    }
}
