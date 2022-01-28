namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed;
    using Microsoft.AspNetCore.Mvc;

    public partial class LearningPortalController
    {
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [Route("/LearningPortal/Completed/{page=1:int}")]
        public async Task<IActionResult> Completed(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Descending,
            int page = 1
        )
        {
            sortBy ??= CourseSortByOptions.CompletedDate.PropertyName;
            var delegateId = User.GetCandidateIdKnownNotNull();
            var completedCourses = courseDataService.GetCompletedCourses(delegateId);
            var (completedLearningResources, sourcedFromFallbackData) =
                await GetCompletedLearningResourcesIfSignpostingEnabled(delegateId);
            var bannerText = GetBannerText();
            var model = new CompletedPageViewModel(
                completedCourses,
                completedLearningResources,
                sourcedFromFallbackData,
                config,
                searchString,
                sortBy,
                sortDirection,
                bannerText,
                page
            );
            return View("Completed/Completed", model);
        }

        public async Task<IActionResult> AllCompletedItems()
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            var completedCourses = courseDataService.GetCompletedCourses(delegateId);
            var (completedLearningResources, _) = await GetCompletedLearningResourcesIfSignpostingEnabled(delegateId);
            var model = new AllCompletedItemsPageViewModel(completedCourses, completedLearningResources, config);
            return View("Completed/AllCompletedItems", model);
        }

        private async Task<(IEnumerable<CompletedActionPlanResource>, bool sourcedFromFallbackData)>
            GetCompletedLearningResourcesIfSignpostingEnabled(int delegateId)
        {
            if (!config.IsSignpostingUsed())
            {
                return (new List<CompletedActionPlanResource>(), false);
            }

            var (resources, sourcedFromFallbackData) =
                await actionPlanService.GetCompletedActionPlanResources(delegateId);
            return (resources.Select(r => new CompletedActionPlanResource(r)), sourcedFromFallbackData);
        }
    }
}
