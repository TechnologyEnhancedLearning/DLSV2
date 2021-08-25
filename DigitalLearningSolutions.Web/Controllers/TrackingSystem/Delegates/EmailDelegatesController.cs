namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/Email")]
    public class EmailDelegatesController : Controller
    {
        private readonly IUserService userService;

        public EmailDelegatesController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var delegateUsers = GetDelegateUserCards();
            var model = new EmailDelegatesViewModel(delegateUsers);

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(EmailDelegatesViewModel model)
        {
            var delegateUsers = GetDelegateUserCards();

            if (!ModelState.IsValid)
            {
                model.SetDelegates(delegateUsers);
                return View(model);
            }

            var selectedUsers = delegateUsers.Where(user => model.SelectedDelegateIds!.Contains(user.Id));
            var emails = selectedUsers.Select(delegateUser => delegateUser.EmailAddress);
            var emailDate = new DateTime(model.Year!.Value, model.Month!.Value, model.Day!.Value);
            return new ObjectResult(emailDate.ToString("dd/MM/yyyy") + "\n" + string.Join("\n", emails.ToList()));
        }

        private IEnumerable<DelegateUserCard> GetDelegateUserCards()
        {
            var centreId = User.GetCentreId();
            return userService.GetDelegateUserCardsForWelcomeEmail(centreId)
                .OrderByDescending(card => card.DateRegistered).Take(5);
        }
    }
}
