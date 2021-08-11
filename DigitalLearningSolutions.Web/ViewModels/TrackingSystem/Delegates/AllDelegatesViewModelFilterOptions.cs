namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
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
            string filterValueName = customPrompt.CustomPromptNumber switch
            {
                1 => nameof(DelegateUserCard.Answer1),
                2 => nameof(DelegateUserCard.Answer2),
                3 => nameof(DelegateUserCard.Answer3),
                4 => nameof(DelegateUserCard.Answer4),
                5 => nameof(DelegateUserCard.Answer5),
                6 => nameof(DelegateUserCard.Answer6),
                _ => throw new ArgumentOutOfRangeException()
            };

            return customPrompt.Options.Select(
                x => new FilterOptionViewModel(
                    x,
                    filterValueName + FilteringHelper.Separator + filterValueName + FilteringHelper.Separator + x,
                    FilterStatus.Default
                )
            );
        }
    }
}
