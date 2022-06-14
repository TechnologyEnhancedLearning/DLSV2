namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData;

    public interface IMultiPageFormDataDataService
    {
        MultiPageFormData? GetMultiPageFormDataByGuidAndFeature(Guid tempDataGuid, string feature);

        void InsertMultiPageFormData(MultiPageFormData multiPageFormData);

        void UpdateJsonByGuid(Guid tempDataGuid, string json);

        void DeleteByGuid(Guid tempDataGuid);
    }

    public class MultiPageFormDataDataService : IMultiPageFormDataDataService
    {
        private readonly IDbConnection connection;

        public MultiPageFormDataDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public MultiPageFormData? GetMultiPageFormDataByGuidAndFeature(Guid tempDataGuid, string feature)
        {
            return connection.QuerySingleOrDefault<MultiPageFormData>(
                @"SELECT
                        ID,
                        TempDataGuid,
                        Json,
                        Feature,
                        CreatedDate
                    FROM MultiPageFormData
                    WHERE TempDataGuid = @tempDataGuid AND Feature = @feature",
                new { tempDataGuid, feature }
            );
        }

        public void InsertMultiPageFormData(MultiPageFormData multiPageFormData)
        {
            connection.Execute(
                @"INSERT MultiPageFormData (TempDataGuid, Json, Feature, CreatedDate)
                    VALUES (@TempDataGuid, @Json, @Feature, @CreatedDate)",
                multiPageFormData
            );
        }

        public void UpdateJsonByGuid(Guid tempDataGuid, string json)
        {
            connection.Execute(
                @"UPDATE MultiPageFormData SET Json = @json WHERE TempDataGuid = @tempDataGuid",
                new { tempDataGuid, json }
            );
        }

        public void DeleteByGuid(Guid tempDataGuid)
        {
            connection.Execute(
                @"DELETE MultiPageFormData WHERE TempDataGuid = @tempDataGuid",
                new { tempDataGuid }
            );
        }
    }
}
