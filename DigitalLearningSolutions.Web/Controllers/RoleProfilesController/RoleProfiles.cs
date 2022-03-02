namespace DigitalLearningSolutions.Web.Controllers.RoleProfilesController
{
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.RoleProfiles;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Data.Models.SessionData.RoleProfiles;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    public partial class RoleProfilesController
    {
        private const string CookieName = "DLSFrameworkService";
        [Route("/RoleProfiles/View/{tabname}/{page=1:int}")]
        [SetSelectedTab(nameof(NavMenuTab.RolesProfiles))]
        public IActionResult ViewRoleProfiles(string tabname, string? searchString = null,
            string sortBy = RoleProfileSortByOptionTexts.RoleProfileName,
            string sortDirection = BaseRoleProfilesPageViewModel.AscendingText,
            int page = 1
            )
        {
            var adminId = GetAdminID();
            var isWorkforceManager = GetIsWorkforceManager();
            var isWorkforceContributor = GetIsWorkforceContributor();
            IEnumerable<RoleProfile> roleProfiles;
            if (tabname == "All")
            {
                roleProfiles = roleProfileService.GetAllRoleProfiles(adminId);
            }
            else
            {
                if (!isWorkforceContributor && !isWorkforceManager)
                {
                    return RedirectToAction("ViewRoleProfiles", "RoleProfiles", new { tabname = "All" });
                }
                roleProfiles = roleProfileService.GetRoleProfilesForAdminId(adminId);
            }
            if (roleProfiles == null)
            {
                logger.LogWarning($"Attempt to display role profiles for admin {adminId} returned null.");
                return StatusCode(403);
            }
            MyRoleProfilesViewModel myRoleProfiles;
            AllRoleProfilesViewModel allRoleProfiles;
            if (tabname == "Mine")
            {
                myRoleProfiles = new MyRoleProfilesViewModel(
                roleProfiles,
                searchString,
                sortBy,
                sortDirection,
                page,
                isWorkforceManager);
                allRoleProfiles = new AllRoleProfilesViewModel(
                    new List<RoleProfile>(),
                    searchString,
                    sortBy,
                    sortDirection,
                    page
                    );
            }
            else
            {
                allRoleProfiles = new AllRoleProfilesViewModel(
                                roleProfiles,
                                searchString,
                                sortBy,
                                sortDirection,
                                page);
                myRoleProfiles = new MyRoleProfilesViewModel(
                   new List<RoleProfile>(),
                   searchString,
                   sortBy,
                   sortDirection,
                   page,
                               isWorkforceManager
                   );
            }

            RoleProfilesViewModel? model = new RoleProfilesViewModel(
                isWorkforceManager,
                isWorkforceContributor,
                allRoleProfiles,
                myRoleProfiles
                );
            return View("Index", model);
        }
        public IActionResult StartNewRoleProfileSession()
        {
            var adminId = GetAdminID();
            TempData.Clear();
            var sessionNewRoleProfile = new SessionNewRoleProfile();
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

                sessionNewRoleProfile.Id = id;
            }
            else
            {
                if (Request.Cookies.TryGetValue(CookieName, out string idString))
                {
                    sessionNewRoleProfile.Id = Guid.Parse(idString);
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
                    sessionNewRoleProfile.Id = id;
                }
            }
            RoleProfileBase roleProfileBase = new RoleProfileBase()
            {
                BrandID = 6,
                OwnerAdminID = adminId,
                National = true,
                Public = true,
                PublishStatusID = 1,
                UserRole = 3
            };
            sessionNewRoleProfile.RoleProfileBase = roleProfileBase;
            TempData.Set(sessionNewRoleProfile);
            return RedirectToAction("RoleProfileName", "RoleProfiles", new { actionname = "New" });
        }

        [Route("/RoleProfiles/Name/{actionname}/{roleProfileId}")]
        [Route("/RoleProfiles/Name/{actionname}")]
        [SetSelectedTab(nameof(NavMenuTab.RolesProfiles))]
        public IActionResult RoleProfileName(string actionname, int roleProfileId = 0)
        {
            var adminId = GetAdminID();
            RoleProfileBase? roleProfileBase;
            if (roleProfileId > 0)
            {
                roleProfileBase = roleProfileService.GetRoleProfileBaseById(roleProfileId, adminId);
                if (roleProfileBase == null)
                {
                    logger.LogWarning($"Failed to load name page for roleProfileId: {roleProfileId} adminId: {adminId}");
                    return StatusCode(500);
                }
                if (roleProfileBase.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
            else
            {
                SessionNewRoleProfile sessionNewRoleProfile = TempData.Peek<SessionNewRoleProfile>();
                TempData.Set(sessionNewRoleProfile);
                roleProfileBase = sessionNewRoleProfile.RoleProfileBase;
                TempData.Set(sessionNewRoleProfile);
            }
            return View("Name", roleProfileBase);
        }

        [HttpPost]
        [Route("/RoleProfiles/Name/{actionname}/{roleProfileId}")]
        [Route("/RoleProfiles/Name/{actionname}")]
        [SetSelectedTab(nameof(NavMenuTab.RolesProfiles))]
        public IActionResult SaveProfileName(RoleProfileBase roleProfileBase, string actionname, int roleProfileId = 0)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(RoleProfileBase.RoleProfileName));
                ModelState.AddModelError(nameof(RoleProfileBase.RoleProfileName), "Please enter a valid role profile name (between 3 and 255 characters)");
                // do something
                return View("Name", roleProfileBase);
            }
            else
            {
                if (actionname == "New")
                {
                    var sameItems = roleProfileService.GetRoleProfileByName(roleProfileBase.RoleProfileName, GetAdminID());
                    if (sameItems != null)
                    {
                        ModelState.Remove(nameof(RoleProfileBase.RoleProfileName));
                        ModelState.AddModelError(nameof(RoleProfileBase.RoleProfileName), "Another role profile exists with that name. Please choose a different name.");
                        // do something
                        return View("Name", roleProfileBase);
                    }
                    SessionNewRoleProfile sessionNewRoleProfile = TempData.Peek<SessionNewRoleProfile>();
                    sessionNewRoleProfile.RoleProfileBase = roleProfileBase;
                    TempData.Set(sessionNewRoleProfile);
                    return RedirectToAction("RoleProfileProfessionalGroup", "RoleProfiles", new { actionname });
                }
                else
                {
                    var adminId = GetAdminID();
                    var isUpdated = roleProfileService.UpdateRoleProfileName(roleProfileBase.ID, adminId, roleProfileBase.RoleProfileName);
                    if (isUpdated)
                    {
                        return RedirectToAction("ViewRoleProfile", new { tabname = "Details", roleProfileId });
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(RoleProfileBase.RoleProfileName), "Another role profile exists with that name. Please choose a different name.");
                        // do something
                        return View("Name", roleProfileBase);
                    }
                }

            }
        }

        [Route("/RoleProfiles/ProfessionalGroup/{actionname}/{roleProfileId}")]
        [Route("/RoleProfiles/ProfessionalGroup/{actionname}")]
        [SetSelectedTab(nameof(NavMenuTab.RolesProfiles))]
        public IActionResult RoleProfileProfessionalGroup(string actionname, int roleProfileId = 0)
        {
            var adminId = GetAdminID();
            RoleProfileBase? roleProfileBase;
            if (roleProfileId > 0)
            {
                roleProfileBase = roleProfileService.GetRoleProfileBaseById(roleProfileId, adminId);
                if (roleProfileBase == null)
                {
                    logger.LogWarning($"Failed to load Professional Group page for roleProfileId: {roleProfileId} adminId: {adminId}");
                    return StatusCode(500);
                }
                if (roleProfileBase.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
            else
            {
                SessionNewRoleProfile sessionNewRoleProfile = TempData.Peek<SessionNewRoleProfile>();
                TempData.Set(sessionNewRoleProfile);
                roleProfileBase = sessionNewRoleProfile.RoleProfileBase;
                TempData.Set(sessionNewRoleProfile);
            }
            var professionalGroups = roleProfileService.GetNRPProfessionalGroups();
            var model = new ProfessionalGroupViewModel()
            {
                NRPProfessionalGroups = professionalGroups,
                RoleProfileBase = roleProfileBase
            };
            return View("ProfessionalGroup", model);
        }

        [HttpPost]
        [Route("/RoleProfiles/ProfessionalGroup/{actionname}/{roleProfileId}")]
        [Route("/RoleProfiles/ProfessionalGroup/{actionname}")]
        [SetSelectedTab(nameof(NavMenuTab.RolesProfiles))]
        public IActionResult SaveProfessionalGroup(RoleProfileBase roleProfileBase, string actionname, int roleProfileId = 0)
        {
            if (roleProfileBase.NRPProfessionalGroupID == null)
            {
                ModelState.Remove(nameof(RoleProfileBase.NRPProfessionalGroupID));
                ModelState.AddModelError(nameof(RoleProfileBase.NRPProfessionalGroupID), "Please choose a professional group" + (roleProfileId == 0 ? "or Skip this step" : "") + ".");
                // do something
                return View("Name", roleProfileBase);
            }
            if (actionname == "New")
            {
                SessionNewRoleProfile sessionNewRoleProfile = TempData.Peek<SessionNewRoleProfile>();
                sessionNewRoleProfile.RoleProfileBase = roleProfileBase;
                TempData.Set(sessionNewRoleProfile);
                return RedirectToAction("RoleProfileSubGroup", "RoleProfiles", new { actionname });
            }
            else
            {
                var adminId = GetAdminID();
                var isUpdated = roleProfileService.UpdateRoleProfileProfessionalGroup(roleProfileBase.ID, adminId, roleProfileBase.NRPProfessionalGroupID);
                if (isUpdated)
                {
                    return RedirectToAction("RoleProfileSubGroup", "RoleProfiles", new { actionname, roleProfileId });
                }
                else
                {
                    return RedirectToAction("ViewRoleProfile", new { tabname = "Details", roleProfileId });
                }
            }
        }

        [Route("/RoleProfiles/SubGroup/{actionname}/{roleProfileId}")]
        [Route("/RoleProfiles/SubGroup/{actionname}")]
        [SetSelectedTab(nameof(NavMenuTab.RolesProfiles))]
        public IActionResult RoleProfileSubGroup(string actionname, int roleProfileId = 0)
        {
            return View("SubGroup");
        }
    }
}
