namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class NumberOfAdministratorsViewComponent : ViewComponent
    {
        private readonly INumberOfAdministratorsHelper numberOfAdministratorsHelper;

        public NumberOfAdministratorsViewComponent(
            INumberOfAdministratorsHelper numberOfAdministratorsHelper
        )
        {
            this.numberOfAdministratorsHelper = numberOfAdministratorsHelper;
        }

        public IViewComponentResult Invoke(int centreId)
        {
            var numberOfAdministrators = numberOfAdministratorsHelper.GetCentreAdministratorNumbers(centreId);

            var numberOfAdminsViewModel = new NumberOfAdministratorsViewModel(numberOfAdministrators);

            return View(numberOfAdminsViewModel);
        }
    }
}
