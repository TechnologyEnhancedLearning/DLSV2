namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class VerifyEmailWarningIfAppropriateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            bool isChooseACentrePage,
            bool isMyAccountPage,
            bool isRegistrationJourney,
            string? unverifiedPrimaryEmail,
            List<(string centreName, string? centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            var model = new VerifyEmailWarningIfAppropriateViewModel(
                isChooseACentrePage,
                isMyAccountPage,
                isRegistrationJourney,
                unverifiedPrimaryEmail,
                unverifiedCentreEmails
            );
            return View(model);
        }
    }
}
