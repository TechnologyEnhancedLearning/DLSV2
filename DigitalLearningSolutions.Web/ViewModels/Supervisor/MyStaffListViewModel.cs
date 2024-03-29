﻿namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
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
        ) : base(result, false, searchLabel: "Search")
        {
            AdminUser = adminUser;
            CentreRegistrationPrompts = centreRegistrationPrompts;
            SuperviseDelegateDetailViewModels = result.ItemsToDisplay.Where(x=>x.SupervisorDelegateDetail.DelegateUserID != x.LoggedInUserId);
        }

        public MyStaffListViewModel() : this(
            null,
            new SearchSortFilterPaginationResult<SupervisorDelegateDetailViewModel>(
                Enumerable.Empty<SupervisorDelegateDetailViewModel>(),
                1,
                1,
                1,
                0,
                true,
                null,
                string.Empty,
                string.Empty,
                null
            ),
            new CentreRegistrationPrompts()
        )
        { }

        public IEnumerable<SupervisorDelegateDetailViewModel> SuperviseDelegateDetailViewModels { get; set; }

        public SupervisorDelegateDetailViewModel? SelfSuperviseDelegateDetailViewModels { get; set; }

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

        public bool IsActiveSupervisorDelegateExist { get; set; }

        public override bool NoDataFound => !SuperviseDelegateDetailViewModels.Any() && NoSearchOrFilter;

        [Required(ErrorMessage = "Enter an email")]
        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(ErrorMessage = CommonValidationErrorMessages.WhitespaceInEmail)]
        public string? DelegateEmailAddress { get; set; }
    }
}
