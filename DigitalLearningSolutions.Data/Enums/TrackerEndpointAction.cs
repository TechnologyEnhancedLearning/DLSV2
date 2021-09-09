namespace DigitalLearningSolutions.Data.Enums
{
    using System;

    public class TrackerEndpointAction : Enumeration
    {
        public TrackerEndpointAction(int id, string name) : base(id, name) { }

        public static implicit operator TrackerEndpointAction(string value)
        {
            try
            {
                return FromName<TrackerEndpointAction>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public static implicit operator string(TrackerEndpointAction trackerEndpointAction) => trackerEndpointAction.Name;
    }
}
