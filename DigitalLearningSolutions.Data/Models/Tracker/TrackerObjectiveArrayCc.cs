namespace DigitalLearningSolutions.Data.Models.Tracker
{
    using System.Collections.Generic;

    public class TrackerObjectiveArrayCc : ITrackerEndpointDataModel
    {
        public TrackerObjectiveArrayCc(IEnumerable<CcObjective> objectives)
        {
            Objectives = objectives;
        }

        public IEnumerable<CcObjective> Objectives { get; set; }
    }
}
