namespace DigitalLearningSolutions.Web.Extensions
{
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Newtonsoft.Json;

    public static class TempDataExtension
    {
        public static T? Peek<T>(this ITempDataDictionary tempData) where T : class
        {
            var temp = (string?)tempData.Peek(typeof(T).Name);
            return temp != null ? JsonConvert.DeserializeObject<T>(temp) : null;
        }

        public static void Set<T>(this ITempDataDictionary tempData, T incomingTempData)
            => tempData[typeof(T).Name] = JsonConvert.SerializeObject(incomingTempData);

        public static T? Get<T>(this ITempDataDictionary tempData) where T : class
        {
            var temp = (string?)tempData[typeof(T).Name];
            return temp != null ? JsonConvert.DeserializeObject<T>(temp) : null;
        }
    }
}
