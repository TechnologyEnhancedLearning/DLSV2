namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;

    public interface IRegionDataService
    {
        public IEnumerable<(int regionId, string regionName)> GetRegionsAlphabetical();
    }

    public class RegionDataService : IRegionDataService
    {
        private readonly IDbConnection connection;

        public RegionDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<(int regionId, string regionName)> GetRegionsAlphabetical()
        {
            return connection.Query<(int, string)>(
                @"SELECT RegionID, RegionName
                        FROM Regions
                        ORDER BY RegionName"
            );
        }
    }
}
