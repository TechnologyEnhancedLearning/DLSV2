namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class VerifyEmailWarningIfAppropriateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            bool showCentreEmailsDetailedList,
            bool showResendButton,
            bool mentionBlockedActionsOnChooseACentrePage,
            bool primaryEmailIsUnverified,
            List<(string centreName, string? centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            var model = new VerifyEmailWarningIfAppropriateViewModel(
                showCentreEmailsDetailedList,
                showResendButton,
                mentionBlockedActionsOnChooseACentrePage,
                primaryEmailIsUnverified,
                unverifiedCentreEmails
            );
            return View(model);
        }
    }
}
