namespace DigitalLearningSolutions.Web.ViewModels.Support.Resources
{
    using DigitalLearningSolutions.Data.Models.Support;
    using System.Collections.Generic;
    using System.Linq;

    public class ResourceGroupViewModel
    {
        public ResourceGroupViewModel(ResourceGroup resourceGroup, string currentSystemBaseUrl)
        {
            Category = resourceGroup.Category;
            Resources = resourceGroup.Resources.Select(r => new ResourceViewModel(r, currentSystemBaseUrl));
        }

        public string Category { get; }
        public IEnumerable<ResourceViewModel> Resources { get; }
    }
}
