using System.Collections.Generic;

namespace DigitalLearningSolutions.Data.Models.Tracker
{
    public class GetObjectiveArrayData : ITrackerEndpointDataModel
    {
        public GetObjectiveArrayData(IEnumerable<Objective> objectives)
        {
            Objectives = objectives;
        }

        public IEnumerable<Objective> Objectives { get; set; }
    }
}
