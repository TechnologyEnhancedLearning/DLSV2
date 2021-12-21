namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public interface ITrackerActionService
    {
        TrackerObjectiveArray? GetObjectiveArray(int? customisationId, int? sectionId);

        TrackerObjectiveArrayCc? GetObjectiveArrayCc(int? customisationId, int? sectionId, bool? isPostLearning);

        public TrackerEndpointResponse? StoreDiagnosticJson(int? progressId, string? diagnosticOutcome);
    }

    public class TrackerActionService : ITrackerActionService
    {
        private readonly ILogger<TrackerActionService> logger;
        private readonly IProgressService progressService;
        private readonly ITutorialContentDataService tutorialContentDataService;

        public TrackerActionService(
            ITutorialContentDataService tutorialContentDataService,
            IProgressService progressService,
            ILogger<TrackerActionService> logger
        )
        {
            this.tutorialContentDataService = tutorialContentDataService;
            this.progressService = progressService;
            this.logger = logger;
        }

        public TrackerObjectiveArray? GetObjectiveArray(int? customisationId, int? sectionId)
        {
            if (!customisationId.HasValue || !sectionId.HasValue)
            {
                return null;
            }

            var objectives = tutorialContentDataService
                .GetNonArchivedObjectivesBySectionAndCustomisationId(sectionId.Value, customisationId.Value)
                .ToList();

            return objectives.Any() ? new TrackerObjectiveArray(objectives) : null;
        }

        public TrackerObjectiveArrayCc? GetObjectiveArrayCc(int? customisationId, int? sectionId, bool? isPostLearning)
        {
            if (!customisationId.HasValue || !sectionId.HasValue || !isPostLearning.HasValue)
            {
                return null;
            }

            var ccObjectives = tutorialContentDataService
                .GetNonArchivedCcObjectivesBySectionAndCustomisationId(
                    sectionId.Value,
                    customisationId.Value,
                    isPostLearning.Value
                )
                .ToList();

            return ccObjectives.Any() ? new TrackerObjectiveArrayCc(ccObjectives) : null;
        }

        public TrackerEndpointResponse? StoreDiagnosticJson(int? progressId, string? diagnosticOutcome)
        {
            if (progressId == null || diagnosticOutcome == null)
            {
                return TrackerEndpointResponse.StoreDiagnosticScoreException;
            }

            var json = JsonConvert.DeserializeObject<IEnumerable<JObject>>(diagnosticOutcome);

            try
            {
                foreach (var obj in json)
                {
                    var tutorialId = (int)obj["tutorialId"];
                    var myScore = (int)obj["myscore"];
                    progressService.UpdateDiagnosticScore(progressId.Value, tutorialId, myScore);
                }
            }
            catch (Exception e)
            {
                logger.LogWarning(
                    $"Updating diagnostic score failed. Error: {e}"
                );
                return TrackerEndpointResponse.StoreDiagnosticScoreException;
            }

            return TrackerEndpointResponse.Success;
        }
    }
}
