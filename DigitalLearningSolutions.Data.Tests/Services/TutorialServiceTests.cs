namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class TutorialServiceTests
    {
        private IProgressDataService progressDataService = null!;
        private ITutorialContentDataService tutorialContentDataService = null!;
        private ITutorialService tutorialService = null!;

        [SetUp]
        public void Setup()
        {
            progressDataService = A.Fake<IProgressDataService>();
            tutorialContentDataService = A.Fake<ITutorialContentDataService>();

            tutorialService = new TutorialService(
                tutorialContentDataService,
                progressDataService
            );
        }

        [Test]
        public void UpdateTutorialsStatuses_calls_data_services_correct_number_of_times()
        {
            // Given
            var tutorialOne = new Tutorial(1, "Test", true, true);
            var tutorialTwo = new Tutorial(2, "Case", false, false);
            var tutorials = new List<Tutorial> { tutorialOne, tutorialTwo };
            A.CallTo(
                    () => tutorialContentDataService.UpdateOrInsertCustomisationTutorialStatuses(
                        A<int>._,
                        A<int>._,
                        A<bool>._,
                        A<bool>._
                    )
                )
                .DoesNothing();
            A.CallTo(() => progressDataService.InsertNewAspProgressRecordsForTutorialIfNoneExist(A<int>._, A<int>._))
                .DoesNothing();

            // When
            tutorialService.UpdateTutorialsStatuses(tutorials, 1);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => tutorialContentDataService.UpdateOrInsertCustomisationTutorialStatuses(
                            A<int>._,
                            A<int>._,
                            A<bool>._,
                            A<bool>._
                        )
                    )
                    .MustHaveHappened(2, Times.Exactly);
                A.CallTo(() => progressDataService.InsertNewAspProgressRecordsForTutorialIfNoneExist(A<int>._, A<int>._))
                    .MustHaveHappened(2, Times.Exactly);
            }
        }

        [Test]
        public void GetTutorialsForSection_calls_data_service()
        {
            // Given
            var tutorialOne = new Tutorial(1, "Test", true, true);
            var tutorialTwo = new Tutorial(2, "Case", false, false);
            var tutorials = new List<Tutorial> { tutorialOne, tutorialTwo };

            A.CallTo(
                () => tutorialContentDataService.GetTutorialsForSection(1)
            ).Returns(tutorials);

            // When
            var result = tutorialService.GetTutorialsForSection(1);

            // Then
            A.CallTo(() => tutorialContentDataService.GetTutorialsForSection(1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeEquivalentTo(tutorials);
        }
    }
}
