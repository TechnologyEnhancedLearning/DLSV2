namespace DigitalLearningSolutions.Web.Controllers.CompetencyAssessmentsController
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Data.Models.SessionData.CompetencyAssessments;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;

    public partial class CompetencyAssessmentsController
    {
        private const string CookieName = "DLSFrameworkService";
        [Route("/CompetencyAssessments/View/{tabname}/{page=1:int}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult ViewCompetencyAssessments(string tabname, string? searchString = null,
            string sortBy = CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentName,
            string sortDirection = BaseCompetencyAssessmentsPageViewModel.AscendingText,
            int page = 1
            )
        {
            var adminId = GetAdminID();
            var isWorkforceManager = GetIsWorkforceManager() | GetIsFrameworkDeveloper();
            var isWorkforceContributor = GetIsWorkforceContributor() | GetIsFrameworkContributor();
            IEnumerable<CompetencyAssessment> competencyAssessments;
            if (tabname == "All")
            {
                competencyAssessments = competencyAssessmentService.GetAllCompetencyAssessments(adminId);
            }
            else
            {
                if (!isWorkforceContributor && !isWorkforceManager)
                {
                    return RedirectToAction("ViewCompetencyAssessments", "CompetencyAssessments", new { tabname = "All" });
                }
                competencyAssessments = competencyAssessmentService.GetCompetencyAssessmentsForAdminId(adminId);
            }
            if (competencyAssessments == null)
            {
                logger.LogWarning($"Attempt to display competency assessments for admin {adminId} returned null.");
                return StatusCode(403);
            }
            MyCompetencyAssessmentsViewModel myCompetencyAssessments;
            AllCompetencyAssessmentsViewModel allCompetencyAssessments;
            if (tabname == "Mine")
            {
                myCompetencyAssessments = new MyCompetencyAssessmentsViewModel(
                competencyAssessments,
                searchString,
                sortBy,
                sortDirection,
                page,
                isWorkforceManager);
                allCompetencyAssessments = new AllCompetencyAssessmentsViewModel(
                    new List<CompetencyAssessment>(),
                    searchString,
                    sortBy,
                    sortDirection,
                    page
                    );
            }
            else
            {
                allCompetencyAssessments = new AllCompetencyAssessmentsViewModel(
                                competencyAssessments,
                                searchString,
                                sortBy,
                                sortDirection,
                                page);
                myCompetencyAssessments = new MyCompetencyAssessmentsViewModel(
                   new List<CompetencyAssessment>(),
                   searchString,
                   sortBy,
                   sortDirection,
                   page,
                   isWorkforceManager
                   );
            }

            var currentTab = tabname == "All" ? CompetencyAssessmentsTab.AllCompetencyAssessments : CompetencyAssessmentsTab.MyCompetencyAssessments;
            CompetencyAssessmentsViewModel? model = new CompetencyAssessmentsViewModel(
                isWorkforceManager,
                isWorkforceContributor,
                allCompetencyAssessments,
                myCompetencyAssessments,
                currentTab
            );
            return View("Index", model);
        }
        public IActionResult StartNewCompetencyAssessmentSession()
        {
            var adminId = GetAdminID();
            TempData.Clear();
            var sessionNewCompetencyAssessment = new SessionNewCompetencyAssessment();
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

                sessionNewCompetencyAssessment.Id = id;
            }
            else
            {
                if (Request.Cookies.TryGetValue(CookieName, out string idString))
                {
                    sessionNewCompetencyAssessment.Id = Guid.Parse(idString);
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
                    sessionNewCompetencyAssessment.Id = id;
                }
            }
            CompetencyAssessmentBase competencyAssessmentBase = new CompetencyAssessmentBase()
            {
                BrandID = 6,
                OwnerAdminID = adminId,
                National = true,
                Public = true,
                PublishStatusID = 1,
                UserRole = 3
            };
            sessionNewCompetencyAssessment.CompetencyAssessmentBase = competencyAssessmentBase;
            TempData.Set(sessionNewCompetencyAssessment);
            return RedirectToAction("CompetencyAssessmentName", "CompetencyAssessments", new { actionname = "New" });
        }

        [Route("/CompetencyAssessments/Name/{actionname}/{competencyAssessmentId}")]
        [Route("/CompetencyAssessments/Name/{actionname}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult CompetencyAssessmentName(string actionname, int competencyAssessmentId = 0)
        {
            var adminId = GetAdminID();
            CompetencyAssessmentBase? competencyAssessmentBase;
            if (competencyAssessmentId > 0)
            {
                competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
                if (competencyAssessmentBase == null)
                {
                    logger.LogWarning($"Failed to load name page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                    return StatusCode(500);
                }
                if (competencyAssessmentBase.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
            else
            {
                SessionNewCompetencyAssessment sessionNewCompetencyAssessment = TempData.Peek<SessionNewCompetencyAssessment>();
                TempData.Set(sessionNewCompetencyAssessment);
                competencyAssessmentBase = sessionNewCompetencyAssessment.CompetencyAssessmentBase;
                TempData.Set(sessionNewCompetencyAssessment);
            }
            return View("Name", competencyAssessmentBase);
        }

        [HttpPost]
        [Route("/CompetencyAssessments/Name/{actionname}/{competencyAssessmentId}")]
        [Route("/CompetencyAssessments/Name/{actionname}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult SaveProfileName(CompetencyAssessmentBase competencyAssessmentBase, string actionname, int competencyAssessmentId = 0, int? frameworkId = null)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(CompetencyAssessmentBase.CompetencyAssessmentName));
                ModelState.AddModelError(nameof(CompetencyAssessmentBase.CompetencyAssessmentName), "Please enter a valid competency assessment name (between 3 and 255 characters)");
                return View("Name", competencyAssessmentBase);
            }
            else
            {
                var userCentreId = (int)GetCentreId();
                var adminId = GetAdminID();
                if (actionname == "New")
                {
                    var sameItems = competencyAssessmentService.GetCompetencyAssessmentByName(competencyAssessmentBase.CompetencyAssessmentName, GetAdminID());
                    if (sameItems != null)
                    {
                        ModelState.Remove(nameof(CompetencyAssessmentBase.CompetencyAssessmentName));
                        ModelState.AddModelError(nameof(CompetencyAssessmentBase.CompetencyAssessmentName), "Another competency assessment exists with that name. Please choose a different name.");
                        return View("Name", competencyAssessmentBase);
                    }
                    competencyAssessmentService.InsertCompetencyAssessment(adminId, userCentreId, competencyAssessmentBase.CompetencyAssessmentName, frameworkId);
                }
                else
                { 
                    
                    var isUpdated = competencyAssessmentService.UpdateCompetencyAssessmentName(competencyAssessmentBase.ID, adminId, competencyAssessmentBase.CompetencyAssessmentName);
                    if (!isUpdated)
                    {
                        ModelState.AddModelError(nameof(CompetencyAssessmentBase.CompetencyAssessmentName), "Another competency assessment exists with that name. Please choose a different name.");
                        return View("Name", competencyAssessmentBase);
                    }
                }
                return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId });
            }
        }

        [Route("/CompetencyAssessments/ProfessionalGroup/{actionname}/{competencyAssessmentId}")]
        [Route("/CompetencyAssessments/ProfessionalGroup/{actionname}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult CompetencyAssessmentProfessionalGroup(string actionname, int competencyAssessmentId = 0)
        {
            var adminId = GetAdminID();
            CompetencyAssessmentBase? competencyAssessmentBase;
            if (competencyAssessmentId > 0)
            {
                competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
                if (competencyAssessmentBase == null)
                {
                    logger.LogWarning($"Failed to load Professional Group page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                    return StatusCode(500);
                }
                if (competencyAssessmentBase.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
            else
            {
                SessionNewCompetencyAssessment sessionNewCompetencyAssessment = TempData.Peek<SessionNewCompetencyAssessment>();
                TempData.Set(sessionNewCompetencyAssessment);
                competencyAssessmentBase = sessionNewCompetencyAssessment.CompetencyAssessmentBase;
                TempData.Set(sessionNewCompetencyAssessment);
            }
            var professionalGroups = competencyAssessmentService.GetNRPProfessionalGroups();
            var model = new ProfessionalGroupViewModel()
            {
                NRPProfessionalGroups = professionalGroups,
                CompetencyAssessmentBase = competencyAssessmentBase
            };
            return View("ProfessionalGroup", model);
        }

        [HttpPost]
        [Route("/CompetencyAssessments/ProfessionalGroup/{actionname}/{competencyAssessmentId}")]
        [Route("/CompetencyAssessments/ProfessionalGroup/{actionname}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult SaveProfessionalGroup(CompetencyAssessmentBase competencyAssessmentBase, string actionname, int competencyAssessmentId = 0)
        {
            if (competencyAssessmentBase.NRPProfessionalGroupID == null)
            {
                ModelState.Remove(nameof(CompetencyAssessmentBase.NRPProfessionalGroupID));
                ModelState.AddModelError(nameof(CompetencyAssessmentBase.NRPProfessionalGroupID), "Please choose a professional group" + (competencyAssessmentId == 0 ? "or Skip this step" : "") + ".");
                // do something
                return View("Name", competencyAssessmentBase);
            }
            if (actionname == "New")
            {
                SessionNewCompetencyAssessment sessionNewCompetencyAssessment = TempData.Peek<SessionNewCompetencyAssessment>();
                sessionNewCompetencyAssessment.CompetencyAssessmentBase = competencyAssessmentBase;
                TempData.Set(sessionNewCompetencyAssessment);
                return RedirectToAction("CompetencyAssessmentSubGroup", "CompetencyAssessments", new { actionname });
            }
            else
            {
                var adminId = GetAdminID();
                var isUpdated = competencyAssessmentService.UpdateCompetencyAssessmentProfessionalGroup(competencyAssessmentBase.ID, adminId, competencyAssessmentBase.NRPProfessionalGroupID);
                if (isUpdated)
                {
                    return RedirectToAction("CompetencyAssessmentSubGroup", "CompetencyAssessments", new { actionname, competencyAssessmentId });
                }
                else
                {
                    return RedirectToAction("ManageCompetencyAssessment", new { tabname = "Details", competencyAssessmentId });
                }
            }
        }

        [Route("/CompetencyAssessments/SubGroup/{actionname}/{competencyAssessmentId}")]
        [Route("/CompetencyAssessments/SubGroup/{actionname}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult CompetencyAssessmentSubGroup(string actionname, int competencyAssessmentId = 0)
        {
            return View("SubGroup");
        }
    }
}
