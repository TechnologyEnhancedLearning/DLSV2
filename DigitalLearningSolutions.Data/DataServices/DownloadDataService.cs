namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IDownloadDataService
    {
        IEnumerable<Resource> GetAllResources();
    }

    public class DownloadDataService : IDownloadDataService
    {
        private readonly IDbConnection connection;

        public DownloadDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Resource> GetAllResources()
        {
            return connection.Query<Resource>(
                @"SELECT
                        Category,
                        Description,
                        UploadDTT AS UploadDate,
                        FileSize,
                        Tag,
                        Filename
                    FROM Downloads
                    WHERE Active = 1"
            );
        }
    }
}
