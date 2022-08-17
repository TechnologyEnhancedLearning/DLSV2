namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Newtonsoft.Json;

    public interface IMultiPageFormService
    {
        void SetMultiPageFormData(object formData, MultiPageFormDataFeature feature, ITempDataDictionary tempData);

        T GetMultiPageFormData<T>(MultiPageFormDataFeature feature, ITempDataDictionary tempData);

        void ClearMultiPageFormData(MultiPageFormDataFeature feature, ITempDataDictionary tempData);

        bool FormDataExistsForGuidAndFeature(MultiPageFormDataFeature feature, Guid tempDataGuid);
    }

    public class MultiPageFormService : IMultiPageFormService
    {
        private readonly IClockService clockService;

        private readonly IMultiPageFormDataService multiPageFormDataService;

        public MultiPageFormService(
            IClockService clockService,
            IMultiPageFormDataService multiPageFormDataService
        )
        {
            this.clockService = clockService;
            this.multiPageFormDataService = multiPageFormDataService;
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
                    multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(tempDataGuid, feature.Name);
                if (existingMultiPageFormData != null)
                {
                    multiPageFormDataService.UpdateJsonByGuid(tempDataGuid, json);
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
            multiPageFormDataService.InsertMultiPageFormData(multiPageFormData);
            tempData[feature.TempDataKey] = multiPageFormData.TempDataGuid;
        }

        public T GetMultiPageFormData<T>(MultiPageFormDataFeature feature, ITempDataDictionary tempData)
        {
            if (tempData[feature.TempDataKey] == null)
            {
                throw new MultiPageFormDataException("Attempted to get data with no Guid identifier");
            }

            var settings = new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var tempDataGuid = (Guid)tempData.Peek(feature.TempDataKey);
            var existingMultiPageFormData =
                multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(tempDataGuid, feature.Name);

            if (existingMultiPageFormData == null)
            {
                throw new MultiPageFormDataException($"MultiPageFormData not found for {tempDataGuid}");
            }

            tempData[feature.TempDataKey] = tempDataGuid;
            return JsonConvert.DeserializeObject<T>(existingMultiPageFormData.Json, settings);
        }

        public void ClearMultiPageFormData(MultiPageFormDataFeature feature, ITempDataDictionary tempData)
        {
            if (tempData[feature.TempDataKey] == null)
            {
                throw new MultiPageFormDataException("Attempted to clear data with no Guid identifier");
            }

            var tempDataGuid = (Guid)tempData.Peek(feature.TempDataKey);
            multiPageFormDataService.DeleteByGuid(tempDataGuid);
            tempData.Remove(feature.TempDataKey);
        }

        public bool FormDataExistsForGuidAndFeature(MultiPageFormDataFeature feature, Guid tempDataGuid)
        {
            var existingMultiPageFormData =
                multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(tempDataGuid, feature.Name);
            return existingMultiPageFormData != null;
        }
    }
}
