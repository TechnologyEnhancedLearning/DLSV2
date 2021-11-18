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
            IEnumerable<ResourceGroup> resourceGroups
        ) : base(dlsSubApplication, currentPage, currentSystemBaseUrl)
        {
            CurrentPage = currentPage;
            DlsSubApplication = dlsSubApplication;
            Categories = resourceGroups.Select(rc => new ResourceGroupViewModel(rc, currentSystemBaseUrl));
        }

        public IEnumerable<ResourceGroupViewModel> Categories { get; }
    }
}
