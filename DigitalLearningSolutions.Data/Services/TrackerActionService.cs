namespace DigitalLearningSolutions.Data.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public interface ITrackerActionService
    {
        string GetObjectiveArray(int? customisationId, int? sectionId);
    }

    public class TrackerActionService : ITrackerActionService
    {
        private readonly JsonSerializerSettings settings;
        private readonly ITutorialContentDataService tutorialContentDataService;

        public TrackerActionService(ITutorialContentDataService tutorialContentDataService)
        {
            this.tutorialContentDataService = tutorialContentDataService;
            settings = new JsonSerializerSettings { ContractResolver = new LowercaseContractResolver() };
        }

        public string GetObjectiveArray(int? customisationId, int? sectionId)
        {
            if (!customisationId.HasValue || !sectionId.HasValue)
            {
                return JsonConvert.SerializeObject(new { });
            }

            var objectives = tutorialContentDataService.GetObjectivesBySectionId(sectionId.Value, customisationId.Value)
                .ToList();

            object objectiveArrayObject = objectives.Any() ? (object)new { objectives } : new { };

            return JsonConvert.SerializeObject(objectiveArrayObject, settings);
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
