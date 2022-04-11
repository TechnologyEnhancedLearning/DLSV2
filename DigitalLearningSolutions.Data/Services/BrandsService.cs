﻿namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;

    public interface IBrandsService
    {
        IEnumerable<BrandDetail> GetPublicBrands();
    }

    public class BrandsService : IBrandsService
    {
        private readonly IBrandsDataService brandsDataService;

        public BrandsService(IBrandsDataService brandsDataService)
        {
            this.brandsDataService = brandsDataService;
        }

        public IEnumerable<BrandDetail> GetPublicBrands()
        {
            return brandsDataService.GetAllBrands()
                .Where(f => f.IncludeOnLanding);
        }
    }
}
