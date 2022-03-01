namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Data.Models.User;

    public class MyStaffListViewModel : BaseSearchablePageViewModel
    {
        public MyStaffListViewModel(
            AdminUser adminUser,
            IEnumerable<SupervisorDelegateDetailViewModel> supervisorDelegateDetailViewModels,
            CentreCustomPrompts centreCustomPrompts,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page
        ) : base(searchString, page, false, sortBy, sortDirection, searchLabel: "Search administrators")
        {
            IsNominatedSupervisor = adminUser?.IsNominatedSupervisor ?? false;
            CentreCustomPrompts = centreCustomPrompts;
            var sortedItems = GenericSortingHelper.SortAllItems(
                supervisorDelegateDetailViewModels.AsQueryable(),
                sortBy,
                sortDirection
            );
            var searchedItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = searchedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(searchedItems);
            SuperviseDelegateDetailViewModels = paginatedItems;
        }

        public MyStaffListViewModel() : this(null, Enumerable.Empty<SupervisorDelegateDetailViewModel>(), new CentreCustomPrompts(), null, string.Empty, string.Empty, 1)
        {

        }

        public IEnumerable<SupervisorDelegateDetailViewModel> SuperviseDelegateDetailViewModels { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };

        public CentreCustomPrompts CentreCustomPrompts { get; set; }
        public bool IsNominatedSupervisor { get; set; }

        public override bool NoDataFound => !SuperviseDelegateDetailViewModels.Any() && NoSearchOrFilter;

        [Required(ErrorMessage = "Enter an email address")]
        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(CommonValidationErrorMessages.WhitespaceInEmail)]
        public string? DelegateEmail { get; set; }
    }
}
