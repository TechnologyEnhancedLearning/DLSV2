namespace DigitalLearningSolutions.Web.ViewModels.Support.Resources
{
    using DigitalLearningSolutions.Data.Models.Support;
    using System.Collections.Generic;
    using System.Linq;

    public class ResourcesCategoryViewModel
    {
        public ResourcesCategoryViewModel(ResourceCategory resourceCategory, string currentSystemBaseUrl)
        {
            CategoryName = resourceCategory.CategoryName;
            Resources = resourceCategory.Resources.Select(r => new ResourcesItemViewModel(r, currentSystemBaseUrl));
        }

        public string CategoryName { get; }
        public IEnumerable<ResourcesItemViewModel> Resources { get; }
    }
}
