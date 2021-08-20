namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class AllDelegatesViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionViewModel> PasswordStatusOptions = new[]
        {
            DelegatePasswordStatusFilterOptions.PasswordSet,
            DelegatePasswordStatusFilterOptions.PasswordNotSet
        };

        public static readonly IEnumerable<FilterOptionViewModel> AdminStatusOptions = new[]
        {
            DelegateAdminStatusFilterOptions.IsAdmin,
            DelegateAdminStatusFilterOptions.IsNotAdmin
        };

        public static readonly IEnumerable<FilterOptionViewModel> ActiveStatusOptions = new[]
        {
            DelegateActiveStatusFilterOptions.IsActive,
            DelegateActiveStatusFilterOptions.IsNotActive
        };

        public static IEnumerable<FilterOptionViewModel> GetJobGroupOptions(
            IEnumerable<(int id, string name)> jobGroups
        )
        {
            return jobGroups.Select(
                j => new FilterOptionViewModel(
                    j.name,
                    nameof(DelegateUserCard.JobGroupId) + FilteringHelper.Separator +
                    nameof(DelegateUserCard.JobGroupId) +
                    FilteringHelper.Separator + j.id,
                    FilterStatus.Default
                )
            );
        }

        public static IEnumerable<FilterOptionViewModel> GetCustomPromptOptions(
            CustomPrompt customPrompt
        )
        {
            string filterValueName =
                CustomPromptHelper.GetDelegateCustomPromptAnswerName(customPrompt.CustomPromptNumber);

            var options = customPrompt.Options.Select(
                x => new FilterOptionViewModel(
                    x,
                    filterValueName + FilteringHelper.Separator + filterValueName + FilteringHelper.Separator + x,
                    FilterStatus.Default
                )
            ).ToList();
            options.Add(
                new FilterOptionViewModel(
                    "No option selected",
                    filterValueName + FilteringHelper.Separator + filterValueName + FilteringHelper.Separator +
                    FilteringHelper.EmptyValue,
                    FilterStatus.Default
                )
            );
            return options;
        }
    }
}
