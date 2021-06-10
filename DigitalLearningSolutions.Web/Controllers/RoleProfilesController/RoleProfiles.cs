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
    public partial class RoleProfileController
    {
        private const string CookieName = "DLSFrameworkService";
        [Route("/WorkforceManager/MyRoleProfiles/{page=1:int}")]
        public IActionResult MyRoleProfiles(string? searchString = null,
            string sortBy = RoleProfileSortByOptionTexts.RoleProfileCreatedDate,
            string sortDirection = BaseRoleProfilesPageViewModel.DescendingText,
            int page = 1)
        {
            var adminId = GetAdminID();
            var isWorkforceManager = GetIsWorkforceManager();
            var roleProfiles = roleProfileService.GetRoleProfilesForAdminId(adminId);
            if (roleProfiles == null)
            {
                logger.LogWarning($"Attempt to display role profiles for admin {adminId} returned null.");
                return StatusCode(403);
            }
            var model = new MyRoleProfilesViewModel(
                roleProfiles,
                searchString,
                sortBy,
                sortDirection,
                page,
                isWorkforceManager);
            return View("WorkforceManager/MyRoleProfiles", model);
        }
    }
}
