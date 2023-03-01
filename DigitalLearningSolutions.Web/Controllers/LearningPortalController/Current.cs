namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.FeatureManagement.Mvc;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/Current/{page=1:int}")]
        public async Task<IActionResult> Current(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Descending,
            int page = 1
        )
        {
            TempData["LearningActivity"] = "Current";
            sortBy ??= CourseSortByOptions.LastAccessed.PropertyName;
            var delegateId = User.GetCandidateIdKnownNotNull();
            var delegateUserId = User.GetUserIdKnownNotNull();
            var currentCourses = courseDataService.GetCurrentCourses(delegateId);
            var bannerText = GetBannerText();

            var centreId = User.GetCentreIdKnownNotNull();
            var selfAssessments =
                selfAssessmentService.GetSelfAssessmentsForCandidate(delegateUserId, centreId);

            var (learningResources, apiIsAccessible) =
                await GetIncompleteActionPlanResourcesIfSignpostingEnabled(delegateUserId);

            var allItems = currentCourses.Cast<CurrentLearningItem>().ToList();
            allItems.AddRange(selfAssessments);
            allItems.AddRange(learningResources);

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

            var model = new CurrentPageViewModel(
                result,
                apiIsAccessible,
                bannerText
            );
            return View("Current/Current", model);
        }

        public async Task<IActionResult> AllCurrentItems()
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            var delegateUserId = User.GetUserIdKnownNotNull();
            var currentCourses = courseDataService.GetCurrentCourses(delegateId);
            var centreId = User.GetCentreIdKnownNotNull();

            var selfAssessment =
                selfAssessmentService.GetSelfAssessmentsForCandidate(delegateUserId, centreId);

            var (learningResources, _) = await GetIncompleteActionPlanResourcesIfSignpostingEnabled(delegateUserId);
            var model = new AllCurrentItemsPageViewModel(currentCourses, selfAssessment, learningResources);
            return View("Current/AllCurrentItems", model);
        }

        [HttpPost]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCurrentCourseCompleteByDate(
            int id,
            int progressId,
            EditCompleteByDateFormData formData
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new EditCompleteByDateViewModel(formData, id, progressId);
                return View("Current/SetCompleteByDate", model);
            }

            var completeByDate = DateValidator.IsDateNull(formData.Day, formData.Month, formData.Year)
                ? (DateTime?)null
                : new DateTime(formData.Year!.Value, formData.Month!.Value, formData.Day!.Value);

            courseDataService.SetCompleteByDate(progressId, User.GetCandidateIdKnownNotNull(), completeByDate);
            return RedirectToAction("Current");
        }

        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCurrentCourseCompleteByDate(int id, ReturnPageQuery returnPageQuery)
        {
            var currentCourses = courseDataService.GetCurrentCourses(User.GetCandidateIdKnownNotNull());
            var course = currentCourses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                logger.LogWarning(
                    $"Attempt to set complete by date for course with id {id} which is not a current course for user with id {User.GetCandidateIdKnownNotNull()}"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var courseModel = new CurrentCourseViewModel(course, returnPageQuery);
            if (courseModel.CompleteByDate != null && !courseModel.SelfEnrolled)
            {
                logger.LogWarning(
                    $"Attempt to set complete by date for course with id {id} for user with id ${User.GetCandidateIdKnownNotNull()} " +
                    "but the complete by date has already been set and the user has not self enrolled"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var editCompleteByDateViewModel = new EditCompleteByDateViewModel(
                id,
                course.Name,
                LearningItemType.Course,
                courseModel.CompleteByDate,
                returnPageQuery,
                courseModel.ProgressId
            );
            return View("Current/SetCompleteByDate", editCompleteByDateViewModel);
        }

        [Route("/LearningPortal/Current/Remove/{id:int}")]
        public IActionResult RemoveCurrentCourseConfirmation(int id, ReturnPageQuery returnPageQuery)
        {
            var currentCourses = courseDataService.GetCurrentCourses(User.GetCandidateIdKnownNotNull());
            var course = currentCourses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                logger.LogWarning(
                    $"Attempt to remove course with id {id} which is not a current course for user with id {User.GetCandidateIdKnownNotNull()}"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var model = new CurrentCourseViewModel(course, returnPageQuery);
            return View("Current/RemoveCurrentCourseConfirmation", model);
        }

        [Route("/LearningPortal/Current/Remove/{progressId:int}")]
        [HttpPost]
        public IActionResult RemoveCurrentCourse(int progressId)
        {
            courseDataService.RemoveCurrentCourse(
                progressId,
                User.GetCandidateIdKnownNotNull(),
                RemovalMethod.RemovedByDelegate
            );
            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/Current/RequestUnlock/{progressId:int}")]
        public IActionResult RequestUnlock(int progressId)
        {
            var currentCourses = courseDataService.GetCurrentCourses(User.GetCandidateIdKnownNotNull());
            var course = currentCourses.FirstOrDefault(c => c.ProgressID == progressId && c.PLLocked);
            if (course == null)
            {
                logger.LogWarning(
                    $"Attempt to unlock course with progress id {progressId} however found no course with that progress id " +
                    $"and PLLocked for user with id {User.GetCandidateIdKnownNotNull()}"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            notificationService.SendUnlockRequest(progressId);
            return View("Current/UnlockCurrentCourse");
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/MarkAsComplete")]
        public async Task<IActionResult> MarkActionPlanResourceAsComplete(int learningLogItemId, ReturnPageQuery returnPageQuery)
        {
            var (actionPlanResource, apiIsAccessible) =
                await actionPlanService.GetActionPlanResource(learningLogItemId);

            if (actionPlanResource == null)
            {
                return NotFound();
            }

            var model = new MarkActionPlanResourceAsCompleteViewModel(
                learningLogItemId,
                actionPlanResource.AbsentInLearningHub,
                actionPlanResource!.Name,
                apiIsAccessible,
                returnPageQuery
            );
            return View("Current/MarkActionPlanResourceAsComplete", model);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/MarkAsComplete")]
        public IActionResult MarkActionPlanResourceAsComplete(
            int learningLogItemId,
            MarkActionPlanResourceAsCompleteFormData formData
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new MarkActionPlanResourceAsCompleteViewModel(formData, learningLogItemId);
                return View("Current/MarkActionPlanResourceAsComplete", model);
            }

            var completionDate = new DateTime(formData.Year!.Value, formData.Month!.Value, formData.Day!.Value);

            actionPlanService.SetCompletionDate(learningLogItemId, completionDate);
            return RedirectToAction("Current");
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/CompleteBy")]
        public async Task<IActionResult> SetCurrentActionPlanResourceCompleteByDate(int learningLogItemId, ReturnPageQuery returnPageQuery)
        {
            var (actionPlanResource, apiIsAccessible) =
                await actionPlanService.GetActionPlanResource(learningLogItemId);

            if (actionPlanResource == null || actionPlanResource.AbsentInLearningHub)
            {
                return NotFound();
            }

            var model = new EditCompleteByDateViewModel(
                learningLogItemId,
                actionPlanResource!.Name,
                LearningItemType.Resource,
                actionPlanResource.CompleteByDate,
                returnPageQuery,
                apiIsAccessible: apiIsAccessible
            );

            return View("Current/SetCompleteByDate", model);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/CompleteBy")]
        public IActionResult SetCurrentActionPlanResourceCompleteByDate(
            int learningLogItemId,
            EditCompleteByDateFormData formData
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new EditCompleteByDateViewModel(formData, learningLogItemId);
                return View("Current/SetCompleteByDate", model);
            }

            var completeByDate = DateValidator.IsDateNull(formData.Day, formData.Month, formData.Year)
                ? (DateTime?)null
                : new DateTime(formData.Year!.Value, formData.Month!.Value, formData.Day!.Value);

            actionPlanService.SetCompleteByDate(learningLogItemId, completeByDate);
            return RedirectToAction("Current");
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/Remove")]
        public async Task<IActionResult> RemoveResourceFromActionPlan(int learningLogItemId, ReturnPageQuery returnPageQuery)
        {
            var (actionPlanResource, apiIsAccessible) =
                await actionPlanService.GetActionPlanResource(learningLogItemId);

            if (actionPlanResource == null)
            {
                return NotFound();
            }

            var model = new RemoveActionPlanResourceViewModel(
                actionPlanResource!.Id,
                actionPlanResource.Name,
                actionPlanResource.AbsentInLearningHub,
                apiIsAccessible,
                returnPageQuery
            );
            return View("Current/RemoveCurrentActionPlanResourceConfirmation", model);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/Remove")]
        public IActionResult RemoveResourceFromActionPlanPost(int learningLogItemId)
        {
            actionPlanService.RemoveActionPlanResource(learningLogItemId, User.GetCandidateIdKnownNotNull());
            return RedirectToAction("Current");
        }

        private async Task<(IList<ActionPlanResource>, bool apiIsAccessible)>
            GetIncompleteActionPlanResourcesIfSignpostingEnabled(
                int delegateUserId
            )
        {
            if (!config.IsSignpostingUsed())
            {
                return (new List<ActionPlanResource>(), false);
            }

            var (resources, apiIsAccessible) =
                await actionPlanService.GetIncompleteActionPlanResources(delegateUserId);
            return (resources.ToList(), apiIsAccessible);
        }
    }
}
