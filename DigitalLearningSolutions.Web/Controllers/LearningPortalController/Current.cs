namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Models.Enums;
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
            var learningResources = await GetIncompleteActionPlanResourcesIfSignpostingEnabled(delegateId);
            var model = new CurrentPageViewModel(
                currentCourses,
                searchString,
                sortBy,
                sortDirection,
                selfAssessments,
                learningResources,
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
            var learningResources = await GetIncompleteActionPlanResourcesIfSignpostingEnabled(delegateId);
            var model = new AllCurrentItemsPageViewModel(currentCourses, selfAssessment, learningResources);
            return View("Current/AllCurrentItems", model);
        }

        [HttpPost]
        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCurrentCourseCompleteByDate(int id, int day, int month, int year, int progressId)
        {
            if (day == 0 && month == 0 && year == 0)
            {
                courseDataService.SetCompleteByDate(progressId, User.GetCandidateIdKnownNotNull(), null);
                return RedirectToAction("Current");
            }

            var validationResult = OldDateValidator.ValidateDate(day, month, year);
            if (!validationResult.DateValid)
            {
                return RedirectToAction("SetCurrentCourseCompleteByDate", new { id, day, month, year });
            }

            var completeByDate = new DateTime(year, month, day);
            courseDataService.SetCompleteByDate(progressId, User.GetCandidateIdKnownNotNull(), completeByDate);
            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCurrentCourseCompleteByDate(int id, int? day, int? month, int? year)
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

            var model = new CurrentCourseViewModel(course);
            if (model.CompleteByDate != null && !model.SelfEnrolled)
            {
                logger.LogWarning(
                    $"Attempt to set complete by date for course with id {id} for user with id ${User.GetCandidateIdKnownNotNull()} " +
                    "but the complete by date has already been set and the user has not self enrolled"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            if (day != null && month != null && year != null)
            {
                model.CompleteByValidationResult = OldDateValidator.ValidateDate(day.Value, month.Value, year.Value);
            }

            return View("Current/SetCompleteByDate", model);
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

        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/LaunchLearningResource/{learningLogItemId}")]
        public async Task<IActionResult> LaunchLearningResource(int learningLogItemId)
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            var learningResourceLink =
                await actionPlanService.GetLearningResourceLinkAndUpdateLastAccessedDate(learningLogItemId, delegateId);

            if (string.IsNullOrWhiteSpace(learningResourceLink))
            {
                return NotFound();
            }

            // TODO: HEEDLS-678 redirect user to new LH forwarding endpoint.
            return Redirect(learningResourceLink);
        }

        [HttpGet]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/MarkAsComplete")]
        public async Task<IActionResult> MarkActionPlanResourceAsComplete(int learningLogItemId)
        {
            var actionPlanResource = await actionPlanService.GetActionPlanResource(learningLogItemId);
            var model = new MarkActionPlanResourceAsCompleteViewModel(learningLogItemId, actionPlanResource!.Name);
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

        [Route("/LearningPortal/Current/MarkAsComplete/{id:int}")]
        public IActionResult MarkCurrentCourseAsComplete(int id, int? day, int? month, int? year)
        {
            var currentCourses = courseDataService.GetCurrentCourses(User.GetCandidateIdKnownNotNull());
            var course = currentCourses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                logger.LogWarning(
                    $"Attempt to set completed date for course with id {id} which is not a current course for user with id {User.GetCandidateIdKnownNotNull()}"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var model = new CurrentCourseViewModel(course);

            if (day != null && month != null && year != null)
            {
                model.CompleteByValidationResult = OldDateValidator.ValidateDate(day.Value, month.Value, year.Value);
            }

            return View("Current/MarkAsComplete", model);
        }

        [HttpPost]
        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult MarkCurrentCourseAsComplete(int id, int day, int month, int year, int progressId)
        {
            if (day == 0 && month == 0 && year == 0)
            {
                learningLogItemsDataService.SetCompletedDate(progressId, User.GetCandidateIdKnownNotNull(), null);
                return RedirectToAction("Current");
            }

            var validationResult = OldDateValidator.ValidateDate(day, month, year);
            if (!validationResult.DateValid)
            {
                return RedirectToAction("MarkCurrentCourseAsComplete", new { id, day, month, year });
            }

            var completeByDate = new DateTime(year, month, day);
            courseDataService.SetCompleteByDate(progressId, User.GetCandidateIdKnownNotNull(), completeByDate);
            return RedirectToAction("Current");
        }

        [HttpGet]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/Remove")]
        public async Task<IActionResult> RemoveResourceFromActionPlan(int learningLogItemId)
        {
            var actionPlanResource = await actionPlanService.GetActionPlanResource(learningLogItemId);
            var model = new RemoveActionPlanResourceViewModel(actionPlanResource!.Id, actionPlanResource.Name);
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

        private async Task<IEnumerable<ActionPlanResource>> GetIncompleteActionPlanResourcesIfSignpostingEnabled(
            int delegateId
        )
        {
            return config.IsSignpostingUsed()
                ? await actionPlanService.GetIncompleteActionPlanResources(delegateId)
                : new List<ActionPlanResource>();
        }
    }
}
