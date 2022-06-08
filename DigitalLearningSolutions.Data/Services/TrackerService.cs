namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public interface ITrackerService
    {
        string ProcessQuery(
            TrackerEndpointQueryParams query,
            Dictionary<TrackerEndpointSessionVariable, string?> sessionVariables
        );
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

        public string ProcessQuery(
            TrackerEndpointQueryParams query,
            Dictionary<TrackerEndpointSessionVariable, string?> sessionVariables
        )
        {
            if (string.IsNullOrWhiteSpace(query.Action))
            {
                return TrackerEndpointResponse.NullAction;
            }

            try
            {
                if (Enum.TryParse<TrackerEndpointAction>(query.Action, true, out var action))
                {
                    if (action == TrackerEndpointAction.GetObjectiveArray)
                    {
                        var result = trackerActionService.GetObjectiveArray(
                            query.CustomisationId,
                            query.SectionId
                        );
                        return ConvertToJsonString(result);
                    }

                    if (action == TrackerEndpointAction.GetObjectiveArrayCc)
                    {
                        var result = trackerActionService.GetObjectiveArrayCc(
                            query.CustomisationId,
                            query.SectionId,
                            ConvertParamToNullableBoolean(query.IsPostLearning)
                        );
                        return ConvertToJsonString(result);
                    }

                    if (action == TrackerEndpointAction.StoreDiagnosticJson)
                    {
                        return trackerActionService.StoreDiagnosticJson(
                            query.ProgressId,
                            query.DiagnosticOutcome
                        );
                    }

                    if (action == TrackerEndpointAction.StoreAspProgressV2)
                    {
                        return trackerActionService.StoreAspProgressV2(
                            query.ProgressId,
                            query.Version,
                            sessionVariables[TrackerEndpointSessionVariable.LmGvSectionRow],
                            query.TutorialId,
                            query.TutorialTime,
                            query.TutorialStatus,
                            query.CandidateId,
                            query.CustomisationId
                        );
                    }

                    if (action == TrackerEndpointAction.StoreAspProgressNoSession)
                    {
                        return trackerActionService.StoreAspProgressNoSession(
                            query.ProgressId,
                            query.Version,
                            sessionVariables[TrackerEndpointSessionVariable.LmGvSectionRow],
                            query.TutorialId,
                            query.TutorialTime,
                            query.TutorialStatus,
                            query.CandidateId,
                            query.CustomisationId,
                            sessionVariables[TrackerEndpointSessionVariable.LmSessionId]
                        );
                    }

                    throw new ArgumentOutOfRangeException();
                }

                return TrackerEndpointResponse.InvalidAction;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing {query.Action}");
                return TrackerEndpointResponse.UnexpectedException;
            }
        }

        private bool? ConvertParamToNullableBoolean(string? value)
        {
            if (bool.TryParse(value, out var result))
            {
                return result;
            }

            switch (value)
            {
                case "1":
                    return true;
                case "0":
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
