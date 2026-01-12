namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class ManageCompetencyRoleRequirementsFormData
    {
        public int Id { get; set; }
        public bool? TaskStatus { get; set; }
        public bool EnforceRoleRequirementsForSignOff { get; set; }
        public bool IncludeRequirementsFilters { get; set; }
    }
}
