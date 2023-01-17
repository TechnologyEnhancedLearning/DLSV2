namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed;
    using Microsoft.AspNetCore.Mvc;

    public partial class LearningPortalController
    {
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [Route("/LearningPortal/Completed/{page=1:int}")]
        public async Task<IActionResult> Completed(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Descending,
            int page = 1
        )
        {
            sortBy ??= CourseSortByOptions.CompletedDate.PropertyName;
            var delegateId = User.GetCandidateIdKnownNotNull();
            var delegateUserId = User.GetUserIdKnownNotNull();
            var completedCourses = courseDataService.GetCompletedCourses(delegateId);
            var (completedLearningResources, apiIsAccessible) =
                await GetCompletedLearningResourcesIfSignpostingEnabled(delegateUserId);
            var bannerText = GetBannerText();

            var allItems = completedCourses.Cast<CompletedLearningItem>().ToList();
            allItems.AddRange(completedLearningResources);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(sortBy, sortDirection),
                null,
                new PaginationOptions(page)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                allItems,
                searchSortPaginationOptions
            );

            var model = new CompletedPageViewModel(
                result,
                apiIsAccessible,
                config,
                bannerText
            );
            return View("Completed/Completed", model);
        }

        public async Task<IActionResult> AllCompletedItems()
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            var delegateUserId = User.GetUserIdKnownNotNull();
            var completedCourses = courseDataService.GetCompletedCourses(delegateId);
            var (completedLearningResources, _) = await GetCompletedLearningResourcesIfSignpostingEnabled(delegateUserId);
            var model = new AllCompletedItemsPageViewModel(completedCourses, completedLearningResources, config);
            return View("Completed/AllCompletedItems", model);
        }

        private async Task<(IList<CompletedActionPlanResource>, bool apiIsAccessible)>
            GetCompletedLearningResourcesIfSignpostingEnabled(int delegateUserId)
        {
            if (!config.IsSignpostingUsed())
            {
                return (new List<CompletedActionPlanResource>(), false);
            }

            var (resources, apiIsAccessible) =
                await actionPlanService.GetCompletedActionPlanResources(delegateUserId);
            return (resources.Select(r => new CompletedActionPlanResource(r)).ToList(), apiIsAccessible);
        }
    }
}
