﻿namespace DigitalLearningSolutions.Data.Enums
{
    public class TrackerEndpointResponse : Enumeration
    {
        public static readonly TrackerEndpointResponse Success =
            new TrackerEndpointResponse(1, nameof(Success));

        public static readonly TrackerEndpointResponse UnexpectedException =
            new TrackerEndpointResponse(-1, nameof(UnexpectedException));

        public static readonly TrackerEndpointResponse InvalidAction =
            new TrackerEndpointResponse(-2, nameof(InvalidAction));

        public static readonly TrackerEndpointResponse NoRowUpdated =
            new TrackerEndpointResponse(-3, nameof(NoRowUpdated));

        public static readonly TrackerEndpointResponse StoreAspAssessException =
            new TrackerEndpointResponse(-6, nameof(StoreAspAssessException));

        public static readonly TrackerEndpointResponse NullScoreTutorialStatusOrTime =
            new TrackerEndpointResponse(-14, nameof(NullScoreTutorialStatusOrTime));

        public static readonly TrackerEndpointResponse NullAction =
            new TrackerEndpointResponse(-15, nameof(NullAction));

        public static readonly TrackerEndpointResponse StoreAspProgressException =
            new TrackerEndpointResponse(-24, nameof(StoreAspProgressException));

        public static readonly TrackerEndpointResponse StoreDiagnosticScoreException =
            new TrackerEndpointResponse(-25, nameof(StoreDiagnosticScoreException));

        public static readonly TrackerEndpointResponse StoreSuspendDataException =
            new TrackerEndpointResponse(-26, nameof(StoreSuspendDataException));

        public static readonly TrackerEndpointResponse StoreLessonLocationException =
            new TrackerEndpointResponse(-27, nameof(StoreLessonLocationException));

        public TrackerEndpointResponse(int id, string name) : base(id, name) { }

        public static implicit operator string(TrackerEndpointResponse trackerEndpointResponse)
        {
            return $"[#Result:{trackerEndpointResponse.Id}]";
        }
    }
}
