namespace DigitalLearningSolutions.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("FindYourCentre")]
    public class FindYourCentreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
