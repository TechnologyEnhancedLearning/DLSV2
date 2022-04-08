namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IBrandsService
    {
        IEnumerable<BrandDetail> GetPublicBrandsDetails();
    }

    public class BrandsService : IBrandsService
    {
        private readonly IBrandsDataService brandsDataService;

        public BrandsService(IBrandsDataService brandsDataService)
        {
            this.brandsDataService = brandsDataService;
        }

        public IEnumerable<BrandDetail> GetPublicBrandsDetails()
        {
            return brandsDataService.GetAllBrands()
                .Where(f => f.IncludeOnLanding);
        }
    }
}
