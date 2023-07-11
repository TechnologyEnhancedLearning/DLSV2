namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IRegionDataService
    {
        public IEnumerable<(int regionId, string regionName)> GetRegionsAlphabetical();
        string? GetRegionName(int regionId);
    }

    public class RegionDataService : IRegionDataService
    {
        private readonly IDbConnection connection;

        public RegionDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public string? GetRegionName(int regionId)
        {
            var name = connection.QueryFirstOrDefault<string?>(
                 @"SELECT RegionName
                        FROM Regions
                        WHERE RegionID = @regionId",
                 new { regionId }
             );
            return name;
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
