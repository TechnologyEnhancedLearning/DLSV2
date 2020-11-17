using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [Route("Frameworks/Dashboard")]
        public IActionResult FrameworksDashboard()
        {
            var frameworks = frameworkService.GetFrameworksForAdminId(GetAdminID());
            if (frameworks == null)
            {
                logger.LogWarning($"Attempt to display frameworks for admin {GetAdminID()} returned null.");
                return StatusCode(403);
            }
            var model = new DashboardViewModel(frameworks);
            return View("Dashboard/Dashboard", model);
        }
    }
}
