namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class NumberOfAdministratorsViewComponent : ViewComponent
    {
        private readonly INumberOfAdministratorsService numberOfAdministratorsService;

        public NumberOfAdministratorsViewComponent(
            INumberOfAdministratorsService numberOfAdministratorsService
        )
        {
            this.numberOfAdministratorsService = numberOfAdministratorsService;
        }

        public IViewComponentResult Invoke(int centreId)
        {
            var numberOfAdministrators = numberOfAdministratorsService.GetCentreAdministratorNumbers(centreId);

            var numberOfAdminsViewModel = new NumberOfAdministratorsViewModel(numberOfAdministrators);

            return View(numberOfAdminsViewModel);
        }
    }
}
