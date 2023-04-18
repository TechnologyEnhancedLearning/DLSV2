namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class StoreAspServiceTests
    {
        private const int DefaultProgressId = 101;
        private const int DefaultCustomisationVersion = 1;
        private const string? DefaultLmGvSectionRow = "Test";
        private const int DefaultTutorialId = 123;
        private const int DefaultTutorialTime = 2;
        private const int DefaultTutorialStatus = 3;
        private const int DefaultDelegateId = 4;
        private const int DefaultCustomisationId = 5;
        private const int DefaultSessionId = 312;
        private const int DefaultSectionId = 6;

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

        private ICourseDataService courseDataService = null!;
        private ILogger<StoreAspService> logger = null!;
        private IProgressService progressService = null!;
        private ISessionDataService sessionDataService = null!;
        private IStoreAspService storeAspService = null!;

        [SetUp]
        public void Setup()
        {
            courseDataService = A.Fake<ICourseDataService>();
            progressService = A.Fake<IProgressService>();
            sessionDataService = A.Fake<ISessionDataService>();
            logger = A.Fake<ILogger<StoreAspService>>();
            storeAspService = new StoreAspService(progressService, sessionDataService, courseDataService, logger);
        }

        [TestCase(null, 1, 123, 456, 789)]
        [TestCase(101, null, 123, 456, 789)]
        [TestCase(101, 1, null, 456, 789)]
        [TestCase(101, 1, 123, null, 789)]
        [TestCase(101, 1, 123, 456, null)]
        public void
            GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints_returns_StoreAspProgressException_if_a_query_param_is_null(
                int? progressId,
                int? version,
                int? tutorialId,
                int? delegateId,
                int? customisationId
            )
        {
            // When
            var result = storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
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
            GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints_returns_NullTutorialStatusOrTime_if_tutorialTime_or_tutorialStatus_is_null(
                int? tutorialTime,
                int? tutorialStatus
            )
        {
            // When
            var result = storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
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
                result.validationResponse.Should().Be(TrackerEndpointResponse.NullScoreTutorialStatusOrTime);
                result.progress.Should().BeNull();
                A.CallTo(() => progressService.GetDetailedCourseProgress(A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void
            GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints_returns_StoreAspProgressException_if_progress_is_null()
        {
            // Given
            A.CallTo(
                () => progressService.GetDetailedCourseProgress(DefaultProgressId)
            ).Returns(null);

            // When
            var result = storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
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
            GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints_returns_StoreAspProgressException_if_a_param_does_not_match_progress_record(
                int delegateId,
                int customisationId
            )
        {
            // Given
            ProgressServiceReturnsDefaultDetailedCourseProgress();

            // When
            var result = storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
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
            GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints_returns_progress_record_and_no_exception_when_all_is_valid()
        {
            // Given
            ProgressServiceReturnsDefaultDetailedCourseProgress();

            // When
            var result = storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
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

        [TestCase(null, -6)]
        [TestCase("non integer value", -6)]
        [TestCase("12.456", -6)]
        [TestCase(null, -24)]
        [TestCase("non integer value", -24)]
        [TestCase("12.456", -24)]
        public void
            ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints_returns_specified_exception_when_session_ID_is_not_integer(
                string? sessionId,
                int trackerEndpointResponseId
            )
        {
            // Given
            var suppliedResponse = Enumeration.FromId<TrackerEndpointResponse>(trackerEndpointResponseId);

            // When
            var result = storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                sessionId,
                DefaultDelegateId,
                DefaultCustomisationVersion,
                suppliedResponse
            );

            // Then
            using (new AssertionScope())
            {
                result.parsedSessionId.Should().BeNull();
                result.validationResponse.Should().Be(suppliedResponse);
                A.CallTo(() => sessionDataService.GetSessionById(A<int>._)).MustNotHaveHappened();
            }
        }

        [TestCase(-6)]
        [TestCase(-24)]
        public void
            ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints_returns_specified_exception_when_session_is_null(
                int trackerEndpointResponseId
            )
        {
            // Given
            var suppliedResponse = Enumeration.FromId<TrackerEndpointResponse>(trackerEndpointResponseId);
            A.CallTo(() => sessionDataService.GetSessionById(DefaultSessionId))
                .Returns(null);

            // When
            var result = storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                DefaultSessionId.ToString(),
                DefaultDelegateId,
                DefaultCustomisationId,
                suppliedResponse
            );

            // Then
            using (new AssertionScope())
            {
                result.parsedSessionId.Should().BeNull();
                result.validationResponse.Should().Be(suppliedResponse);
                A.CallTo(() => sessionDataService.GetSessionById(DefaultSessionId)).MustHaveHappenedOnceExactly();
            }
        }

        [TestCase(DefaultCustomisationId + 1, DefaultDelegateId, true, -6)]
        [TestCase(DefaultCustomisationId, DefaultDelegateId + 1, true, -6)]
        [TestCase(DefaultCustomisationId, DefaultDelegateId, false, -6)]
        [TestCase(DefaultCustomisationId + 1, DefaultDelegateId, true, -24)]
        [TestCase(DefaultCustomisationId, DefaultDelegateId + 1, true, -24)]
        [TestCase(DefaultCustomisationId, DefaultDelegateId, false, -24)]
        public void
            ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints_returns_specified_exception_when_session_details_do_not_match_necessary_requirements(
                int customisationId,
                int delegateId,
                bool sessionActive,
                int trackerEndpointResponseId
            )
        {
            // Given
            var suppliedResponse = Enumeration.FromId<TrackerEndpointResponse>(trackerEndpointResponseId);
            A.CallTo(() => sessionDataService.GetSessionById(DefaultSessionId))
                .Returns(new Session(DefaultSessionId, delegateId, customisationId, DateTime.UtcNow, 1, sessionActive));

            // When
            var result = storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                DefaultSessionId.ToString(),
                DefaultDelegateId,
                DefaultCustomisationId,
                suppliedResponse
            );

            // Then
            using (new AssertionScope())
            {
                result.parsedSessionId.Should().BeNull();
                result.validationResponse.Should().Be(suppliedResponse);
                A.CallTo(() => sessionDataService.GetSessionById(DefaultSessionId)).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints_returns_parsed_session_ID_and_no_exception_when_session_is_valid()
        {
            // Given
            SessionServiceReturnsDefaultSession();

            // When
            var result = storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                DefaultSessionId.ToString(),
                DefaultDelegateId,
                DefaultCustomisationId,
                TrackerEndpointResponse.StoreAspProgressException
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
            storeAspService.StoreAspProgressAndSendEmailIfComplete(
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
            storeAspService.StoreAspProgressAndSendEmailIfComplete(
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

        [TestCase(null, 1, 123)]
        [TestCase(101, null, 123)]
        [TestCase(101, 1, null)]
        public void
            GetProgressAndValidateInputsForStoreAspAssess_returns_StoreAspAssessException_if_a_query_param_is_null(
                int? version,
                int? candidateId,
                int? customisationId
            )
        {
            // When
            var result = storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                version,
                100,
                candidateId,
                customisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspAssessException);
                result.progress.Should().BeNull();
                A.CallTo(() => courseDataService.GetDelegateCoursesInfo(A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void GetProgressAndValidateInputsForStoreAspAssess_returns_NullScoreException_if_score_param_is_null()
        {
            // When
            var result = storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                1,
                null,
                2,
                3
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.NullScoreTutorialStatusOrTime);
                result.progress.Should().BeNull();
                A.CallTo(() => courseDataService.GetDelegateCoursesInfo(A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void GetProgressAndValidateInputsForStoreAspAssess_returns_StoreAspAssessException_if_progress_is_null()
        {
            // Given
            A.CallTo(() => courseDataService.GetDelegateCoursesInfo(A<int>._)).Returns(new List<DelegateCourseInfo>());

            // When
            var result = storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                1,
                2,
                DefaultDelegateId,
                4
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspAssessException);
                result.progress.Should().BeNull();
                A.CallTo(() => courseDataService.GetDelegateCoursesInfo(DefaultDelegateId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            GetProgressAndValidateInputsForStoreAspAssess_returns_StoreAspAssessException_if_progress_records_are_all_invalid()
        {
            // Given
            var invalidProgressRecords = Builder<DelegateCourseInfo>.CreateListOfSize(3).All()
                .With(p => p.RemovedDate = null)
                .And(p => p.Completed = null)
                .And(p => p.CustomisationId = DefaultCustomisationId)
                .And(p => p.IsProgressLocked = false)
                .TheFirst(1).With(p => p.RemovedDate = DateTime.UtcNow)
                .TheNext(1).With(p => p.Completed = DateTime.UtcNow)
                .TheNext(1).With(p => p.CustomisationId = DefaultCustomisationId + 100)
                .Build();
            A.CallTo(() => courseDataService.GetDelegateCoursesInfo(A<int>._)).Returns(invalidProgressRecords);

            // When
            var result = storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                1,
                2,
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspAssessException);
                result.progress.Should().BeNull();
                A.CallTo(() => courseDataService.GetDelegateCoursesInfo(DefaultDelegateId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            GetProgressAndValidateInputsForStoreAspAssess_returns_StoreAspAssessException_if_otherwise_valid_progress_record_is_locked()
        {
            // Given
            var progressRecords = Builder<DelegateCourseInfo>.CreateListOfSize(4).All()
                .With(p => p.RemovedDate = null)
                .And(p => p.Completed = null)
                .And(p => p.CustomisationId = DefaultCustomisationId)
                .And(p => p.IsProgressLocked = false)
                .TheFirst(1).With(p => p.RemovedDate = DateTime.UtcNow)
                .TheNext(1).With(p => p.Completed = DateTime.UtcNow)
                .TheNext(1).With(p => p.CustomisationId = DefaultCustomisationId + 100)
                .TheNext(1).With(p => p.IsProgressLocked = true)
                .Build();
            A.CallTo(() => courseDataService.GetDelegateCoursesInfo(A<int>._)).Returns(progressRecords);

            // When
            var result = storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                1,
                2,
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspAssessException);
                result.progress.Should().BeNull();
                A.CallTo(() => courseDataService.GetDelegateCoursesInfo(DefaultDelegateId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            GetProgressAndValidateInputsForStoreAspAssess_returns_StoreAspAssessException_if_multiple_valid_progress_records_obtained()
        {
            // Given
            var progressRecords = Builder<DelegateCourseInfo>.CreateListOfSize(5).All()
                .With(p => p.RemovedDate = null)
                .And(p => p.Completed = null)
                .And(p => p.CustomisationId = DefaultCustomisationId)
                .And(p => p.IsProgressLocked = false)
                .TheFirst(1).With(p => p.RemovedDate = DateTime.UtcNow)
                .TheNext(1).With(p => p.Completed = DateTime.UtcNow)
                .TheNext(1).With(p => p.CustomisationId = DefaultCustomisationId + 100)
                .Build();
            A.CallTo(() => courseDataService.GetDelegateCoursesInfo(A<int>._)).Returns(progressRecords);

            // When
            var result = storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                1,
                2,
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspAssessException);
                result.progress.Should().BeNull();
                A.CallTo(() => courseDataService.GetDelegateCoursesInfo(DefaultDelegateId))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            GetProgressAndValidateInputsForStoreAspAssess_returns_correct_progress_record_and_no_exception_when_all_is_valid()
        {
            // Given
            var progressRecords = Builder<DelegateCourseInfo>.CreateListOfSize(4).All()
                .With(p => p.RemovedDate = null)
                .And(p => p.Completed = null)
                .And(p => p.CustomisationId = DefaultCustomisationId)
                .And(p => p.IsProgressLocked = false)
                .TheFirst(1).With(p => p.RemovedDate = DateTime.UtcNow)
                .TheNext(1).With(p => p.Completed = DateTime.UtcNow)
                .TheNext(1).With(p => p.CustomisationId = DefaultCustomisationId + 100)
                .Build();
            A.CallTo(() => courseDataService.GetDelegateCoursesInfo(A<int>._)).Returns(progressRecords);

            // When
            var result = storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                1,
                2,
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                var expectedProgressRecord = progressRecords[3];
                result.validationResponse.Should().BeNull();
                result.progress.Should().BeEquivalentTo(expectedProgressRecord);
                A.CallTo(() => courseDataService.GetDelegateCoursesInfo(DefaultDelegateId))
                    .MustHaveHappenedOnceExactly();
            }
        }


        [Test]
        public void GetAndValidateSectionAssessmentDetails_returns_StoreAspAssessException_when_sectionId_is_null()
        {
            // When
            var result = storeAspService.GetAndValidateSectionAssessmentDetails(
                null,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspAssessException);
                result.assessmentDetails.Should().BeNull();
                A.CallTo(() => progressService.GetSectionAndApplicationDetailsForAssessAttempts(A<int>._, A<int>._))
                    .MustNotHaveHappened();
            }
        }

        [Test]
        public void
            GetAndValidateSectionAssessmentDetails_returns_StoreAspAssessException_when_details_cannot_be_found()
        {
            // Given
            A.CallTo(() => progressService.GetSectionAndApplicationDetailsForAssessAttempts(A<int>._, A<int>._))
                .Returns(null);

            // When
            var result = storeAspService.GetAndValidateSectionAssessmentDetails(
                DefaultSectionId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().Be(TrackerEndpointResponse.StoreAspAssessException);
                result.assessmentDetails.Should().BeNull();
                A.CallTo(
                    () => progressService.GetSectionAndApplicationDetailsForAssessAttempts(
                        DefaultSectionId,
                        DefaultCustomisationId
                    )
                ).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            GetAndValidateSectionAssessmentDetails_returns_assessment_details_and_no_exception_when_details_are_retrieved_correctly()
        {
            // Given
            var expectedAssessmentDetails = new SectionAndApplicationDetailsForAssessAttempts
            {
                AssessAttempts = 1,
                PlaPassThreshold = 100,
                SectionNumber = 5,
            };
            A.CallTo(() => progressService.GetSectionAndApplicationDetailsForAssessAttempts(A<int>._, A<int>._))
                .Returns(expectedAssessmentDetails);

            // When
            var result = storeAspService.GetAndValidateSectionAssessmentDetails(
                DefaultSectionId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.validationResponse.Should().BeNull();
                result.assessmentDetails.Should().BeEquivalentTo(expectedAssessmentDetails);
                A.CallTo(
                    () => progressService.GetSectionAndApplicationDetailsForAssessAttempts(
                        DefaultSectionId,
                        DefaultCustomisationId
                    )
                ).MustHaveHappenedOnceExactly();
            }
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
