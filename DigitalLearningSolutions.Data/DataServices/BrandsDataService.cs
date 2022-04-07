namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common;

    public interface IBrandsDataService
    {
        IEnumerable<BrandDetail> GetAllBrands();

        BrandDetail? GetPublicBrandById(int brandId);
    }

    public class BrandsDataService : IBrandsDataService
    {
        private const string BrandsSql =
            @"SELECT
                    BrandID,
	                BrandName,
	                BrandDescription,
	                BrandImage,
                    ImageFileType,
	                IncludeOnLanding,
	                ContactEmail,
	                OwnerOrganisationID,
	                Active,
                    OrderByNumber,
                    BrandLogo,
                    PopularityHigh
                FROM Brands";

        private readonly IDbConnection connection;

        public BrandsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<BrandDetail> GetAllBrands()
        {
            return connection.Query<BrandDetail>($@"{BrandsSql}");
        }

        public BrandDetail? GetPublicBrandById(int brandId)
        {
            return connection.Query<BrandDetail>(
                @$"{BrandsSql} WHERE BrandID = @brandId AND IncludeOnLanding = 1",
                new { brandId }
            ).SingleOrDefault();
        }
    }
}
