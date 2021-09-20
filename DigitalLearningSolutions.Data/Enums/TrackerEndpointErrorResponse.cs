namespace DigitalLearningSolutions.Data.Enums
{
    public class TrackerEndpointErrorResponse : Enumeration
    {
        public static readonly TrackerEndpointErrorResponse UnexpectedException =
            new TrackerEndpointErrorResponse(-1, nameof(UnexpectedException));

        public static readonly TrackerEndpointErrorResponse InvalidAction =
            new TrackerEndpointErrorResponse(-2, nameof(InvalidAction));

        public static readonly TrackerEndpointErrorResponse NullAction =
            new TrackerEndpointErrorResponse(-15, nameof(NullAction));

        public TrackerEndpointErrorResponse(int id, string name) : base(id, name) { }

        public static implicit operator string(TrackerEndpointErrorResponse trackerEndpointErrorResponse)
        {
            return $"[#Result:{trackerEndpointErrorResponse.Id}]";
        }
    }
}
