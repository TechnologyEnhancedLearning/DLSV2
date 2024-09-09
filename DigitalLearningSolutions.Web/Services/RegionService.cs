using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Utilities;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IRegionService
    {
        IEnumerable<(int regionId, string regionName)> GetRegionsAlphabetical();
        string? GetRegionName(int regionId);
    }
    public class RegionService : IRegionService
    {
        private readonly IRegionDataService regionDataService;
        public RegionService(IRegionDataService regionDataService)
        {
            this.regionDataService = regionDataService;

        }

        public string? GetRegionName(int regionId)
        {
            return regionDataService.GetRegionName(regionId);
        }

        public IEnumerable<(int regionId, string regionName)> GetRegionsAlphabetical()
        {
            return regionDataService.GetRegionsAlphabetical();
        }
    }
}
