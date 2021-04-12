namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.ViewModels.LearningContent;
    using Microsoft.AspNetCore.Mvc;

    [Route("LearningContent")]
    public class LearningContentController : Controller
    {
        private const string ItSkillsPathwayBrand = "ITSkillsPathway";
        private const string ReasonableAdjustmentFlagBrand = "ReasonableAdjustmentFlag";
        public const string TerminologyAndClassificationsDeliveryServiceBrand = "TerminologyandClassificationsDeliveryService";

        [Route("ITSkillsPathway")]
        public IActionResult ItSkillsPathway()
        {
            var model = new LearningContentViewModel(ItSkillsPathwayBrand);
            return View("Index", model);
        }

        [Route("ReasonableAdjustmentFlag")]
        public IActionResult ReasonableAdjustmentFlag()
        {
            var model = new LearningContentViewModel(ReasonableAdjustmentFlagBrand);
            return View("Index", model);
        }

        [Route("TerminologyandClassificationsDeliveryService")]
        public IActionResult TerminologyAndClassificationsDeliveryService()
        {
            var model = new LearningContentViewModel(TerminologyAndClassificationsDeliveryServiceBrand);
            return View("Index", model);
        }
    }
}
