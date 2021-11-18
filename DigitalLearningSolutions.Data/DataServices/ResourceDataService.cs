namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IResourceDataService
    {
        IEnumerable<Resource> GetAllResources();
    }

    public class ResourceDataService : IResourceDataService
    {
        private readonly IDbConnection connection;

        public ResourceDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Resource> GetAllResources()
        {
            return connection.Query<Resource>(
                @"SELECT
                        Category,
                        Description,
                        UploadDTT AS UploadDateTime,
                        FileSize,
                        Tag,
                        Filename
                    FROM Downloads
                    WHERE Active = 1"
            );
        }
    }
}
