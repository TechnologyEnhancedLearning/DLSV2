namespace DigitalLearningSolutions.Web.Helpers.ExternalApis
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.External.Maps;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public interface IMapsApiHelper
    {
        Task<MapsResponse> GeocodePostcode(string postcode);
    }

    public class MapsApiHelper : IMapsApiHelper
    {
        private readonly HttpClient client;
        private readonly string mapsApiBaseUrl;

        public MapsApiHelper(HttpClient httpClient, IConfiguration config)
        {
            var mapsApiKey = config.GetMapsApiKey();
            mapsApiBaseUrl =
                $"https://maps.googleapis.com/maps/api/geocode/json?key={mapsApiKey}&components=country:GB|postal_code:";

            client = httpClient;
        }

        public async Task<MapsResponse> GeocodePostcode(string postcode)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            var response = await client.GetStringAsync(mapsApiBaseUrl + postcode);
            var result = JsonConvert.DeserializeObject<MapsResponse>(response);
            return result;
        }
    }
}
