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
        private readonly HttpClient client;
        private readonly string mapsApiBaseUrl;

        public MapsApiHelper(HttpClient httpClient)
        {
            var mapsApiKey = ConfigHelper.GetAppConfig()["MapsAPIKey"];
            mapsApiBaseUrl =
                $"https://maps.googleapis.com/maps/api/geocode/json?key={mapsApiKey}&components=country:GB|postal_code:";
            httpClient.DefaultRequestHeaders.Accept.Clear();
            client = httpClient;
        }

        public async Task<MapsResponse> GetPostcodeCoordinates(string postcode)
        {
            var response = await client.GetStringAsync(mapsApiBaseUrl + postcode);
            var result = JsonConvert.DeserializeObject<MapsResponse>(response);
            return result;
        }
    }
}
