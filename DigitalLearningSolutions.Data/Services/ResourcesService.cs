namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IResourcesService
    {
        IEnumerable<ResourceGroup> GetAllResources();
    }

    public class ResourcesService : IResourcesService
    {
        private readonly IResourceDataService resourceDataService;

        public ResourcesService(IResourceDataService resourceDataService)
        {
            this.resourceDataService = resourceDataService;
        }

        public IEnumerable<ResourceGroup> GetAllResources()
        {
            var resources = resourceDataService.GetAllResources();

            var resourceGroups = resources.GroupBy(r => r.Category).Select(
                rg => new ResourceGroup(
                    rg.Key,
                    rg.OrderByDescending(r => r.UploadDateTime)
                )
            ).OrderBy(rc => rc.Category);

            return resourceGroups;
        }
    }
}
