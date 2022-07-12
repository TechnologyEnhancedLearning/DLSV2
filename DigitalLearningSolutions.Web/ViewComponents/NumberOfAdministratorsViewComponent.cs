namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class NumberOfAdministratorsViewComponent : ViewComponent
    {
        private readonly ICentreContractAdminUsageService centreContractAdminUsageService;

        public NumberOfAdministratorsViewComponent(
            ICentreContractAdminUsageService centreContractAdminUsageService
        )
        {
            this.centreContractAdminUsageService = centreContractAdminUsageService;
        }

        public IViewComponentResult Invoke(int centreId)
        {
            var numberOfAdministrators = centreContractAdminUsageService.GetCentreAdministratorNumbers(centreId);

            var numberOfAdminsViewModel = new NumberOfAdministratorsViewModel(numberOfAdministrators);

            return View(numberOfAdminsViewModel);
        }
    }
}
