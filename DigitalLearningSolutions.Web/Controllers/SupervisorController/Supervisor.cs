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
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Data.Models.SessionData.Supervisor;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public partial class SupervisorController
    {
        private const string CookieName = "DLSSupervisorService";
        public IActionResult Index()
        {
            var adminId = GetAdminID();
            var dashboardData = supervisorService.GetDashboardDataForAdminId(adminId);
            var supervisorDashboardToDoItems = supervisorService.GetSupervisorDashboardToDoItems(adminId);
            var model = new SupervisorDashboardViewModel()
            {
                DashboardData = dashboardData,
                SupervisorDashboardToDoItems = supervisorDashboardToDoItems
            };
            return View(model);
        }
        [Route("/Supervisor/Staff/List/{page=1:int}")]
        public IActionResult MyStaffList(string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Ascending,
            int page = 1)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var centreCustomPrompts = centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(centreId);
            var supervisorDelegateDetails = supervisorService.GetSupervisorDelegateDetailsForAdminId(adminId);
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            var model = new MyStaffListViewModel(supervisorDelegateDetails, centreCustomPrompts, searchString, sortBy, sortDirection, page);
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
            var supervisorDelegateId = supervisorService.AddSuperviseDelegate(adminId, null, delegateEmail, supervisorEmail, centreId);
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
            return View("RemoveConfirm", superviseDelegate);
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
        [Route("/Supervisor/AllStaffList")]
        public IActionResult AllStaffList()
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var centreCustomPrompts = centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(centreId);
            var supervisorDelegateDetails = supervisorService.GetSupervisorDelegateDetailsForAdminId(adminId);
            var model = new AllStaffListViewModel(supervisorDelegateDetails, centreCustomPrompts);
            return View("AllStaffList", model);
        }
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/Review")]
        public IActionResult ReviewDelegateSelfAssessment(int supervisorDelegateId, int candidateAssessmentId)
        {
            var adminId = GetAdminID();
            var superviseDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);
            var reviewedCompetencies = PopulateCompetencyLevelDescriptors(selfAssessmentService.GetCandidateAssessmentResultsById(candidateAssessmentId, adminId).ToList());
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentByCandidateAssessmentId(candidateAssessmentId, adminId);
            var model = new ReviewSelfAssessmentViewModel()
            {
                SupervisorDelegateDetail = superviseDelegate,
                DelegateSelfAssessment = delegateSelfAssessment,
                CompetencyGroups = reviewedCompetencies.GroupBy(competency => competency.CompetencyGroup)
            };
            return View("ReviewSelfAssessment", model);
        }

        private List<Competency> PopulateCompetencyLevelDescriptors(List<Competency> reviewedCompetencies)
        {
            foreach (var competency in reviewedCompetencies)
            {
                foreach (var assessmentQuestion in competency.AssessmentQuestions)
                {
                    if (assessmentQuestion.AssessmentQuestionInputTypeID != 2)
                    {
                        assessmentQuestion.LevelDescriptors = selfAssessmentService.GetLevelDescriptorsForAssessmentQuestion(assessmentQuestion.Id, assessmentQuestion.MinValue, assessmentQuestion.MaxValue, assessmentQuestion.MinValue == 0).ToList();
                    }
                }
            }
            return reviewedCompetencies;
        }
        private AssessmentQuestion GetLevelDescriptorsForAssessmentQuestion (AssessmentQuestion assessmentQuestion)
        {
            if (assessmentQuestion.AssessmentQuestionInputTypeID != 2)
            {
                assessmentQuestion.LevelDescriptors = selfAssessmentService.GetLevelDescriptorsForAssessmentQuestion(assessmentQuestion.Id, assessmentQuestion.MinValue, assessmentQuestion.MaxValue, assessmentQuestion.MinValue == 0).ToList();
            }
            return assessmentQuestion;
        }
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/{viewMode}/{resultId}/")]
        public IActionResult ReviewCompetencySelfAssessment(int supervisorDelegateId, int candidateAssessmentId, string viewMode, int resultId)
        {
            var adminId = GetAdminID();
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);
            var competency = selfAssessmentService.GetCompetencyByCandidateAssessmentResultId(resultId, candidateAssessmentId, adminId);
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentBaseByCandidateAssessmentId(candidateAssessmentId);
            var assessmentQuestion = GetLevelDescriptorsForAssessmentQuestion(competency.AssessmentQuestions.First());
            var model = new ReviewCompetencySelfAsessmentViewModel()
            {
                DelegateSelfAssessment = delegateSelfAssessment,
                SupervisorDelegate = supervisorDelegate,
                Competency = competency,
                ResultSupervisorVerificationId = assessmentQuestion.SelfAssessmentResultSupervisorVerificationId,
                SupervisorComments = assessmentQuestion.SupervisorComments,
                SignedOff = assessmentQuestion.SignedOff != null ? (bool)assessmentQuestion.SignedOff : false
            };
            return View("ReviewCompetencySelfAsessment", model);
        }
        [HttpPost]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/{viewMode}/{resultId}/")]
        public IActionResult SubmitReviewCompetencySelfAssessment(int supervisorDelegateId, int candidateAssessmentId, string viewMode, int resultId, int resultSupervisorVerificationId, string? supervisorComments, bool signedOff)
        {
            if (supervisorService.UpdateSelfAssessmentResultSupervisorVerifications(resultSupervisorVerificationId, supervisorComments, signedOff, GetAdminID()))
            {
                //send notification to delegate:
                frameworkNotificationService.SendSupervisorResultReviewed(GetAdminID(), supervisorDelegateId, candidateAssessmentId, resultId);
            };
            return RedirectToAction("ReviewCompetencySelfAssessment", "Supervisor", new { supervisorDelegateId = supervisorDelegateId, candidateAssessmentId = candidateAssessmentId, viewMode = "View", resultId = resultId });
        }
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/VerifyMultiple/")]
        public IActionResult VerifyMultipleResults(int supervisorDelegateId, int candidateAssessmentId)
        {
            var adminId = GetAdminID();
            var superviseDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentBaseByCandidateAssessmentId(candidateAssessmentId);
            var reviewedCompetencies = PopulateCompetencyLevelDescriptors(selfAssessmentService.GetCandidateAssessmentResultsForReviewById(candidateAssessmentId, adminId).ToList());
            var model = new ReviewSelfAssessmentViewModel()
            {
                SupervisorDelegateDetail = superviseDelegate,
                DelegateSelfAssessment = delegateSelfAssessment,
                CompetencyGroups = reviewedCompetencies.GroupBy(competency => competency.CompetencyGroup)
            };
            return View("VerifyMultipleResults", model);
        }
        [HttpPost]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/VerifyMultiple/")]
        public IActionResult SubmitVerifyMultipleResults(int supervisorDelegateId, int candidateAssessmentId, List<int> resultChecked)
        {
            int countResults = 0;
            foreach (var result in resultChecked)
            {
                if (supervisorService.UpdateSelfAssessmentResultSupervisorVerifications(result, null, true, GetAdminID()))
                { countResults += 1; }
            }
            if (countResults > 0)
            {
                //Send notification
                frameworkNotificationService.SendSupervisorMultipleResultsReviewed(GetAdminID(), supervisorDelegateId, candidateAssessmentId, countResults);
            }
            return RedirectToAction("ReviewDelegateSelfAssessment", "Supervisor", new { supervisorDelegateId = supervisorDelegateId, candidateAssessmentId = candidateAssessmentId, viewMode = "Review" });
        }
            public IActionResult StartEnrolDelegateOnProfileAssessment(int supervisorDelegateId)
        {
            TempData.Clear();
            var sessionEnrolOnRoleProfile = new SessionEnrolOnRoleProfile();
            if (!Request.Cookies.ContainsKey(CookieName))
            {
                var id = Guid.NewGuid();

                Response.Cookies.Append(
                    CookieName,
                    id.ToString(),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    });

                sessionEnrolOnRoleProfile.Id = id;
            }
            else
            {
                if (Request.Cookies.TryGetValue(CookieName, out string idString))
                {
                    sessionEnrolOnRoleProfile.Id = Guid.Parse(idString);
                }
                else
                {
                    var id = Guid.NewGuid();

                    Response.Cookies.Append(
                        CookieName,
                        id.ToString(),
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30)
                        });

                    sessionEnrolOnRoleProfile.Id = id;
                }
            }
            TempData.Set(sessionEnrolOnRoleProfile);
            return RedirectToAction("EnrolDelegateOnProfileAssessment", "Supervisor", new { supervisorDelegateId = supervisorDelegateId });
        }
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/Profile")]
        public IActionResult EnrolDelegateOnProfileAssessment(int supervisorDelegateId)
        {
            SessionEnrolOnRoleProfile sessionEnrolOnRoleProfile = TempData.Peek<SessionEnrolOnRoleProfile>();
            TempData.Set(sessionEnrolOnRoleProfile);
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);
            var roleProfiles = supervisorService.GetAvailableRoleProfilesForDelegate((int)supervisorDelegate.CandidateID, GetCentreId());
            var model = new EnrolDelegateOnProfileAssessmentViewModel()
            {
                SessionEnrolOnRoleProfile = sessionEnrolOnRoleProfile,
                SupervisorDelegateDetail = supervisorDelegate,
                RoleProfiles = roleProfiles
            };
            return View("EnrolDelegateOnProfileAssessment", model);
        }
        [HttpPost]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/Profile")]
        public IActionResult EnrolSetRoleProfile(int supervisorDelegateId, int selfAssessmentID)
        {
            SessionEnrolOnRoleProfile sessionEnrolOnRoleProfile = TempData.Peek<SessionEnrolOnRoleProfile>();
            sessionEnrolOnRoleProfile.SelfAssessmentID = selfAssessmentID;
            TempData.Set(sessionEnrolOnRoleProfile);
            return RedirectToAction("EnrolDelegateCompleteBy", "Supervisor", new { supervisorDelegateId = supervisorDelegateId });
        }
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/CompleteBy")]
        public IActionResult EnrolDelegateCompleteBy(int supervisorDelegateId, int? day, int? month, int? year)
        {
            SessionEnrolOnRoleProfile sessionEnrolOnRoleProfile = TempData.Peek<SessionEnrolOnRoleProfile>();
            TempData.Set(sessionEnrolOnRoleProfile);
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);
            var roleProfile = supervisorService.GetRoleProfileById((int)sessionEnrolOnRoleProfile.SelfAssessmentID);
            var model = new EnrolDelegateSetCompletByDateViewModel()
            {
                SupervisorDelegateDetail = supervisorDelegate,
                RoleProfile = roleProfile,
                CompleteByDate = sessionEnrolOnRoleProfile.CompleteByDate
            };
            if (day != null && month != null && year != null)
            {
                model.CompleteByValidationResult = OldDateValidator.ValidateDate(day.Value, month.Value, year.Value);
            }
            return View("EnrolDelegateSetCompleteBy", model);
        }
        [HttpPost]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/CompleteBy")]
        public IActionResult EnrolDelegateSetCompleteBy(int supervisorDelegateId, int day, int month, int year)
        {
            SessionEnrolOnRoleProfile sessionEnrolOnRoleProfile = TempData.Peek<SessionEnrolOnRoleProfile>();
            if (day != 0 | month != 0 | year != 0)
            {
                var validationResult = OldDateValidator.ValidateDate(day, month, year);
                if (!validationResult.DateValid)
                {
                    return RedirectToAction("EnrolDelegateCompleteBy", new { supervisorDelegateId, day, month, year });
                }
                else
                {
                    var completeByDate = new DateTime(year, month, day);
                    sessionEnrolOnRoleProfile.CompleteByDate = completeByDate;
                    TempData.Set(sessionEnrolOnRoleProfile);
                }
            }
            var supervisorRoles = supervisorService.GetSupervisorRolesForSelfAssessment(sessionEnrolOnRoleProfile.SelfAssessmentID.Value);
            if (supervisorRoles.Count() > 1)
            {
                return RedirectToAction("EnrolDelegateSupervisorRole", "Supervisor", new { supervisorDelegateId = supervisorDelegateId });
            }
            else if (supervisorRoles.Count() == 1)
            {
                sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId = supervisorRoles.First().ID;
                TempData.Set(sessionEnrolOnRoleProfile);
            }
            return RedirectToAction("EnrolDelegateSummary", "Supervisor", new { supervisorDelegateId = supervisorDelegateId });
        }
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/SupervisorRole")]
        public IActionResult EnrolDelegateSupervisorRole(int supervisorDelegateId)
        {
            SessionEnrolOnRoleProfile sessionEnrolOnRoleProfile = TempData.Peek<SessionEnrolOnRoleProfile>();
            TempData.Set(sessionEnrolOnRoleProfile);
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);
            var roleProfile = supervisorService.GetRoleProfileById((int)sessionEnrolOnRoleProfile.SelfAssessmentID);
            var supervisorRoles = supervisorService.GetSupervisorRolesForSelfAssessment(sessionEnrolOnRoleProfile.SelfAssessmentID.Value);
            var model = new EnrolDelegateSupervisorRoleViewModel()
            {
                SupervisorDelegateDetail = supervisorDelegate,
                RoleProfile = roleProfile,
                SelfAssessmentSupervisorRoleId = sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId,
                SelfAssessmentSupervisorRoles = supervisorRoles
            };
            return View("EnrolDelegateSupervisorRole", model);
        }
        [HttpPost]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/SupervisorRole")]
        public IActionResult EnrolDelegateSetSupervisorRole(int supervisorDelegateId, int selfAssessmentSupervisorRoleId)
        {
            SessionEnrolOnRoleProfile sessionEnrolOnRoleProfile = TempData.Peek<SessionEnrolOnRoleProfile>();
            sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId = selfAssessmentSupervisorRoleId;
            TempData.Set(sessionEnrolOnRoleProfile);
            return RedirectToAction("EnrolDelegateSummary", "Supervisor", new { supervisorDelegateId = supervisorDelegateId });
        }
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/Summary")]
        public IActionResult EnrolDelegateSummary(int supervisorDelegateId)
        {
            SessionEnrolOnRoleProfile sessionEnrolOnRoleProfile = TempData.Peek<SessionEnrolOnRoleProfile>();
            TempData.Set(sessionEnrolOnRoleProfile);
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);
            var roleProfile = supervisorService.GetRoleProfileById((int)sessionEnrolOnRoleProfile.SelfAssessmentID);
            var supervisorRoleName = (sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId == null ? "Supervisor" : supervisorService.GetSupervisorRoleById(sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId.Value).RoleName);
            var supervisorRoleCount = (sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId == null ? 0 : supervisorService.GetSupervisorRolesForSelfAssessment(sessionEnrolOnRoleProfile.SelfAssessmentID.Value).Count());
            var model = new EnrolDelegateSummaryViewModel()
            {
                SupervisorDelegateDetail = supervisorDelegate,
                RoleProfile = roleProfile,
                SupervisorRoleName = supervisorRoleName,
                CompleteByDate = sessionEnrolOnRoleProfile.CompleteByDate,
                SupervisorRoleCount = supervisorRoleCount
            };
            return View("EnrolDelegateSummary", model);
        }
        public IActionResult EnrolDelegateConfirm(int delegateId, int supervisorDelegateId)
        {
            SessionEnrolOnRoleProfile sessionEnrolOnRoleProfile = TempData.Peek<SessionEnrolOnRoleProfile>();
            var selfAssessmentId = sessionEnrolOnRoleProfile.SelfAssessmentID;
            var completeByDate = sessionEnrolOnRoleProfile.CompleteByDate;
            var selfAssessmentSupervisorRoleId = sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId;
            var candidateAssessmentId = supervisorService.EnrolDelegateOnAssessment(delegateId, supervisorDelegateId, selfAssessmentId.Value, completeByDate, selfAssessmentSupervisorRoleId, GetAdminID());
            if (candidateAssessmentId > 0)
            {
                //send delegate notification:
                frameworkNotificationService.SendSupervisorEnroledDelegate(GetAdminID(), supervisorDelegateId, candidateAssessmentId, completeByDate);
            }
            return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
        }
        public IActionResult RemoveDelegateSelfAssessment(int candidateAssessmentId, int supervisorDelegateId)
        {
            supervisorService.RemoveCandidateAssessment(candidateAssessmentId);
            return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
        }
        public IActionResult SendReminderDelegateSelfAssessment(int candidateAssessmentId, int supervisorDelegateId)
        {
            frameworkNotificationService.SendReminderDelegateSelfAssessment(GetAdminID(), supervisorDelegateId, candidateAssessmentId);
            return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
        }
    }
}
