namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    public class ResponseCounts
    {
        public ResponseCounts(string response, int count)
        {
            Response = response;
            Count = count;
        }

        public string Response { get; set; }

        public int Count { get; set; }
    }
}
