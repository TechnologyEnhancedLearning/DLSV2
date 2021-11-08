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
        private readonly IDownloadDataService downloadDataService;

        public ResourcesService(IDownloadDataService downloadDataService)
        {
            this.downloadDataService = downloadDataService;
        }

        public IEnumerable<ResourceCategory> GetAllResources()
        {
            var resources = downloadDataService.GetAllResources();

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
