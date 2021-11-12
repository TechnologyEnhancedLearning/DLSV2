namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public interface ITrackerService
    {
        string ProcessQuery(TrackerEndpointQueryParams query);
    }

    public class TrackerService : ITrackerService
    {
        private readonly ILogger<TrackerService> logger;
        private readonly ITrackerActionService trackerActionService;
        private readonly JsonSerializerSettings settings;

        public TrackerService(ILogger<TrackerService> logger, ITrackerActionService trackerActionService)
        {
            this.logger = logger;
            this.trackerActionService = trackerActionService;
            settings = new JsonSerializerSettings { ContractResolver = new LowercaseContractResolver() };
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
                    var actionDataResult = action switch
                    {
                        TrackerEndpointAction.GetObjectiveArray => trackerActionService.GetObjectiveArray(
                            query.CustomisationId,
                            query.SectionId
                        ),
                        _ => throw new ArgumentOutOfRangeException(),
                    };

                    return ConvertToJsonString(actionDataResult);
                }

                return TrackerEndpointErrorResponse.InvalidAction;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing {query.Action}");
                return TrackerEndpointErrorResponse.UnexpectedException;
            }
        }

        private string ConvertToJsonString(ITrackerEndpointDataModel? foo)
        {
            if (foo == null)
            {
                return JsonConvert.SerializeObject(new { });
            }

            return JsonConvert.SerializeObject(foo, settings);
        }

        private class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }
    }
}
