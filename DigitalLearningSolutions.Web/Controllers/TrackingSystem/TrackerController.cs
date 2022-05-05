namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Route("Tracking/Tracker")]
    public class TrackerController : Controller
    {
        private readonly ITrackerService trackerService;

        public TrackerController(ITrackerService trackerService)
        {
            this.trackerService = trackerService;
        }

        public string Index([FromQuery] TrackerEndpointQueryParams queryParams)
        {
            var sessionVariables = GetSessionVariablesDictionary();
            return trackerService.ProcessQuery(queryParams, sessionVariables);
        }

        private Dictionary<TrackerEndpointSessionVariable, string?> GetSessionVariablesDictionary()
        {
            var sessionVariableKeys = Enumeration.GetAll<TrackerEndpointSessionVariable>();
            return sessionVariableKeys.ToDictionary(
                sessionVariableKey => sessionVariableKey,
                sessionVariableKey => HttpContext.Session.GetString(sessionVariableKey.Name)
            );
        }
    }
}
