namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public interface ILearningLogItemsService
    {
        public LearningLogItem? SelectLearningLogItemById(int learningLogItemId);

        public void SetCompletionDate(int learningLogItemId, DateTime completedDate);
    }

    public class LearningLogItemsService : ILearningLogItemsService
    {
        private readonly ILearningLogItemsDataService learningLogItemsDataService;

        public LearningLogItemsService(
            ILearningLogItemsDataService learningLogItemsDataService
        )
        {
            this.learningLogItemsDataService = learningLogItemsDataService;
        }

        public LearningLogItem? SelectLearningLogItemById(int learningLogItemId)
        {
            return learningLogItemsDataService.SelectLearningLogItemById(learningLogItemId);
        }

        public void SetCompletionDate(int learningLogItemId, DateTime completedDate)
        {
            learningLogItemsDataService.SetCompletionDate(learningLogItemId, completedDate);
        }
    }
}
