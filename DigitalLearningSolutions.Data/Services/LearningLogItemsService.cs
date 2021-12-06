namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;

    public interface ILearningLogItemsService
    {
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

        public void SetCompletionDate(int learningLogItemId, DateTime completedDate)
        {
            learningLogItemsDataService.SetCompletionDate(learningLogItemId, completedDate);
        }
    }
}
