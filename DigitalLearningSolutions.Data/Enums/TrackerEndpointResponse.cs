namespace DigitalLearningSolutions.Data.Enums
{
    public class TrackerEndpointResponse : Enumeration
    {
        public static readonly TrackerEndpointResponse Success =
            new TrackerEndpointResponse(1, nameof(Success));

        public static readonly TrackerEndpointResponse UnexpectedException =
            new TrackerEndpointResponse(-1, nameof(UnexpectedException));

        public static readonly TrackerEndpointResponse InvalidAction =
            new TrackerEndpointResponse(-2, nameof(InvalidAction));

        public static readonly TrackerEndpointResponse NullTutorialStatusOrTime =
            new TrackerEndpointResponse(-14, nameof(NullTutorialStatusOrTime));

        public static readonly TrackerEndpointResponse NullAction =
            new TrackerEndpointResponse(-15, nameof(NullAction));

        public static readonly TrackerEndpointResponse StoreAspProgressException =
            new TrackerEndpointResponse(-24, nameof(StoreAspProgressException));

        public static readonly TrackerEndpointResponse StoreDiagnosticScoreException =
            new TrackerEndpointResponse(-25, nameof(StoreDiagnosticScoreException));

        public TrackerEndpointResponse(int id, string name) : base(id, name) { }

        public static implicit operator string(TrackerEndpointResponse trackerEndpointResponse)
        {
            return $"[#Result:{trackerEndpointResponse.Id}]";
        }
    }
}
