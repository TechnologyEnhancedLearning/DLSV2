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
        public IActionResult SetCurrentCourseCompleteByDate(
            int id,
            int progressId,
            EditCompleteByDateFormData formData
        )
        {
            if (progressId == 0)
            {
                return new StatusCodeResult(500);
            }

            if (IsDateBlank(formData.Day, formData.Month, formData.Year))
            {
                courseDataService.SetCompleteByDate(progressId, User.GetCandidateIdKnownNotNull(), null);
                return RedirectToAction("Current");
            }

            if (!ModelState.IsValid)
            {
                var model = new EditCompleteByDateViewModel(formData, id);
                return View("Current/SetCompleteByDate", model);
            }

            var completeByDate = new DateTime(formData.Year!.Value, formData.Month!.Value, formData.Day!.Value);
            courseDataService.SetCompleteByDate(progressId, User.GetCandidateIdKnownNotNull(), completeByDate);
            return RedirectToAction("Current");
        }

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
                progressId: courseModel.ProgressId
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
            if (IsDateBlank(formData.Day, formData.Month, formData.Year))
            {
                actionPlanService.SetCompletionDate(learningLogItemId, null);
                return RedirectToAction("Current");
            }

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
        [Route("/LearningPortal/Current/Resource/CompleteBy/{learningLogItemId:int}")]
        public IActionResult SetCurrentActionPlanItemCompleteByDate(int learningLogItemId)
        {
            var learningLogItem =
                actionPlanService.SelectLearningLogItemById(learningLogItemId)!;

            var (day, month, year) = GetDayMonthAndYear(learningLogItem.DueDate);

            var model = new EditCompleteByDateViewModel(
                learningLogItemId,
                learningLogItem.Activity!,
                LearningItemType.Resource,
                day,
                month,
                year
            );

            return View("Current/SetCompleteByDate", model);
        }

        [HttpPost]
        [Route("/LearningPortal/Current/Resource/CompleteBy/{learningLogItemId:int}")]
        public IActionResult SetCurrentActionPlanItemCompleteByDate(
            int learningLogItemId,
            EditCompleteByDateFormData formData
        )
        {
            if (IsDateBlank(formData.Day, formData.Month, formData.Year))
            {
                actionPlanService.SetCompleteByDate(learningLogItemId, null);
                return RedirectToAction("Current");
            }

            if (!ModelState.IsValid)
            {
                var model = new EditCompleteByDateViewModel(formData, learningLogItemId);
                return View("Current/SetCompleteByDate", model);
            }

            var completionDate = new DateTime(formData.Year!.Value, formData.Month!.Value, formData.Day!.Value);

            actionPlanService.SetCompleteByDate(learningLogItemId, completionDate);
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

        public bool IsDateBlank(int? day, int? month, int? year)
        {
            return (day ?? 0) == 0 && (month ?? 0) == 0 && (year ?? 0) == 0;
        }

        public (int?, int?, int?) GetDayMonthAndYear(
            DateTime? date
        )
        {
            if (date == null)
            {
                return (null, null, null);
            }

            return (date.Value.Day, date.Value.Month, date.Value.Year);
        }
    }
}
