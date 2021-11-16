namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem
{
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Services;
    using Microsoft.AspNetCore.Mvc;

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
            return trackerService.ProcessQuery(queryParams);
        }
    }
}
