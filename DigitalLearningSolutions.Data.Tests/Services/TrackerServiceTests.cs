namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class TrackerServiceTests
    {
        private ILogger<TrackerService> logger = null!;
        private ITrackerActionService actionService = null!;
        private ITrackerService trackerService = null!;

        [SetUp]
        public void Setup()
        {
            logger =  A.Fake<ILogger<TrackerService>>();
            actionService = A.Fake<ITrackerActionService>();

            trackerService = new TrackerService(logger, actionService);
        }

        [Test]
        public void ProcessQuery_with_null_action_returns_NullAction_response()
        {
            // Given
            var query = new TrackerEndpointQueryParams { Action = null };

            // When
            var result = trackerService.ProcessQuery(query);

            // Then
            result.Should().Be(TrackerEndpointErrorResponse.NullAction);
        }

        [Test]
        public void ProcessQuery_with_unknown_action_returns_InvalidAction_response()
        {
            // Given
            var query = new TrackerEndpointQueryParams{Action = "InvalidAction"};

            // When
            var result = trackerService.ProcessQuery(query);

            // Then
            result.Should().Be(TrackerEndpointErrorResponse.InvalidAction);
        }

        [Test]
        public void
            ProcessQuery_with_GetObjectiveArray_action_passes_query_params_and_returns_appropriate_service_response()
        {
            // Given
            var query = new TrackerEndpointQueryParams { Action = "GetObjectiveArray", CustomisationId = 1, SectionId = 1 };
            A.CallTo(() => actionService.GetObjectiveArray(1, 1)).Returns("GOA result");

            // When
            var result = trackerService.ProcessQuery(query);

            // Then
            result.Should().Be("GOA result");
        }
    }
}
