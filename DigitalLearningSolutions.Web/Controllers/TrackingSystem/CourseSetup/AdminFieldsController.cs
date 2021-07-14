namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
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
            var courseCustomPrompts = customPromptsService.GetCustomPromptsForCourse(customisationId, centreId, categoryId.Value);

            if (courseCustomPrompts == null)
            {
                return NotFound();
            }

            var model = new AdminFieldsViewModel(courseCustomPrompts);
            return View(model);
        }
    }
}
