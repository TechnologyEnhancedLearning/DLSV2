﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/CourseDelegates")]
    public class CourseDelegatesController : Controller
    {
        private readonly ICourseDelegatesService courseDelegatesService;

        public CourseDelegatesController(
            ICourseDelegatesService courseDelegatesService
        )
        {
            this.courseDelegatesService = courseDelegatesService;
        }

        public IActionResult Index(int? customisationId = null)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!.Value;
            var courseDelegatesData =
                courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(centreId, categoryId, customisationId);

            var model = new CourseDelegatesViewModel(courseDelegatesData);

            return View(model);
        }
    }
}
