namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class StoreAspProgressServiceTests
    {
        private const int DefaultProgressId = 101;
        private const int DefaultCustomisationVersion = 1;
        private const string? DefaultLmGvSectionRow = "Test";
        private const int DefaultTutorialId = 123;
        private const int DefaultTutorialTime = 2;
        private const int DefaultTutorialStatus = 3;
        private const int DefaultDelegateId = 4;
        private const int DefaultCustomisationId = 5;
        public const int DefaultSessionId = 312;

        private readonly DetailedCourseProgress defaultCourseProgress =
            ProgressTestHelper.GetDefaultDetailedCourseProgress(
                DefaultProgressId,
                DefaultDelegateId,
                DefaultCustomisationId
            );

        private readonly Session defaultSession = SessionTestHelper.CreateDefaultSession(
            DefaultSessionId,
            DefaultDelegateId,
            DefaultCustomisationId
        );

        private IProgressService progressService = null!;
        private ISessionDataService sessionDataService = null!;
        private IStoreAspProgressService storeAspProgressService = null!;

        [SetUp]
        public void Setup()
        {
            progressService = A.Fake<IProgressService>();
            sessionDataService = A.Fake<ISessionDataService>();
            storeAspProgressService = new StoreAspProgressService(progressService, sessionDataService);
        }

        [TestCase(null, 1, 123, 456, 789)]
        [TestCase(101, null, 123, 456, 789)]
        [TestCase(101, 1, null, 456, 789)]
        [TestCase(101, 1, 123, null, 789)]
        [TestCase(101, 1, 123, 456, null)]
        public void
            GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints_returns_StoreAspProgressException_if_a_query_param_is_null(
                int? progressId,
                int? version,
                int? tutorialId,
                int? delegateId,
                int? customisationId
            )
        {
            // When
            var result = storeAspProgressService.GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                progressId,
                version,
                tutorialId,
                1,
                1,
                delegateId,
                customisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
                result.progress.Should().BeNull();
                A.CallTo(() => progressService.GetDetailedCourseProgress(A<int>._)).MustNotHaveHappened();
            }
        }

        [TestCase(null, 1)]
        [TestCase(1, null)]
        public void
            GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints_returns_NullTutorialStatusOrTime_if_tutorialTime_or_tutorialStatus_is_null(
                int? tutorialTime,
                int? tutorialStatus
            )
        {
            // When
            var result = storeAspProgressService.GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                101,
                1,
                123,
                tutorialTime,
                tutorialStatus,
                456,
                789
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.NullTutorialStatusOrTime);
                result.progress.Should().BeNull();
                A.CallTo(() => progressService.GetDetailedCourseProgress(A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void
            GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints_returns_StoreAspProgressException_if_progress_is_null()
        {
            // Given
            A.CallTo(
                () => progressService.GetDetailedCourseProgress(DefaultProgressId)
            ).Returns(null);

            // When
            var result = storeAspProgressService.GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                DefaultProgressId,
                1,
                123,
                1,
                1,
                456,
                789
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
                result.progress.Should().BeNull();
                A.CallTo(() => progressService.GetDetailedCourseProgress(DefaultProgressId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [TestCase(100, DefaultCustomisationId)]
        [TestCase(DefaultDelegateId, 100)]
        public void
            GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints_returns_StoreAspProgressException_if_a_param_does_not_match_progress_record(
                int delegateId,
                int customisationId
            )
        {
            // Given
            ProgressServiceReturnsDefaultDetailedCourseProgress();

            // When
            var result = storeAspProgressService.GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                delegateId,
                customisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
                result.progress.Should().BeNull();
                A.CallTo(() => progressService.GetDetailedCourseProgress(DefaultProgressId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints_returns_progress_record_and_null_exception_when_all_is_valid()
        {
            // Given
            ProgressServiceReturnsDefaultDetailedCourseProgress();

            // When
            var result = storeAspProgressService.GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().BeNull();
                result.progress.Should().Be(defaultCourseProgress);
                A.CallTo(() => progressService.GetDetailedCourseProgress(DefaultProgressId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [TestCase(null)]
        [TestCase("non integer value")]
        [TestCase("12.456")]
        public void
            ParseSessionIdAndValidateSessionForStoreAspProgressNoSession_returns_exception_when_session_ID_is_not_integer(
                string? sessionId
            )
        {
            // When
            var result = storeAspProgressService.ParseSessionIdAndValidateSessionForStoreAspProgressNoSession(
                sessionId,
                DefaultDelegateId,
                DefaultCustomisationVersion
            );

            // Then
            using (new AssertionScope())
            {
                result.parsedSessionId.Should().BeNull();
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
                A.CallTo(() => sessionDataService.GetSessionById(A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void
            ParseSessionIdAndValidateSessionForStoreAspProgressNoSession_returns_exception_when_session_is_null()
        {
            // Given
            A.CallTo(() => sessionDataService.GetSessionById(DefaultSessionId))
                .Returns(null);

            // When
            var result = storeAspProgressService.ParseSessionIdAndValidateSessionForStoreAspProgressNoSession(
                DefaultSessionId.ToString(),
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.parsedSessionId.Should().BeNull();
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
                A.CallTo(() => sessionDataService.GetSessionById(DefaultSessionId)).MustHaveHappenedOnceExactly();
            }
        }

        [TestCase(DefaultCustomisationId + 1, DefaultDelegateId, true)]
        [TestCase(DefaultCustomisationId, DefaultDelegateId + 1, true)]
        [TestCase(DefaultCustomisationId, DefaultDelegateId, false)]
        public void
            ParseSessionIdAndValidateSessionForStoreAspProgressNoSession_returns_exception_when_session_details_do_not_match_necessary_requirements(
                int customisationId,
                int delegateId,
                bool sessionActive
            )
        {
            // Given
            A.CallTo(() => sessionDataService.GetSessionById(DefaultSessionId))
                .Returns(new Session(DefaultSessionId, delegateId, customisationId, DateTime.UtcNow, 1, sessionActive));

            // When
            var result = storeAspProgressService.ParseSessionIdAndValidateSessionForStoreAspProgressNoSession(
                DefaultSessionId.ToString(),
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.parsedSessionId.Should().BeNull();
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
                A.CallTo(() => sessionDataService.GetSessionById(DefaultSessionId)).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            ParseSessionIdAndValidateSessionForStoreAspProgressNoSession_returns_parsed_session_ID_and_null_exception_when_session_is_valid()
        {
            // Given
            SessionServiceReturnsDefaultSession();

            // When
            var result = storeAspProgressService.ParseSessionIdAndValidateSessionForStoreAspProgressNoSession(
                DefaultSessionId.ToString(),
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.parsedSessionId.Should().Be(DefaultSessionId);
                result.validationResponse.Should().Be(null);
                A.CallTo(() => sessionDataService.GetSessionById(DefaultSessionId)).MustHaveHappenedOnceExactly();
            }
        }

        [TestCase(1)]
        [TestCase(3)]
        public void
            StoreAspProgressAndSendEmailIfComplete_stores_AspProgress_and_does_not_CheckProgressForCompletion_if_TutorialStatus_is_not_2(
                int tutorialStatus
            )
        {
            // When
            storeAspProgressService.StoreAspProgressAndSendEmailIfComplete(
                defaultCourseProgress,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                tutorialStatus
            );

            // Then
            A.CallTo(
                () => progressService.StoreAspProgressV2(
                    DefaultProgressId,
                    DefaultCustomisationVersion,
                    DefaultLmGvSectionRow,
                    DefaultTutorialId,
                    DefaultTutorialTime,
                    tutorialStatus
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => progressService.CheckProgressForCompletionAndSendEmailIfCompleted(A<DetailedCourseProgress>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void
            StoreAspProgressAndSendEmailIfComplete_stores_AspProgress_and_calls_CheckProgressForCompletion_if_TutorialStatus_is_2()
        {
            // Given
            const int tutorialStatus = 2;

            // When
            storeAspProgressService.StoreAspProgressAndSendEmailIfComplete(
                defaultCourseProgress,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                tutorialStatus
            );

            // Then
            A.CallTo(
                () => progressService.StoreAspProgressV2(
                    DefaultProgressId,
                    DefaultCustomisationVersion,
                    DefaultLmGvSectionRow,
                    DefaultTutorialId,
                    DefaultTutorialTime,
                    tutorialStatus
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => progressService.CheckProgressForCompletionAndSendEmailIfCompleted(defaultCourseProgress)
            ).MustHaveHappenedOnceExactly();
        }

        private void ProgressServiceReturnsDefaultDetailedCourseProgress()
        {
            A.CallTo(
                () => progressService.GetDetailedCourseProgress(DefaultProgressId)
            ).Returns(defaultCourseProgress);
        }

        private void SessionServiceReturnsDefaultSession()
        {
            A.CallTo(() => sessionDataService.GetSessionById(DefaultSessionId)).Returns(defaultSession);
        }
    }
}
