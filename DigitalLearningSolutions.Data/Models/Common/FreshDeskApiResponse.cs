namespace DigitalLearningSolutions.Data.Models.Common
{
    public class FreshDeskApiResponse : GenericApiResponse
    {
        public long? TicketId { get; set; }
        public string? FullErrorDetails { get; set; }
    }
}
