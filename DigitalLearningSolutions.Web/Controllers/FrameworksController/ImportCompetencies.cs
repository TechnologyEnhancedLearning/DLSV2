using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using DigitalLearningSolutions.Web.Extensions;
using DigitalLearningSolutions.Data.Models.SessionData.Frameworks;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [Route("/Framework/{frameworkId}/Structure/Import")]
        public IActionResult ImportCompetencies(int frameworkId)
        {
            
            return View("Developer/ImportCompetencies");
        }
    }
}
