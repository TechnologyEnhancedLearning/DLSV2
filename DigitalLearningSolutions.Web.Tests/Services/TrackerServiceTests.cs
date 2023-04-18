namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class TrackerServiceTests
    {
        private const string DefaultProgressText = "Test progress text";
        private const string DefaultSessionId = "312";

        private readonly TrackerEndpointQueryParams defaultStoreAspAssessQueryParams = new TrackerEndpointQueryParams
        {
            Version = 1,
            CandidateId = 456,
            CustomisationId = 1,
            Score = 1,
            SectionId = 3,
        };

        private readonly TrackerEndpointQueryParams defaultStoreAspProgressQueryParams = new TrackerEndpointQueryParams
        {
            ProgressId = 101,
            Version = 1,
            TutorialId = 123,
            TutorialTime = 2,
            TutorialStatus = 3,
            CandidateId = 456,
            CustomisationId = 1,
        };

        private readonly Dictionary<TrackerEndpointSessionVariable, string?> emptySessionVariablesDictionary =
            new Dictionary<TrackerEndpointSessionVariable, string?>
                { { TrackerEndpointSessionVariable.LmGvSectionRow, null } };

        private readonly Dictionary<TrackerEndpointSessionVariable, string?>
            sessionVariablesForStoreAspProgressNoSession =
                new Dictionary<TrackerEndpointSessionVariable, string?>
                {
                    { TrackerEndpointSessionVariable.LmGvSectionRow, DefaultProgressText },
                    { TrackerEndpointSessionVariable.LmSessionId, DefaultSessionId },
                };

        private readonly Dictionary<TrackerEndpointSessionVariable, string?> sessionVariablesForStoreAspProgressV2 =
            new Dictionary<TrackerEndpointSessionVariable, string?>
            {
                { TrackerEndpointSessionVariable.LmGvSectionRow, DefaultProgressText },
            };

        private ITrackerActionService actionService = null!;
        private ILogger<TrackerService> logger = null!;
        private ITrackerService trackerService = null!;

        [SetUp]
        public void Setup()
        {
            logger = A.Fake<ILogger<TrackerService>>();
            actionService = A.Fake<ITrackerActionService>();

            trackerService = new TrackerService(logger, actionService);
        }

        [Test]
        public void ProcessQuery_with_null_action_returns_NullAction_response()
        {
            // Given
            var query = new TrackerEndpointQueryParams { Action = null };

            // When
            var result = trackerService.ProcessQuery(query, emptySessionVariablesDictionary);

            // Then
            result.Should().Be(TrackerEndpointResponse.NullAction);
        }

        [Test]
        public void ProcessQuery_with_unknown_action_returns_InvalidAction_response()
        {
            // Given
            var query = new TrackerEndpointQueryParams { Action = "InvalidAction" };

            // When
            var result = trackerService.ProcessQuery(query, emptySessionVariablesDictionary);

            // Then
            result.Should().Be(TrackerEndpointResponse.InvalidAction);
        }

        [Test]
        public void ProcessQuery_with_GetObjectiveArray_action_passes_query_params()
        {
            // Given
            var query = new TrackerEndpointQueryParams
            { Action = "GetObjectiveArray", CustomisationId = 1, SectionId = 2 };

            // When
            trackerService.ProcessQuery(query, emptySessionVariablesDictionary);

            // Then
            A.CallTo(() => actionService.GetObjectiveArray(1, 2)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void ProcessQuery_with_valid_action_correctly_serialises_contentful_response()
        {
            // Given
            var dataToReturn = new TrackerObjectiveArray(
                new[]
                {
                    new Objective(1, new List<int> { 6, 7, 8 }, 4), new Objective(2, new List<int> { 17, 18, 19 }, 0),
                }
            );
            var expectedJson =
                "{\"objectives\":[{\"interactions\":[6,7,8],\"tutorialid\":1,\"possible\":4,\"myscore\":0}," +
                "{\"interactions\":[17,18,19],\"tutorialid\":2,\"possible\":0,\"myscore\":0}]}";

            var query = new TrackerEndpointQueryParams
            { Action = "GetObjectiveArray", CustomisationId = 1, SectionId = 1 };
            A.CallTo(() => actionService.GetObjectiveArray(1, 1)).Returns(dataToReturn);

            // When
            var result = trackerService.ProcessQuery(query, emptySessionVariablesDictionary);

            // Then
            result.Should().Be(expectedJson);
        }

        [Test]
        public void ProcessQuery_with_valid_action_correctly_serialises_null_response()
        {
            // Given
            TrackerObjectiveArray? dataToReturn = null;

            var query = new TrackerEndpointQueryParams
            { Action = "GetObjectiveArray", CustomisationId = 1, SectionId = 1 };
            A.CallTo(() => actionService.GetObjectiveArray(1, 1)).Returns(dataToReturn);

            // When
            var result = trackerService.ProcessQuery(query, emptySessionVariablesDictionary);

            // Then
            result.Should().Be("{}");
        }

        [Test]
        [TestCase("true", true)]
        [TestCase("True", true)]
        [TestCase("false", false)]
        [TestCase("False", false)]
        [TestCase("1", true)]
        [TestCase("0", false)]
        [TestCase("mu", null)]
        [TestCase(null, null)]
        public void ProcessQuery_with_GetObjectiveArrayCc_action_passes_query_params_and_parses_IsPostLearning(
            string? isPostLearningValue,
            bool? expectedBool
        )
        {
            // Given
            var query = new TrackerEndpointQueryParams
            {
                Action = "GetObjectiveArrayCc",
                CustomisationId = 1,
                SectionId = 2,
                IsPostLearning = isPostLearningValue,
            };

            // When
            trackerService.ProcessQuery(query, emptySessionVariablesDictionary);

            // Then
            A.CallTo(() => actionService.GetObjectiveArrayCc(1, 2, expectedBool))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void ProcessQuery_with_StoreAspProgressV2_action_passes_query_params()
        {
            // Given
            var query = defaultStoreAspProgressQueryParams;
            query.Action = "StoreAspProgressV2";

            var expectedResponse = TrackerEndpointResponse.Success;

            A.CallTo(
                () => actionService.StoreAspProgressV2(
                    A<int>._,
                    A<int>._,
                    A<string>._,
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).Returns(expectedResponse);

            // When
            var result = trackerService.ProcessQuery(query, sessionVariablesForStoreAspProgressV2);

            // Then
            result.Should().Be(expectedResponse);
            A.CallTo(
                    () => actionService.StoreAspProgressV2(
                        query.ProgressId!.Value,
                        query.Version!.Value,
                        DefaultProgressText,
                        query.TutorialId!.Value,
                        query.TutorialTime!.Value,
                        query.TutorialStatus!.Value,
                        query.CandidateId!.Value,
                        query.CustomisationId!.Value
                    )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void ProcessQuery_with_StoreAspProgressNoSession_action_passes_query_params()
        {
            // Given
            var query = defaultStoreAspProgressQueryParams;
            query.Action = "StoreAspProgressNoSession";

            var expectedResponse = TrackerEndpointResponse.Success;

            A.CallTo(
                () => actionService.StoreAspProgressNoSession(
                    A<int>._,
                    A<int>._,
                    A<string>._,
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<string>._
                )
            ).Returns(expectedResponse);

            // When
            var result = trackerService.ProcessQuery(query, sessionVariablesForStoreAspProgressNoSession);

            // Then
            result.Should().Be(expectedResponse);
            A.CallTo(
                    () => actionService.StoreAspProgressNoSession(
                        query.ProgressId!.Value,
                        query.Version!.Value,
                        DefaultProgressText,
                        query.TutorialId!.Value,
                        query.TutorialTime!.Value,
                        query.TutorialStatus!.Value,
                        query.CandidateId!.Value,
                        query.CustomisationId!.Value,
                        DefaultSessionId
                    )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void ProcessQuery_with_StoreAspAssessNoSession_action_passes_query_params()
        {
            // Given
            var query = defaultStoreAspAssessQueryParams;
            query.Action = "StoreAspAssessNoSession";

            var expectedResponse = TrackerEndpointResponse.Success;

            A.CallTo(
                () => actionService.StoreAspAssessNoSession(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<string>._
                )
            ).Returns(expectedResponse);

            // When
            var result = trackerService.ProcessQuery(query, sessionVariablesForStoreAspProgressNoSession);

            // Then
            result.Should().Be(expectedResponse);
            A.CallTo(
                    () => actionService.StoreAspAssessNoSession(
                        query.Version!.Value,
                        query.SectionId!.Value,
                        query.Score!.Value,
                        query.CandidateId!.Value,
                        query.CustomisationId!.Value,
                        DefaultSessionId
                    )
                )
                .MustHaveHappenedOnceExactly();
        }
    }
}
