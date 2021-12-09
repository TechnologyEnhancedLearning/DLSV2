namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using static System.Boolean;

    public interface ITrackerService
    {
        string ProcessQuery(TrackerEndpointQueryParams query);
    }

    public class TrackerService : ITrackerService
    {
        private readonly ILogger<TrackerService> logger;

        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
            { ContractResolver = new LowercaseContractResolver() };

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
                    ITrackerEndpointDataModel? actionDataResult = action switch
                    {
                        TrackerEndpointAction.GetObjectiveArray => trackerActionService.GetObjectiveArray(
                            query.CustomisationId,
                            query.SectionId
                        ),
                        TrackerEndpointAction.GetObjectiveArrayCc => trackerActionService.GetObjectiveArrayCc(
                            query.CustomisationId,
                            query.SectionId,
                            ConvertToNullableBoolean(query.IsPostLearning)),
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

        private bool? ConvertToNullableBoolean(string? value)
        {
            if (TryParse(value, out var result))
            {
                return result;
            }

            switch (value)
            {
                case "1":
                    return true;
                case "2":
                    return false;
                default:
                    return null;
            }
        }

        private string ConvertToJsonString(ITrackerEndpointDataModel? data)
        {
            if (data == null)
            {
                return JsonConvert.SerializeObject(new { });
            }

            return JsonConvert.SerializeObject(data, settings);
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
