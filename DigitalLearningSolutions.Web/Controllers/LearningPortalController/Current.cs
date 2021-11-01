﻿namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/Current/{page=1:int}")]
        public IActionResult Current(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Descending,
            int page = 1
        )
        {
            sortBy ??= CourseSortByOptions.LastAccessed.PropertyName;

            var currentCourses = courseDataService.GetCurrentCourses(User.GetCandidateIdKnownNotNull());
            var bannerText = GetBannerText();
            var selfAssessments =
                selfAssessmentService.GetSelfAssessmentsForCandidate(User.GetCandidateIdKnownNotNull());
            var model = new CurrentPageViewModel(
                currentCourses,
                searchString,
                sortBy,
                sortDirection,
                selfAssessments,
                bannerText,
                page
            );
            return View("Current/Current", model);
        }

        public IActionResult AllCurrentItems()
        {
            var currentCourses = courseDataService.GetCurrentCourses(User.GetCandidateIdKnownNotNull());
            var selfAssessment =
                selfAssessmentService.GetSelfAssessmentsForCandidate(User.GetCandidateIdKnownNotNull());
            var model = new AllCurrentItemsPageViewModel(currentCourses, selfAssessment);
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

            var validationResult = DateValidator.ValidateDate(day, month, year, "Complete by date");
            if (!validationResult.IsValid)
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

            model.CompleteByValidationResult = DateValidator.ValidateDate(day, month, year, "Complete by date");
            
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
            courseDataService.RemoveCurrentCourse(progressId, User.GetCandidateIdKnownNotNull());
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
    }
}
