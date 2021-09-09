namespace DigitalLearningSolutions.Data.Enums
{
    using System;

    public class TrackerEndpointErrorResponse : Enumeration
    {
        public static readonly TrackerEndpointErrorResponse UnexpectedException =
            new TrackerEndpointErrorResponse(-1, "UnexpectedException");

        public static readonly TrackerEndpointErrorResponse InvalidAction =
            new TrackerEndpointErrorResponse(-2, "InvalidAction");

        public static readonly TrackerEndpointErrorResponse NullAction =
            new TrackerEndpointErrorResponse(-15, "NullAction");

        public TrackerEndpointErrorResponse(int id, string name) : base(id, name) { }

        public static implicit operator string(TrackerEndpointErrorResponse trackerEndpointErrorResponse) => $"[#Result:{trackerEndpointErrorResponse.Id}]";
    }
}
