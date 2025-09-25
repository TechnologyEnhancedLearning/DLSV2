namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/Available/{page=1:int}")]
        public IActionResult Available(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            int page = 1
        )
        {
            TempData["LearningActivity"] = "Available";
            sortBy ??= CourseSortByOptions.Name.PropertyName;

            var availableCourses = courseService.GetAvailableCourses(
                User.GetCandidateIdKnownNotNull(),
                User.GetCentreIdKnownNotNull()
            ).Where(course => !course.HideInLearnerPortal).ToList();
            var bannerText = GetBannerText();
            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(sortBy, sortDirection),
                null,
                new PaginationOptions(page)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                availableCourses,
                searchSortPaginationOptions
            );

            var model = new AvailablePageViewModel(
                result,
                bannerText
            );
            return View("Available/Available", model);
        }

        [NoCaching]
        public IActionResult AllAvailableItems()
        {
            var availableCourses = courseService.GetAvailableCourses(
                User.GetCandidateIdKnownNotNull(),
                User.GetCentreIdKnownNotNull()
            ).Where(course => !course.HideInLearnerPortal).ToList();
            var model = new AllAvailableItemsPageViewModel(availableCourses);
            return View("Available/AllAvailableItems", model);
        }

        public IActionResult EnrolOnSelfAssessment(int selfAssessmentId)
        {
           var selfAssessment = selfAssessmentService.GetSelfAssessmentRetirementDateById(selfAssessmentId);
            if(CheckRetirementDate(selfAssessment.RetirementDate)) return RedirectToAction("ConfirmRetirement", new { selfAssessmentId });
            courseService.EnrolOnSelfAssessment(selfAssessmentId, User.GetUserIdKnownNotNull(), User.GetCentreIdKnownNotNull());
            return RedirectToAction("SelfAssessment", new { selfAssessmentId });
        }

        [Route("/LearningPortal/Retirement/{selfAssessmentId:int}/confirm")]
        public IActionResult ConfirmRetirement(int selfAssessmentId)
        {
            var selfAssessment = selfAssessmentService.GetSelfAssessmentRetirementDateById(selfAssessmentId);
            var model = new RetirementViewModel(selfAssessmentId, selfAssessment.RetirementDate, selfAssessment.Name);
            return View("Available/ConfirmRetirement", model);
        }
        [HttpPost]
        [Route("/LearningPortal/Retirement/{selfAssessmentId:int}/confirm")]
        public IActionResult ConfirmRetirement(RetirementViewModel retirementViewModel)
        {
            if (!ModelState.IsValid && !retirementViewModel.ActionConfirmed)
            {
                var selfAssessment = selfAssessmentService.GetSelfAssessmentRetirementDateById(retirementViewModel.SelfAssessmentId);
                var model = new RetirementViewModel(retirementViewModel.SelfAssessmentId , selfAssessment.RetirementDate, selfAssessment.Name);
                return View("Available/ConfirmRetirement", model);
            }
            var date = selfAssessmentService.GetSelfAssessmentRetirementDateById(retirementViewModel.SelfAssessmentId);
             courseService.EnrolOnSelfAssessment(retirementViewModel.SelfAssessmentId, User.GetUserIdKnownNotNull(), User.GetCentreIdKnownNotNull());
            return RedirectToAction("SelfAssessment", new { retirementViewModel.SelfAssessmentId });
        }
        private  bool CheckRetirementDate(DateTime? date)
        {
            if (date == null)
                return false;

            DateTime twoWeeksbeforeRetirementdate = DateTime.Today.AddDays(14);
            DateTime today = DateTime.Today;
            return (date >= today  && date <= twoWeeksbeforeRetirementdate);
        }
    }
}
