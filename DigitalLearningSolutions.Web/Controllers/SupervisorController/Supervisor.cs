namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
{
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Supervisor;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using DigitalLearningSolutions.Web.Extensions;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    public partial class SupervisorController
    {
        public IActionResult Index()
        {
            var adminId = GetAdminID();
            var dashboardData = supervisorService.GetDashboardDataForAdminId(adminId);
            var model = new SupervisorDashboardViewModel()
            {
                DashboardData = dashboardData
            };
            return View(model);
        }
    }
}
