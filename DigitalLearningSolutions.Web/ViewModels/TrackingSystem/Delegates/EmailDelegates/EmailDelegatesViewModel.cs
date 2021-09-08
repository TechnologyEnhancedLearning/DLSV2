namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class EmailDelegatesViewModel : BaseSearchablePageViewModel, IValidatableObject
    {
        public EmailDelegatesViewModel(string? filterBy) : base(
            null,
            1,
            true,
            DelegateSortByOptions.RegistrationDate.PropertyName,
            Ascending,
            filterBy,
            int.MaxValue
        ) { }

        public EmailDelegatesViewModel() : this(null) { }

        public EmailDelegatesViewModel(
            IEnumerable<DelegateUserCard> delegateUsers,
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CustomPrompt> customPrompts,
            string? filterBy
        ) : this(filterBy)
        {
            Day = DateTime.Today.Day;
            Month = DateTime.Today.Month;
            Year = DateTime.Today.Year;
            SetDelegates(delegateUsers, filterBy);
            SetFilters(jobGroups, customPrompts);
        }

        public IEnumerable<EmailDelegatesItemViewModel>? Delegates { get; set; }

        [Required(ErrorMessage = "You must select at least one delegate")]
        public IEnumerable<int>? SelectedDelegateIds { get; set; }

        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new List<(string, string)>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "Email delivery date", true)
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }

        public void SetFilters(IEnumerable<(int id, string name)> jobGroups, IEnumerable<CustomPrompt> customPrompts)
        {
            var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            Filters = EmailDelegatesViewModelFilterOptions.GetEmailDelegatesFilterViewModels(
                jobGroups,
                promptsWithOptions
            );
        }

        public void SetDelegates(IEnumerable<DelegateUserCard> delegateUsers, string? filterBy)
        {
            var filteredItems = FilteringHelper.FilterItems(delegateUsers.AsQueryable(), filterBy).ToList();
            Delegates = filteredItems.Select(
                delegateUser =>
                {
                    var preChecked = SelectedDelegateIds != null && SelectedDelegateIds.Contains(delegateUser.Id);
                    return new EmailDelegatesItemViewModel(delegateUser, preChecked);
                }
            );
            MatchingSearchResults = Delegates.Count();
        }
    }
}
