namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.ViewModels.LearningContent;
    using Microsoft.AspNetCore.Mvc;

    [DelegateOnlyInaccessible]
    public class LearningContentController : Controller
    {
        private const string ItSkillsPathwayBrand = "ITSkillsPathway";
        private const string ReasonableAdjustmentFlagBrand = "ReasonableAdjustmentFlag";
        public const string TerminologyAndClassificationsDeliveryServiceBrand = "TerminologyandClassificationsDeliveryService";
        
        public IActionResult ItSkillsPathway()
        {
            var model = new LearningContentViewModel(ItSkillsPathwayBrand);
            return View("Index", model);
        }
        
        public IActionResult ReasonableAdjustmentFlag()
        {
            var model = new LearningContentViewModel(ReasonableAdjustmentFlagBrand);
            return View("Index", model);
        }
        
        public IActionResult TerminologyAndClassificationsDeliveryService()
        {
            var model = new LearningContentViewModel(TerminologyAndClassificationsDeliveryServiceBrand);
            return View("Index", model);
        }
    }
}
