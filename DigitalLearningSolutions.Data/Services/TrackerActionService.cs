namespace DigitalLearningSolutions.Data.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public interface ITrackerActionService
    {
        TrackerObjectiveArray? GetObjectiveArray(int? customisationId, int? sectionId);
    }

    public class TrackerActionService : ITrackerActionService
    {
        private readonly ITutorialContentDataService tutorialContentDataService;

        public TrackerActionService(ITutorialContentDataService tutorialContentDataService)
        {
            this.tutorialContentDataService = tutorialContentDataService;
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
    }
}
