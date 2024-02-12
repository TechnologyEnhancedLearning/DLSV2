namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.Linq;

    public class ReviewSelfAssessmentViewModel
    {
        public SupervisorDelegateDetail? SupervisorDelegateDetail { get; set; }
        public DelegateSelfAssessment DelegateSelfAssessment { get; set; }
        public IEnumerable<IGrouping<string, Competency>> CompetencyGroups { get; set; }
        public IEnumerable<SupervisorSignOff>? SupervisorSignOffs { get; set; }
        public bool IsSupervisorResultsReviewed { get; set; }
        public SearchSupervisorCompetencyViewModel SearchViewModel { get; set; }
        public string VocabPlural(string vocabulary)
        {
            return FrameworkVocabularyHelper.VocabularyPlural(vocabulary);
        }
        public int CandidateAssessmentId { get; set; }
        public bool ExportToExcelHide { get; set; }
        public IEnumerable<SupervisorDashboardToDoItem>? supervisorDashboardToDoItem { get; set; }
    }
}
