﻿namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using Microsoft.AspNetCore.Mvc;
    using DigitalLearningSolutions.Web.Helpers;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/Available/{page=1:int}")]
        public IActionResult Available(
            string? searchString = null,
            string sortBy = SortByOptionTexts.Name,
            string sortDirection = BaseCoursePageViewModel.AscendingText,
            int page = 1
        )
        {
            var availableCourses = courseService.GetAvailableCourses(User.GetCandidateId(), User.GetCentreId());
            var bannerText = GetBannerText();
            var model = new AvailablePageViewModel(
                availableCourses,
                config,
                searchString,
                sortBy,
                sortDirection,
                bannerText,
                page
            );
            return View("Available/Available", model);
        }

        public IActionResult AllAvailableItems()
        {
            var availableCourses = courseService.GetAvailableCourses(User.GetCandidateId(), User.GetCentreId());
            var model = new AllAvailableItemsPageViewModel(availableCourses, config);
            return View("Available/AllAvailableItems", model);
        }

        public IActionResult EnrolOnSelfAssessment(int selfAssessmentId)
        {
            courseService.EnrolOnSelfAssessment(selfAssessmentId, User.GetCandidateId());
            return RedirectToAction("SelfAssessment", new { selfAssessmentId = selfAssessmentId});
        }
    }
}
