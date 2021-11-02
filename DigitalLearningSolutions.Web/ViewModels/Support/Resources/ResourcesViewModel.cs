namespace DigitalLearningSolutions.Web.ViewModels.Support.Resources
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class ResourcesViewModel : SupportViewModel
    {
        public ResourcesViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl,
            IEnumerable<ResourceCategory> resourceCategories
        ) : base(dlsSubApplication, currentPage, currentSystemBaseUrl)
        {
            CurrentPage = currentPage;
            DlsSubApplication = dlsSubApplication;
            Categories = resourceCategories.Select(rc => new ResourcesCategoryViewModel(rc, currentSystemBaseUrl));
        }

        public IEnumerable<ResourcesCategoryViewModel> Categories { get; }
    }
}
