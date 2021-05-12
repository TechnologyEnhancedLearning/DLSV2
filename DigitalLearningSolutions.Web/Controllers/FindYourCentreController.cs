namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using Microsoft.AspNetCore.Mvc;

    public class FindYourCentreController : Controller
    {
        public IActionResult Index(string centreId)
        {
            var model = new FindYourCentreViewModel(centreId);
            return View(model);
        }
    }
}
