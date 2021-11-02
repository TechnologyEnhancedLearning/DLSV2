namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class TrackerActionServiceTests
    {
        private ITrackerActionService trackerActionService = null!;

        private ITutorialContentDataService dataService = null!;

        private const string EmptyObjectJson = "{}";

        [SetUp]
        public void Setup()
        {
            dataService = A.Fake<ITutorialContentDataService>();

            trackerActionService = new TrackerActionService(dataService);
        }

        [Test]
        public void GetObjectiveArray_returns_results_in_specified_json_format()
        {
            // given
            A.CallTo(() => dataService.GetObjectivesBySectionId(A<int>._, A<int>._))
                .Returns(new[] { new Objective(1, "6,7,8", 4), new Objective(2, "17,18,19", 0) });
            var expectedJson =
                "{\"objectives\":[{\"tutorialid\":1,\"interactions\":[6,7,8],\"possible\":4,\"myscore\":0}," +
                "{\"tutorialid\":2,\"interactions\":[17,18,19],\"possible\":0,\"myscore\":0}]}";

            // when
            var result = trackerActionService.GetObjectiveArray(1, 1);

            // then
            result.Should().Be(expectedJson);
        }

        [Test]
        public void GetObjectiveArray_returns_empty_object_json_if_no_results_found()
        {
            // given
            A.CallTo(() => dataService.GetObjectivesBySectionId(A<int>._, A<int>._))
                .Returns(new List<Objective>());

            // when
            var result = trackerActionService.GetObjectiveArray(1, 1);

            // then
            result.Should().Be(EmptyObjectJson);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase(1, null)]
        [TestCase(null, 1)]
        public void GetObjectiveArray_returns_empty_object_json_if_parameter_missing(int? customisationId, int? sectionId)
        {
            // given
            A.CallTo(() => dataService.GetObjectivesBySectionId(A<int>._, A<int>._))
                .Returns(new[] { new Objective(1, "1", 9) });

            // when
            var result = trackerActionService.GetObjectiveArray(customisationId, sectionId);

            // then
            result.Should().Be(EmptyObjectJson);
        }
    }
}
