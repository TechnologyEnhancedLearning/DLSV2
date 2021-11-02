namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IDownloadDataService
    {
        IEnumerable<Download> GetAllDownloads();
    }

    public class DownloadDataService : IDownloadDataService
    {
        private readonly IDbConnection connection;

        public DownloadDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Download> GetAllDownloads()
        {
            return connection.Query<Download>(
                @"SELECT
                        Category,
                        Description,
                        UploadDTT AS UploadDate,
                        FileSize,
                        Tag
                    FROM Downloads
                    WHERE (Active = 1)"
            );
        }
    }
}
