namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using FakeItEasy;
    using NUnit.Framework;

    public class LearningLogItemsServiceTests
    {
        private ILearningLogItemsDataService learningLogItemsDataService = null!;

        [SetUp]
        public void Setup()
        {
            learningLogItemsDataService = A.Fake<ILearningLogItemsDataService>();
        }

        [Test]
        public void UpdateCompletionDate_calls_data_service()
        {
            // Given
            const int learningLogItemId = 1;
            var completedDate = new DateTime(2021, 09, 01);

            // When
            learningLogItemsDataService.SetCompletionDate(learningLogItemId, completedDate);

            // Then
            A.CallTo(() => learningLogItemsDataService.SetCompletionDate(learningLogItemId, completedDate))
                .MustHaveHappened();
        }
    }
}
