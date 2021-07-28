namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CourseSetup")]
    public class AdminFieldsController : Controller
    {
        private readonly ICustomPromptsService customPromptsService;

        public AdminFieldsController(ICustomPromptsService customPromptsService)
        {
            this.customPromptsService = customPromptsService;
        }

        [HttpGet]
        [Route("{customisationId}/AdminFields")]
        public IActionResult AdminFields(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;
            var courseCustomPrompts = customPromptsService.GetCustomPromptsForCourse(
                customisationId,
                centreId,
                categoryId.Value
            );

            if (courseCustomPrompts == null)
            {
                return NotFound();
            }

            var model = new AdminFieldsViewModel(courseCustomPrompts.CourseAdminFields, customisationId);
            return View(model);
        }

        [HttpGet]
        [Route("{customisationId}/AdminFields/Edit")]
        public IActionResult EditAdminField(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;
            var courseCustomPrompts = customPromptsService.GetCustomPromptsForCourse(
                customisationId,
                centreId,
                categoryId.Value
            );

            var data = TempData.Get<EditAdminFieldData>();

            var model = data != null
              ? data.EditModel!
              : new EditAdminFieldViewModel();

            return View(model);
        }
  }
}
