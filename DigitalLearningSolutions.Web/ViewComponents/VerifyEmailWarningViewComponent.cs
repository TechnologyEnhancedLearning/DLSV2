namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class VerifyEmailWarningViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            bool isMyAccountPage,
            bool primaryEmailIsUnverified,
            List<string>? unverifiedCentreEmails
        )
        {
            var model = new VerifyEmailWarningViewModel(
                isMyAccountPage,
                primaryEmailIsUnverified,
                unverifiedCentreEmails
            );
            return View(model);
        }
    }
}
