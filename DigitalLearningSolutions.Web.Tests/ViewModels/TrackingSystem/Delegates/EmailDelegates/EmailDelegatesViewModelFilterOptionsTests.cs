namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates;
    using FluentAssertions;
    using NUnit.Framework;

    public class EmailDelegatesViewModelFilterOptionsTests
    {
        [Test]
        public void GetEmailDelegatesFilterModels_should_return_expected_filters()
        {
            // Given
            var (jobGroups, jobGroupsFilter) = GetSampleJobGroupsAndFilter();
            var (customPrompts, promptFilters) = GetSampleCustomPromptsAndFilters();

            // When
            var result = EmailDelegatesViewModelFilterOptions.GetEmailDelegatesFilterModels(
                jobGroups,
                customPrompts
            );

            // Then
            var expectedFilters = promptFilters.Prepend(jobGroupsFilter);
            result.Should().BeEquivalentTo(expectedFilters);
        }

        private static (IEnumerable<(int id, string name)> jobGroups, FilterModel filter) GetSampleJobGroupsAndFilter()
        {
            var jobGroups = new List<(int id, string name)> { (1, "J 1"), (2, "J 2") };

            var jobGroupOptions = new[]
            {
                new FilterOptionModel(
                    "J 1",
                    "JobGroupId" + FilteringHelper.Separator +
                    "JobGroupId" + FilteringHelper.Separator + 1,
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "J 2",
                    "JobGroupId" + FilteringHelper.Separator +
                    "JobGroupId" + FilteringHelper.Separator + 2,
                    FilterStatus.Default
                ),
            };
            var jobGroupFilter = new FilterModel("JobGroupId", "Job Group", jobGroupOptions);

            return (jobGroups, jobGroupFilter);
        }

        private static (List<CentreRegistrationPrompt> customPrompts, List<FilterModel> filters) GetSampleCustomPromptsAndFilters()
        {
            var customPrompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(
                1,
                "First prompt",
                "Clinical\r\nNon-Clinical"
            );
            var customPrompt3 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(3);
            var customPrompt4 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(4, "Fourth prompt", "C 1\r\nC 2\r\nC 3");
            var customPrompts = new List<CentreRegistrationPrompt> { customPrompt1, customPrompt3, customPrompt4 };

            var customPrompt1Options = new[]
            {
                new FilterOptionModel(
                    "Clinical",
                    "Answer1(First prompt)" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "Clinical",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "Non-Clinical",
                    "Answer1(First prompt)" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "Non-Clinical",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "No option selected",
                    "Answer1(First prompt)" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + FilteringHelper.EmptyValue,
                    FilterStatus.Default
                ),
            };
            var customPrompt4Options = new[]
            {
                new FilterOptionModel(
                    "C 1",
                    "Answer4(Fourth prompt)" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 1",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "C 2",
                    "Answer4(Fourth prompt)" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 2",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "C 3",
                    "Answer4(Fourth prompt)" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 3",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "No option selected",
                    "Answer4(Fourth prompt)" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + FilteringHelper.EmptyValue,
                    FilterStatus.Default
                ),
            };
            var customPromptFilters = new List<FilterModel>
            {
                new FilterModel("CentreRegistrationPrompt1", "First prompt", customPrompt1Options),
                new FilterModel("CentreRegistrationPrompt4", "Fourth prompt", customPrompt4Options),
            };

            return (customPrompts, customPromptFilters);
        }
    }
}
