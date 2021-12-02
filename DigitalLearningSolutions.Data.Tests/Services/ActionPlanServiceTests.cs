namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using NUnit.Framework;

    public class ActionPlanServiceTests
    {
        private IActionPlanService actionPlanService = null!;
        private IClockService clockService = null!;
        private ICompetencyLearningResourcesDataService competencyLearningResourcesDataService = null!;
        private ILearningHubApiService learningHubApiService = null!;
        private ILearningLogItemsDataService learningLogItemsDataService = null!;
        private ISelfAssessmentDataService selfAssessmentDataService = null!;

        [SetUp]
        public void Setup()
        {
            clockService = A.Fake<IClockService>();
            competencyLearningResourcesDataService = A.Fake<ICompetencyLearningResourcesDataService>();
            learningLogItemsDataService = A.Fake<ILearningLogItemsDataService>();
            learningHubApiService = A.Fake<ILearningHubApiService>();
            selfAssessmentDataService = A.Fake<ISelfAssessmentDataService>();

            actionPlanService = new ActionPlanService(
                competencyLearningResourcesDataService,
                learningLogItemsDataService,
                clockService,
                learningHubApiService,
                selfAssessmentDataService
            );
        }

        [Test]
        public void AddResourceToActionPlan_calls_expected_insert_data_service_methods()
        {
            // Given
            const int competencyLearningResourceId = 1;
            const int delegateId = 2;
            const int selfAssessmentId = 3;
            const string resourceName = "Activity";
            const string resourceLink = "www.test.com";
            const int learningLogId = 4;
            const int learningHubResourceId = 6;

            var addedDate = new DateTime(2021, 11, 1);
            A.CallTo(() => clockService.UtcNow).Returns(addedDate);

            var competencyLearningResource = new CompetencyLearningResource
            {
                LearningHubResourceReferenceId = learningHubResourceId,
            };
            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyLearningResourceById(
                    competencyLearningResourceId
                )
            ).Returns(competencyLearningResource);

            A.CallTo(() => learningHubApiService.GetResourceNameAndLink(learningHubResourceId))
                .Returns((resourceName, resourceLink));

            var resourceCompetencies = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            A.CallTo(
                () => competencyLearningResourcesDataService.GetCompetencyIdsByLearningHubResourceReference(
                    learningHubResourceId
                )
            ).Returns(resourceCompetencies);

            var assessmentCompetencies = new[] { 2, 3, 5, 6, 8, 9, 10 };
            A.CallTo(() => selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(selfAssessmentId))
                .Returns(assessmentCompetencies);

            A.CallTo(
                () => learningLogItemsDataService.InsertLearningLogItem(
                    A<int>._,
                    A<DateTime>._,
                    A<string>._,
                    A<string>._,
                    A<int>._
                )
            ).Returns(learningLogId);

            var expectedMatchingCompetencies = new[] { 2, 3, 5, 6, 8 };

            // When
            actionPlanService.AddResourceToActionPlan(competencyLearningResourceId, delegateId, selfAssessmentId);

            // Then
            A.CallTo(
                () => learningLogItemsDataService.InsertLearningLogItem(
                    delegateId,
                    addedDate,
                    resourceName,
                    resourceLink,
                    competencyLearningResourceId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => learningLogItemsDataService.InsertCandidateAssessmentLearningLogItem(
                    selfAssessmentId,
                    learningLogId
                )
            ).MustHaveHappenedOnceExactly();

            foreach (var competencyId in expectedMatchingCompetencies)
            {
                A.CallTo(
                    () => learningLogItemsDataService.InsertLearningLogItemCompetencies(
                        learningLogId,
                        competencyId,
                        addedDate
                    )
                ).MustHaveHappenedOnceExactly();
            }

            A.CallTo(
                () => learningLogItemsDataService.InsertLearningLogItemCompetencies(learningLogId, A<int>._, addedDate)
            ).MustHaveHappened(5, Times.Exactly);
        }
    }
}
