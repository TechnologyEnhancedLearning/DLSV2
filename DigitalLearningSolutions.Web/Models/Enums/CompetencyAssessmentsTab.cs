namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System.Collections.Generic;

    public class CompetencyAssessmentsTab : BaseTabEnumeration
    {
        public static CompetencyAssessmentsTab MyCompetencyAssessments = new CompetencyAssessmentsTab(
            1,
            nameof(MyCompetencyAssessments),
            "CompetencyAssessments",
            "ViewCompetencyAssessments",
            "My Competency Assessments",
            new Dictionary<string, string> { { "tabName", "Mine" } }
        );

        public static CompetencyAssessmentsTab AllCompetencyAssessments = new CompetencyAssessmentsTab(
            2,
            nameof(AllCompetencyAssessments),
            "CompetencyAssessments",
            "ViewCompetencyAssessments",
            "All Competency Assessments",
            new Dictionary<string, string> { { "tabName", "All" } }
        );

        private CompetencyAssessmentsTab(
            int id,
            string name,
            string controller,
            string action,
            string linkText,
            Dictionary<string, string> staticRouteData
        ) : base(id, name, controller, action, linkText, staticRouteData) { }

        public override IEnumerable<BaseTabEnumeration> GetAllTabs()
        {
            return GetAll<CompetencyAssessmentsTab>();
        }
    }
}
