

namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DeactivateDelegate;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;


    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessDelegateUser))]
    [Route("TrackingSystem/Delegates/{delegateId:int}/Deactivate")]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    public class DeactivateDelegateController : Controller
    {
        private readonly IUserService userService;
        public DeactivateDelegateController(
           IUserService userService
       )
        {
            this.userService = userService;
        }
        [HttpGet]
        public IActionResult Index(int delegateId)
        {
          var checkDelegate =  userService.CheckDelegateIsActive(delegateId);
            if (checkDelegate != delegateId)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 410 });
            }
            var centreId = User.GetCentreId();
            var delegateEntity = userService.GetDelegateById(delegateId)!;
            var userEntity = userService.GetUserById(delegateEntity.DelegateAccount.UserId);
            var adminAccount = userEntity!.GetCentreAccountSet(centreId)?.AdminAccount;
            var roles = GetRoles(adminAccount, userEntity);
            var model = new DeactivateDelegateAccountViewModel
            {
                DelegateId = delegateId,
                Name = delegateEntity.UserAccount.FirstName + " " + delegateEntity.UserAccount.LastName,
                Roles = roles,
                Email = delegateEntity.UserAccount.PrimaryEmail,
                UserId = delegateEntity.UserAccount.Id    
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(DeactivateDelegateAccountViewModel deactivateDelegateAccountViewModel)
        {
            var centreId = User.GetCentreId();
            if (!ModelState.IsValid)
            {
                var delegateEntity = userService.GetDelegateById(deactivateDelegateAccountViewModel.DelegateId)!;
                var userEntity = userService.GetUserById(delegateEntity.DelegateAccount.UserId);
                var adminAccount = userEntity!.GetCentreAccountSet(centreId)?.AdminAccount;
                var roles = GetRoles(adminAccount, userEntity);
                deactivateDelegateAccountViewModel.Roles = roles;
                return View(deactivateDelegateAccountViewModel);
            }

            if (deactivateDelegateAccountViewModel.Deactivate == true )
            {
                userService.DeactivateDelegateUser(deactivateDelegateAccountViewModel.DelegateId);
                return RedirectToAction("Index", "ViewDelegate", new { deactivateDelegateAccountViewModel.DelegateId });
            }
            userService.DeactivateDelegateUser(deactivateDelegateAccountViewModel.DelegateId);
            userService.DeactivateAdminAccount(deactivateDelegateAccountViewModel.UserId,  centreId.Value);
            return RedirectToAction("Index", "ViewDelegate", new { deactivateDelegateAccountViewModel.DelegateId });
            
        }
        private List<string>? GetRoles(AdminAccount? adminAccount, UserEntity userEntity)
        {
            var roles = new List<string>();
            if (adminAccount != null)
            {
                var adminentity = new AdminEntity(adminAccount, userEntity.UserAccount, null);
                CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                roles = FilterableTagHelper.GetCurrentTagsForAdmin(adminentity).Where(s => s.Hidden == false)
                                                .Select(d => d.DisplayText).ToList<string>();
            }
            return roles;
        }
    }
}
