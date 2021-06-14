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
    public partial class RoleProfilesController
    {
        private const string CookieName = "DLSFrameworkService";
        [Route("/RoleProfiles/View/{tabname}/{page=1:int}")]
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
    }
}
