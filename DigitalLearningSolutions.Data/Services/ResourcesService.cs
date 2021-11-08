namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IResourcesService
    {
        IEnumerable<ResourceCategory> GetAllResources();
    }

    public class ResourcesService : IResourcesService
    {
        private readonly IResourceDataService resourceDataService;

        public ResourcesService(IResourceDataService resourceDataService)
        {
            this.resourceDataService = resourceDataService;
        }

        public IEnumerable<ResourceCategory> GetAllResources()
        {
            var resources = resourceDataService.GetAllResources();

            var resourceCategories = resources.GroupBy(d => d.Category).Select(
                dg => new ResourceCategory(
                    dg.Key,
                    dg.OrderBy(r => r.UploadDate)
                )
            ).OrderBy(rc => rc.CategoryName);

            return resourceCategories;
        }
    }
}
