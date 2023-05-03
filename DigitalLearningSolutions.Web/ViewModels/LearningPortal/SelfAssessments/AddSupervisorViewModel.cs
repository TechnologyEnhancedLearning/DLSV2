namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Common.Users;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class AddSupervisorViewModel : BaseSearchablePageViewModel<Administrator>
    {
        public AddSupervisorViewModel(
            int selfAssessmentID,
            string? selfAssessmentName,
            int supervisorAdminID,
            SearchSortFilterPaginationResult<Administrator> result
            ) : base(result, false, searchLabel: "Search supervisors")
        {
            SelfAssessmentID = selfAssessmentID;
            SelfAssessmentName = selfAssessmentName;
            SupervisorAdminID = supervisorAdminID;
            Supervisors = result.ItemsToDisplay; 
        }

        public AddSupervisorViewModel() : this(
            0,
            string.Empty,
            0,
            new SearchSortFilterPaginationResult<Administrator>(
                Enumerable.Empty<Administrator>(),
                1,
                1,
                1,
                0,
                true,
                null,
                string.Empty,
                string.Empty,
                null
            )
        )
        { }

        public int SelfAssessmentID { get; set; }
        public string? SelfAssessmentName { get; set; }
        public IEnumerable<Administrator>? Supervisors { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a supervisor")]
        public int SupervisorAdminID { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };


        public override bool NoDataFound => !Supervisors.Any() && NoSearchOrFilter;
    }
}
