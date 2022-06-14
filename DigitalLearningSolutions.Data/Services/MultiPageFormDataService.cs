namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Newtonsoft.Json;

    public interface IMultiPageFormDataService
    {
        void SetMultiPageFormData(object formData, MultiPageFormDataFeature feature, ITempDataDictionary tempData);

        T GetMultiPageFormData<T>(MultiPageFormDataFeature feature, ITempDataDictionary tempData);

        void ClearMultiPageFormData(MultiPageFormDataFeature feature, ITempDataDictionary tempData);

        bool FormDataExistsForGuidAndFeature(MultiPageFormDataFeature feature, Guid tempDataGuid);
    }

    public class MultiPageFormDataService : IMultiPageFormDataService
    {
        private readonly IClockService clockService;

        private readonly IMultiPageFormDataDataService multiPageFormDataDataService;

        public MultiPageFormDataService(
            IClockService clockService,
            IMultiPageFormDataDataService multiPageFormDataDataService
        )
        {
            this.clockService = clockService;
            this.multiPageFormDataDataService = multiPageFormDataDataService;
        }

        public void SetMultiPageFormData(
            object formData,
            MultiPageFormDataFeature feature,
            ITempDataDictionary tempData
        )
        {
            var json = JsonConvert.SerializeObject(formData);

            if (tempData[feature.TempDataKey] != null)
            {
                var tempDataGuid = (Guid)tempData[feature.TempDataKey];
                var existingMultiPageFormData =
                    multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(tempDataGuid, feature.Name);
                if (existingMultiPageFormData != null)
                {
                    multiPageFormDataDataService.UpdateJsonByGuid(tempDataGuid, json);
                    tempData[feature.TempDataKey] = tempDataGuid;
                    return;
                }
            }

            var multiPageFormData = new MultiPageFormData
            {
                TempDataGuid = Guid.NewGuid(),
                Json = json,
                Feature = feature.Name,
                CreatedDate = clockService.UtcNow,
            };
            multiPageFormDataDataService.InsertMultiPageFormData(multiPageFormData);
            tempData[feature.TempDataKey] = multiPageFormData.TempDataGuid;
        }

        public T GetMultiPageFormData<T>(MultiPageFormDataFeature feature, ITempDataDictionary tempData)
        {
            if (tempData[feature.TempDataKey] == null)
            {
                throw new MultiPageFormDataException("Attempted to get data with no Guid identifier");
            }

            var tempDataGuid = (Guid)tempData.Peek(feature.TempDataKey);
            var existingMultiPageFormData =
                multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(tempDataGuid, feature.Name);

            if (existingMultiPageFormData == null)
            {
                throw new MultiPageFormDataException($"MultiPageFormData not found for {tempDataGuid}");
            }

            tempData[feature.TempDataKey] = tempDataGuid;
            return JsonConvert.DeserializeObject<T>(existingMultiPageFormData.Json);
        }

        public void ClearMultiPageFormData(MultiPageFormDataFeature feature, ITempDataDictionary tempData)
        {
            if (tempData[feature.TempDataKey] == null)
            {
                throw new MultiPageFormDataException("Attempted to clear data with no Guid identifier");
            }

            var tempDataGuid = (Guid)tempData.Peek(feature.TempDataKey);
            multiPageFormDataDataService.DeleteByGuid(tempDataGuid);
            tempData.Remove(feature.TempDataKey);
        }

        public bool FormDataExistsForGuidAndFeature(MultiPageFormDataFeature feature, Guid tempDataGuid)
        {
            var existingMultiPageFormData =
                multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(tempDataGuid, feature.Name);
            return existingMultiPageFormData != null;
        }
    }
}
