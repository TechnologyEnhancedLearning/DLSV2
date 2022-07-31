namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class EmailDelegatesViewModel : BaseSearchablePageViewModel<DelegateUserCard>
    {
        private EmailDelegatesViewModel(
            SearchSortFilterPaginationResult<DelegateUserCard> result,
            IEnumerable<FilterModel> availableFilters,
            bool selectAll
        ) : base(result, true, availableFilters)
        {
            Delegates = result.ItemsToDisplay.Select(
                delegateUser =>
                {
                    var delegateSelected = selectAll ||
                                           SelectedDelegateIds != null && SelectedDelegateIds.Contains(delegateUser.Id);
                    return new EmailDelegatesItemViewModel(delegateUser, delegateSelected);
                }
            );
        }

        public EmailDelegatesViewModel(
            SearchSortFilterPaginationResult<DelegateUserCard> result,
            IEnumerable<FilterModel> availableFilters,
            DateTime emailDate,
            bool selectAll = false
        ) : this(result, availableFilters, selectAll)
        {
            Day = emailDate.Day;
            Month = emailDate.Month;
            Year = emailDate.Year;
        }

        public EmailDelegatesViewModel(
            SearchSortFilterPaginationResult<DelegateUserCard> result,
            IEnumerable<FilterModel> availableFilters,
            EmailDelegatesFormData formData
        ) : this(result, availableFilters, false)
        {
            SelectedDelegateIds = formData.SelectedDelegateIds;
            Day = formData.Day;
            Month = formData.Month;
            Year = formData.Year;
        }

        public IEnumerable<EmailDelegatesItemViewModel>? Delegates { get; set; }

        public IEnumerable<int>? SelectedDelegateIds { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new List<(string, string)>();

        public override bool NoDataFound => Delegates != null && !Delegates.Any() && NoSearchOrFilter;
    }
}
