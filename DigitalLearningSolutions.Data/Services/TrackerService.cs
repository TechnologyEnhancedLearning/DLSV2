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

        public TrackerService(ILogger<TrackerService> logger)
        {
            this.logger = logger;
        }

        public string ProcessQuery(TrackerEndpointQueryParams query)
        {
            if (string.IsNullOrWhiteSpace(query.Action))
            {
                return TrackerEndpointErrorResponse.NullAction;
            }
            
            try
            {
                return query.Action! switch
                {
                    _ => TrackerEndpointErrorResponse.InvalidAction
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing {query.Action}");
                return TrackerEndpointErrorResponse.UnexpectedException;
            }
        }
    }
}
