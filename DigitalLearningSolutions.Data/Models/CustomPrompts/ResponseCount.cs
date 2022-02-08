namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    public class ResponseCount
    {
        public ResponseCount(string response, int count)
        {
            Response = response;
            Count = count;
        }

        public string Response { get; set; }

        public int Count { get; set; }
    }
}
