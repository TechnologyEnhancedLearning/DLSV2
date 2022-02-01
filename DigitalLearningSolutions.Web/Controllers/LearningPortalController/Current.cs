namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/Current/{page=1:int}")]
        public async Task<IActionResult> Current(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Descending,
            int page = 1
        )
        {
            sortBy ??= CourseSortByOptions.LastAccessed.PropertyName;
            var delegateId = User.GetCandidateIdKnownNotNull();
            var currentCourses = courseDataService.GetCurrentCourses(delegateId);
            var bannerText = GetBannerText();
            var selfAssessments =
                selfAssessmentService.GetSelfAssessmentsForCandidate(delegateId);
            var (learningResources, apiIsAccessible) =
                await GetIncompleteActionPlanResourcesIfSignpostingEnabled(delegateId);
            var model = new CurrentPageViewModel(
                currentCourses,
                searchString,
                sortBy,
                sortDirection,
                selfAssessments,
                learningResources,
                apiIsAccessible,
                bannerText,
                page
            );
            return View("Current/Current", model);
        }

        public async Task<IActionResult> AllCurrentItems()
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            var currentCourses = courseDataService.GetCurrentCourses(delegateId);
            var selfAssessment =
                selfAssessmentService.GetSelfAssessmentsForCandidate(delegateId);
            var (learningResources, _) = await GetIncompleteActionPlanResourcesIfSignpostingEnabled(delegateId);
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
        public IActionResult SetCurrentCourseCompleteByDate(int id)
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

            var courseModel = new CurrentCourseViewModel(course);
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
                courseModel.ProgressId
            );
            return View("Current/SetCompleteByDate", editCompleteByDateViewModel);
        }

        [Route("/LearningPortal/Current/Remove/{id:int}")]
        public IActionResult RemoveCurrentCourseConfirmation(int id)
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

            var model = new CurrentCourseViewModel(course);
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
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/MarkAsComplete")]
        public async Task<IActionResult> MarkActionPlanResourceAsComplete(int learningLogItemId)
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
                apiIsAccessible
            );
            return View("Current/MarkActionPlanResourceAsComplete", model);
        }

        [HttpPost]
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
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/CompleteBy")]
        public async Task<IActionResult> SetCurrentActionPlanResourceCompleteByDate(int learningLogItemId)
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
                apiIsAccessible: apiIsAccessible
            );

            return View("Current/SetCompleteByDate", model);
        }

        [HttpPost]
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
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/Remove")]
        public async Task<IActionResult> RemoveResourceFromActionPlan(int learningLogItemId)
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
                apiIsAccessible
            );
            return View("Current/RemoveCurrentActionPlanResourceConfirmation", model);
        }

        [HttpPost]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/Remove")]
        public IActionResult RemoveResourceFromActionPlanPost(int learningLogItemId)
        {
            actionPlanService.RemoveActionPlanResource(learningLogItemId, User.GetCandidateIdKnownNotNull());
            return RedirectToAction("Current");
        }

        private async Task<(IList<ActionPlanResource>, bool apiIsAccessible)>
            GetIncompleteActionPlanResourcesIfSignpostingEnabled(
                int delegateId
            )
        {
            if (!config.IsSignpostingUsed())
            {
                return (new List<ActionPlanResource>(), false);
            }

            var (resources, apiIsAccessible) =
                await actionPlanService.GetIncompleteActionPlanResources(delegateId);
            return (resources.ToList(), apiIsAccessible);
        }
    }
}
