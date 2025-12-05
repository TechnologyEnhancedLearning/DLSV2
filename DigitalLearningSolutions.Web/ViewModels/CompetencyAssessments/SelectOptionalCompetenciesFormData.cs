namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SelectOptionalCompetenciesFormData
    {
        public int ID { get; set; }
        public bool? TaskStatus { get; set; }
        public int? MinimumOptionalCompetencies { get; set; }
        public string? ManageOptionalCompetenciesPrompt { get; set; }
        public int[] SelectedCompetencyIds { get; set; } = [];
        public int[] GroupIds { get; set; } = [];
    }
}
