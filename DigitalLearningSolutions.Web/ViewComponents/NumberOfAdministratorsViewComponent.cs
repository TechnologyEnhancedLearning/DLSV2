namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
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

        public IViewComponentResult Invoke(int centreId)
        {
            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);

            var numberOfAdminsViewModel = new NumberOfAdministratorsViewModel(centreDetails, adminUsersAtCentre);

            return View(numberOfAdminsViewModel);
        }
    }
}
