namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class CompetencyAssessmentsViewModel
    {
        public CompetencyAssessmentsViewModel(
            bool isWorkforceManager,
            bool isWorkforceContributor,
            AllCompetencyAssessmentsViewModel allCompetencyAssessments,
            MyCompetencyAssessmentsViewModel myCompetencyAssessments,
            CompetencyAssessmentsTab currentTab
        )
        {
            IsWorkforceManager = isWorkforceManager;
            IsWorkforceContributor = isWorkforceContributor;
            MyCompetencyAssessmentsViewModel = myCompetencyAssessments;
            AllCompetencyAssessmentsViewModel = allCompetencyAssessments;
            TabNavLinks = new TabsNavViewModel(currentTab);
        }

        public bool IsWorkforceManager { get; set; }
        public bool IsWorkforceContributor { get; set; }
        public MyCompetencyAssessmentsViewModel MyCompetencyAssessmentsViewModel { get; set; }
        public AllCompetencyAssessmentsViewModel AllCompetencyAssessmentsViewModel { get; set; }
        public TabsNavViewModel TabNavLinks { get; set; }
    }
}
