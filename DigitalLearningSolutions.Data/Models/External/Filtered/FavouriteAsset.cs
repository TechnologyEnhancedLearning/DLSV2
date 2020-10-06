namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class FavouriteAsset
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("saved")]
        public bool Saved { get; set; }
    }
}
