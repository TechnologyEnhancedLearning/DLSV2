using System.Collections.Generic;

namespace DigitalLearningSolutions.Data.Models.Tracker
{
    public class TrackerObjectiveArray : ITrackerEndpointDataModel
    {
        public TrackerObjectiveArray(IEnumerable<Objective> objectives)
        {
            Objectives = objectives;
        }

        public IEnumerable<Objective> Objectives { get; set; }
    }
}
