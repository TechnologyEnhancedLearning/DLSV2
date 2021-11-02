namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using Microsoft.Extensions.Logging;

    public interface ITrackerService
    {
        string ProcessQuery(TrackerEndpointQueryParams query);
    }

    public class TrackerService : ITrackerService
    {
        private readonly ILogger<TrackerService> logger;
        private readonly ITrackerActionService trackerActionService;

        public TrackerService(ILogger<TrackerService> logger, ITrackerActionService trackerActionService)
        {
            this.logger = logger;
            this.trackerActionService = trackerActionService;
        }

        public string ProcessQuery(TrackerEndpointQueryParams query)
        {
            if (string.IsNullOrWhiteSpace(query.Action))
            {
                return TrackerEndpointErrorResponse.NullAction;
            }

            try
            {
                if (Enum.TryParse<TrackerEndpointAction>(query.Action, true, out var action))
                {
                    return action switch
                    {
                        TrackerEndpointAction.GetObjectiveArray => trackerActionService.GetObjectiveArray(query.CustomisationId, query.SectionId),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }

                return TrackerEndpointErrorResponse.InvalidAction;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing {query.Action}");
                return TrackerEndpointErrorResponse.UnexpectedException;
            }
        }
    }
}
