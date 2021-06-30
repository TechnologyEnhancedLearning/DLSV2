﻿namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
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
        [Route("/Supervisor/Staff/List/{page=1:int}")]
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
        [Route("/Supervisor/Staff/List/{page=1:int}")]
        public IActionResult AddSuperviseDelegate(string delegateEmail, int page = 1)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var supervisorEmail = GetUserEmail();
            AddSupervisorDelegateAndReturnId(adminId, delegateEmail, supervisorEmail, centreId);
            return RedirectToAction("MyStaffList", page);
        }
        [Route("/Supervisor/Staff/AddMultiple")]
        public IActionResult AddMultipleSuperviseDelegates()
        {
            var model = new AddMultipleSupervisorDelegatesViewModel();
            return View("AddMultipleSupervisorDelegates", model);
        }
        [HttpPost]
        [Route("/Supervisor/Staff/AddMultiple")]
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
        [Route("/Supervisor/Staff/{supervisorDelegateId}/Remove")]
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
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessments")]
        public IActionResult DelegateProfileAssessments(int supervisorDelegateId)
        {
            var superviseDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);
            var delegateSelfAssessments = supervisorService.GetSelfAssessmentsForSupervisorDelegateId(supervisorDelegateId, GetAdminID());
            var model = new DelegateSelfAssessmentsViewModel()
            {
                SupervisorDelegateDetail = superviseDelegate,
                DelegateSelfAssessments = delegateSelfAssessments
            };
            return View("DelegateProfileAssessments", model);
        }
    }
}
