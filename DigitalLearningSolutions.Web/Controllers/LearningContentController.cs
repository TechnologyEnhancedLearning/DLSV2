namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.ViewModels.LearningContent;
    using Microsoft.AspNetCore.Mvc;

    [RedirectDelegateOnlyToLearningPortal]
    public class LearningContentController : Controller
    {
        private const string ItSkillsPathwayBrand = "ITSkillsPathway";
        private const string ItSkillsPathwayTitle = "IT skills pathway";
        private const string ReasonableAdjustmentFlagBrand = "ReasonableAdjustmentFlag";
        private const string ReasonableAdjustmentFlagTitle = "Reasonable adjustment flag";

        private const string TerminologyAndClassificationsDeliveryServiceBrand =
            "TerminologyandClassificationsDeliveryService";

        private const string TerminologyAndClassificationsDeliveryServiceTitle =
            "Terminology and classifications delivery service";

        public IActionResult ItSkillsPathway()
        {
            var model = new LearningContentViewModel(ItSkillsPathwayBrand, ItSkillsPathwayTitle);
            return View("Index", model);
        }

        public IActionResult ReasonableAdjustmentFlag()
        {
            var model = new LearningContentViewModel(ReasonableAdjustmentFlagBrand, ReasonableAdjustmentFlagTitle);
            return View("Index", model);
        }

        public IActionResult TerminologyAndClassificationsDeliveryService()
        {
            var model = new LearningContentViewModel(
                TerminologyAndClassificationsDeliveryServiceBrand,
                TerminologyAndClassificationsDeliveryServiceTitle,
                titleIsLong: true
            );
            return View("Index", model);
        }
    }
}
