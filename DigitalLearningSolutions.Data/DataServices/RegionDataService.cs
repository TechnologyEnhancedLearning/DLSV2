namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;

    public interface IRegionDataService
    {
        public IEnumerable<(int, string)> GetRegionsAlphabetical();

    }
    public class RegionDataService : IRegionDataService
    {
        private readonly IDbConnection connection;

        public RegionDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<(int, string)> GetRegionsAlphabetical()
        {
            var jobGroups = connection.Query<(int, string)>(
                @"SELECT RegionID, RegionName
                        FROM Regions
                        ORDER BY RegionName"
            );
            return jobGroups;
        }
    }
}
