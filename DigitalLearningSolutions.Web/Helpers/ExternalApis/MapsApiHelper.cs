namespace DigitalLearningSolutions.Web.Helpers.ExternalApis
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.External.Maps;
    using Newtonsoft.Json;

    public interface IMapsApiHelper
    {
        Task<MapsResponse> GetPostcodeCoordinates(string postcode);
    }

    public class MapsApiHelper : IMapsApiHelper
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task<MapsResponse> GetPostcodeCoordinates(string postcode)
        {
            var mapsApiKey = ConfigHelper.GetAppConfig()["MapsAPIKey"];
            var mapsApiUrl =
                $"https://maps.googleapis.com/maps/api/geocode/json?components=postal_code:{postcode}|country:GB&key={mapsApiKey}";

            Client.DefaultRequestHeaders.Accept.Clear();

            var response = await Client.GetStringAsync(mapsApiUrl);
            var result = JsonConvert.DeserializeObject<MapsResponse>(response);

            return result;
        }
    }
}
