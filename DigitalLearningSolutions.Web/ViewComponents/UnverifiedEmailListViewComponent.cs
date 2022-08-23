namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class UnverifiedEmailListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string? primaryEmailIfUnverified,
            List<(int centreId, string centreName, string centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            var centreNamesAndEmails = unverifiedCentreEmails.Select(uce => (uce.centreName, uce.centreSpecificEmail));
            var groupedEmails = centreNamesAndEmails.GroupBy(uce => uce.centreSpecificEmail);
            var dictionaryOfUnverifiedEmailsAndCentreNames = groupedEmails.ToDictionary(
                groupedEmail => groupedEmail.Key,
                groupedEmail => groupedEmail.Select(ge => ge.centreName).ToList()
            );

            var model = new UnverifiedEmailListViewModel(
                primaryEmailIfUnverified,
                dictionaryOfUnverifiedEmailsAndCentreNames
            );

            return View(model);
        }
    }
}
