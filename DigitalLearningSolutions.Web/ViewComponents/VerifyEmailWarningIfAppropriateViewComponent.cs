namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class VerifyEmailWarningIfAppropriateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            bool isMyAccountPage,
            bool mentionBlockedActionsOnChooseACentrePage,
            bool primaryEmailIsUnverified,
            List<(int centreId, string centreName, string? centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            var model = new VerifyEmailWarningIfAppropriateViewModel(
                isMyAccountPage,
                mentionBlockedActionsOnChooseACentrePage,
                primaryEmailIsUnverified,
                unverifiedCentreEmails.Select(uce => (uce.centreName, uce.centreSpecificEmail)).ToList()
            );
            return View(model);
        }
    }
}
