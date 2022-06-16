namespace DigitalLearningSolutions.Data.Enums
{
    public class TrackerEndpointSessionVariable : Enumeration
    {
        public static readonly TrackerEndpointSessionVariable LmGvSectionRow =
            new TrackerEndpointSessionVariable(0, "lmGvSectionRow");

        public static readonly TrackerEndpointSessionVariable LmSessionId =
            new TrackerEndpointSessionVariable(1, "lmSessionID");

        public TrackerEndpointSessionVariable(int id, string name) : base(id, name) { }
    }
}
