﻿namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class NumberOfAdministratorsViewComponent : ViewComponent
    {
        private readonly ICentresDataService centresDataService;
        private readonly IUserDataService userDataService;

        public NumberOfAdministratorsViewComponent(
            ICentresDataService centresDataService,
            IUserDataService userDataService
        )
        {
            this.centresDataService = centresDataService;
            this.userDataService = userDataService;
        }

        /// <summary>
        /// Render NumberOfAdministrators view component.
        /// </summary>
        /// <param name="centreId"></param>
        /// <returns></returns>
        public IViewComponentResult Invoke(int centreId)
        {
            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);

            var numberOfAdminsViewModel = new NumberOfAdministratorsViewModel(centreDetails, adminUsersAtCentre);

            return View(numberOfAdminsViewModel);
        }
    }
}
