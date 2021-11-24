namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class TrackerActionServiceTests
    {
        private ITutorialContentDataService dataService = null!;
        private ITrackerActionService trackerActionService = null!;

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
            A.CallTo(() => dataService.GetNonArchivedObjectivesBySectionAndCustomisationId(A<int>._, A<int>._))
                .Returns(
                    new[]
                    {
                        new Objective(1, new List<int> { 6, 7, 8 }, 4),
                        new Objective(2, new List<int> { 17, 18, 19 }, 0),
                    }
                );

            // when
            var result = trackerActionService.GetObjectiveArray(1, 1);

            // then
            result.Should().BeEquivalentTo(
                new TrackerObjectiveArray(
                    new[]
                    {
                        new Objective(1, new List<int> { 6, 7, 8 }, 4),
                        new Objective(2, new List<int> { 17, 18, 19 }, 0),
                    }
                )
            );
        }

        [Test]
        public void GetObjectiveArray_returns_empty_object_json_if_no_results_found()
        {
            // given
            A.CallTo(() => dataService.GetNonArchivedObjectivesBySectionAndCustomisationId(A<int>._, A<int>._))
                .Returns(new List<Objective>());

            // when
            var result = trackerActionService.GetObjectiveArray(1, 1);

            // then
            result.Should().Be(null);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase(1, null)]
        [TestCase(null, 1)]
        public void GetObjectiveArray_returns_null_if_parameter_missing(
            int? customisationId,
            int? sectionId
        )
        {
            // given
            A.CallTo(() => dataService.GetNonArchivedObjectivesBySectionAndCustomisationId(A<int>._, A<int>._))
                .Returns(new[] { new Objective(1, new List<int> { 1 }, 9) });

            // when
            var result = trackerActionService.GetObjectiveArray(customisationId, sectionId);

            // then
            result.Should().Be(null);
        }
    }
}
