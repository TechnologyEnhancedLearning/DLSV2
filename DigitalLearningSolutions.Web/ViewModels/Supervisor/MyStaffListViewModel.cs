namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class MyStaffListViewModel : BaseSearchablePageViewModel<SupervisorDelegateDetailViewModel>
    {
        private readonly AdminUser AdminUser;

        public MyStaffListViewModel(
            AdminUser adminUser,
            SearchSortFilterPaginationResult<SupervisorDelegateDetailViewModel> result,
            CentreRegistrationPrompts centreRegistrationPrompts
        ) : base(result, false, searchLabel: "Search administrators")
        {
            AdminUser = adminUser;
            CentreRegistrationPrompts = centreRegistrationPrompts;
            SuperviseDelegateDetailViewModels = result.ItemsToDisplay;
        }

        public MyStaffListViewModel() : this(
            null,
            new SearchSortFilterPaginationResult<SupervisorDelegateDetailViewModel>(
                Enumerable.Empty<SupervisorDelegateDetailViewModel>(),
                1,
                1,
                1,
                0,
                null,
                string.Empty,
                string.Empty,
                null
            ),
            new CentreRegistrationPrompts()
        ) { }

        public IEnumerable<SupervisorDelegateDetailViewModel> SuperviseDelegateDetailViewModels { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };

        public CentreRegistrationPrompts CentreRegistrationPrompts { get; set; }

        public bool IsNominatedSupervisor
        {
            get
            {
                return AdminUser?.IsSupervisor == true ? false : AdminUser?.IsNominatedSupervisor ?? false;
            }
        }

        public override bool NoDataFound => !SuperviseDelegateDetailViewModels.Any() && NoSearchOrFilter;

        [Required(ErrorMessage = "Enter an email address")]
        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(CommonValidationErrorMessages.WhitespaceInEmail)]
        public string? DelegateEmail { get; set; }
    }
}
