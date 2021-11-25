namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Tracker;

    public interface ITrackerActionService
    {
        TrackerObjectiveArray? GetObjectiveArray(int? customisationId, int? sectionId);

        TrackerObjectiveArrayCc? GetObjectiveArrayCc(int? customisationId, int? sectionId, bool? isPostLearning);
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

        public TrackerObjectiveArrayCc? GetObjectiveArrayCc(int? customisationId, int? sectionId, bool? isPostLearning)
        {

            if (!customisationId.HasValue || !sectionId.HasValue || !isPostLearning.HasValue)
            {
                return null;
            }

            var ccObjectives = tutorialContentDataService
                .GetNonArchivedCcObjectivesBySectionAndCustomisationId(sectionId.Value, customisationId.Value, isPostLearning.Value)
                .ToList();

            return ccObjectives.Any() ? new TrackerObjectiveArrayCc(ccObjectives) : null;
        }
    }
}
