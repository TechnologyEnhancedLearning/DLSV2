﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates
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

        public static readonly IEnumerable<FilterOptionViewModel> RegistrationTypeOptions = new[]
        {
            DelegateRegistrationTypeFilterOptions.SelfRegistered,
            DelegateRegistrationTypeFilterOptions.SelfRegisteredExternal,
            DelegateRegistrationTypeFilterOptions.RegisteredByCentre
        };

        public static IEnumerable<FilterOptionViewModel> GetJobGroupOptions(
            IEnumerable<(int id, string name)> jobGroups
        )
        {
            return jobGroups.Select(
                jobGroup => new FilterOptionViewModel(
                    jobGroup.name,
                    FilteringHelper.BuildFilterValueString(
                        nameof(DelegateUserCard.JobGroupId),
                        nameof(DelegateUserCard.JobGroupId),
                        jobGroup.id.ToString()
                    ),
                    FilterStatus.Default
                )
            );
        }

        public static IEnumerable<FilterOptionViewModel> GetCustomPromptOptions(
            CustomPrompt customPrompt
        )
        {
            string filterValueName =
                CentreCustomPromptHelper.GetDelegateCustomPromptAnswerName(customPrompt.CustomPromptNumber);

            var options = customPrompt.Options.Select(
                option => new FilterOptionViewModel(
                    option,
                    FilteringHelper.BuildFilterValueString(filterValueName, filterValueName, option),
                    FilterStatus.Default
                )
            ).ToList();
            options.Add(
                new FilterOptionViewModel(
                    "No option selected",
                    FilteringHelper.BuildFilterValueString(
                        filterValueName,
                        filterValueName,
                        FilteringHelper.EmptyValue.ToString()
                    ),
                    FilterStatus.Default
                )
            );
            return options;
        }

        public static List<FilterViewModel> GetAllDelegatesFilterViewModels(
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CustomPrompt> promptsWithOptions
        )
        {
            var filters = new List<FilterViewModel>
            {
                new FilterViewModel(
                    "PasswordStatus",
                    "Password Status",
                    PasswordStatusOptions
                ),
                new FilterViewModel(
                    "AdminStatus",
                    "Admin Status",
                    AdminStatusOptions
                ),
                new FilterViewModel(
                    "ActiveStatus",
                    "Active Status",
                    ActiveStatusOptions
                ),
                new FilterViewModel(
                    "JobGroupId",
                    "Job Group",
                    GetJobGroupOptions(jobGroups)
                ),
                new FilterViewModel(
                    "RegistrationType",
                    "Registration Type",
                    RegistrationTypeOptions
                )
            };
            filters.AddRange(
                promptsWithOptions.Select(
                    customPrompt => new FilterViewModel(
                        $"CustomPrompt{customPrompt.CustomPromptNumber}",
                        customPrompt.CustomPromptText,
                        GetCustomPromptOptions(customPrompt)
                    )
                )
            );
            return filters;
        }
    }
}
