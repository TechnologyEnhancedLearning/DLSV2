namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
{
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Supervisor;
    using DigitalLearningSolutions.Web.Helpers;
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
        [Route("/Supervisor/MyStaff/{page=1:int}")]
        public IActionResult MyStaffList(int page = 1)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var centreCustomPrompts = customPromptsService.GetCustomPromptsForCentreByCentreId(centreId);
            var supervisorDelegateDetails = supervisorService.GetSupervisorDelegateDetailsForAdminId(adminId);
            var model = new MyStaffListViewModel()
            {
                CentreCustomPrompts = centreCustomPrompts,
                SuperviseDelegateDetails = supervisorDelegateDetails
            };
            return View("MyStaffList", model);
        }
        [HttpPost]
        [Route("/Supervisor/MyStaff/{page=1:int}")]
        public IActionResult AddSuperviseDelegate(string delegateEmail, int page = 1)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var supervisorEmail = GetUserEmail();
            AddSupervisorDelegateAndReturnId(adminId, delegateEmail, supervisorEmail, centreId);
            return RedirectToAction("MyStaffList", page);
        }
        [Route("/Supervisor/MyStaff/AddMultiple")]
        public IActionResult AddMultipleSuperviseDelegates()
        {
            var model = new AddMultipleSupervisorDelegatesViewModel();
            return View("AddMultipleSupervisorDelegates", model);
        }
        [HttpPost]
        [Route("/Supervisor/MyStaff/AddMultiple")]
        public IActionResult AddMultipleSuperviseDelegates(AddMultipleSupervisorDelegatesViewModel model)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var supervisorEmail = GetUserEmail();
            var delegateEmailsList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(model.DelegateEmails);
            foreach (var delegateEmail in delegateEmailsList)
            {
                if (delegateEmail.Length > 0)
                {
                    if (RegexStringValidationHelper.IsValidEmail(delegateEmail))
                    {
                        AddSupervisorDelegateAndReturnId(adminId, delegateEmail, supervisorEmail, centreId);
                    }
                }
            }
            return RedirectToAction("MyStaffList");
        }
        private void AddSupervisorDelegateAndReturnId(int adminId, string delegateEmail, string supervisorEmail, int centreId)
        {
            var supervisorDelegateId = supervisorService.AddSuperviseDelegate(adminId, delegateEmail, supervisorEmail, centreId);
            if (supervisorDelegateId > 0)
            {
                frameworkNotificationService.SendSupervisorDelegateInvite(supervisorDelegateId);
            }
        }
        public IActionResult ConfirmSupervise(int supervisorDelegateId)
        {
            if (supervisorService.ConfirmSupervisorDelegateById(supervisorDelegateId, 0, GetAdminID()))
            {
                frameworkNotificationService.SendSupervisorDelegateConfirmed(supervisorDelegateId);
            }
            return RedirectToAction("MyStaffList");
        }
        [Route("/Supervisor/MyStaff/Remove/{supervisorDelegateId}")]
        public IActionResult RemoveSupervisorDelegateConfirm(int supervisorDelegateId)
        {
            var superviseDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);
            return View("RemoveConfirm", supervisorDelegateId);
        }
        public IActionResult RemoveSupervisorDelegate(int supervisorDelegateId)
        {
            supervisorService.RemoveSupervisorDelegateById(supervisorDelegateId, 0, GetAdminID());
            return RedirectToAction("MyStaffList");
        }
    }
}
