namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class TrackerActionServiceTests
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
        private const int DefaultScore = 42;
        private const int DefaultPlaPassThreshold = 50;
        private const int DefaultAssessAttemptLimit = 3;
        private const int DefaultSectionNumber = 2;

        private readonly SectionAndApplicationDetailsForAssessAttempts assessmentDetails =
            new SectionAndApplicationDetailsForAssessAttempts
            {
                AssessAttempts = DefaultAssessAttemptLimit,
                PlaPassThreshold = DefaultPlaPassThreshold,
                SectionNumber = DefaultSectionNumber,
            };

        private readonly DateTime currentTime = new DateTime(2022, 06, 15, 14, 30, 45);

        private readonly DelegateCourseInfo delegateCourseInfo = new DelegateCourseInfo
        {
            ProgressId = DefaultProgressId,
            DelegateId = DefaultDelegateId,
            CustomisationId = DefaultCustomisationId,
        };

        private readonly DetailedCourseProgress detailedCourseProgress =
            ProgressTestHelper.GetDefaultDetailedCourseProgress(
                DefaultProgressId,
                DefaultDelegateId,
                DefaultCustomisationId
            );

        private IClockUtility clockUtility = null!;
        private ILogger<TrackerActionService> logger = null!;
        private IProgressDataService progressDataService = null!;
        private IProgressService progressService = null!;
        private ISessionDataService sessionDataService = null!;
        private IStoreAspService storeAspService = null!;
        private ITrackerActionService trackerActionService = null!;

        private ITutorialContentDataService tutorialContentDataService = null!;

        [SetUp]
        public void Setup()
        {
            tutorialContentDataService = A.Fake<ITutorialContentDataService>();
            clockUtility = A.Fake<IClockUtility>();
            progressService = A.Fake<IProgressService>();
            progressDataService = A.Fake<IProgressDataService>();
            sessionDataService = A.Fake<ISessionDataService>();
            storeAspService = A.Fake<IStoreAspService>();
            logger = A.Fake<ILogger<TrackerActionService>>();

            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            trackerActionService = new TrackerActionService(
                tutorialContentDataService,
                clockUtility,
                progressService,
                progressDataService,
                sessionDataService,
                storeAspService,
                logger
            );
        }

        [Test]
        public void GetObjectiveArray_returns_results_in_specified_json_format()
        {
            // Given
            var sampleObjectiveArrayResult = new[]
            {
                new Objective(1, new List<int> { 6, 7, 8 }, 4),
                new Objective(2, new List<int> { 17, 18, 19 }, 0),
            };
            A.CallTo(
                    () => tutorialContentDataService.GetNonArchivedObjectivesBySectionAndCustomisationId(
                        A<int>._,
                        A<int>._
                    )
                )
                .Returns(sampleObjectiveArrayResult);

            // When
            var result = trackerActionService.GetObjectiveArray(1, 1);

            // Then
            result.Should().BeEquivalentTo(new TrackerObjectiveArray(sampleObjectiveArrayResult));
        }

        [Test]
        public void GetObjectiveArray_returns_empty_object_json_if_no_results_found()
        {
            // Given
            A.CallTo(
                    () => tutorialContentDataService.GetNonArchivedObjectivesBySectionAndCustomisationId(
                        A<int>._,
                        A<int>._
                    )
                )
                .Returns(new List<Objective>());

            // When
            var result = trackerActionService.GetObjectiveArray(1, 1);

            // Then
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
            // Given
            A.CallTo(
                    () => tutorialContentDataService.GetNonArchivedObjectivesBySectionAndCustomisationId(
                        A<int>._,
                        A<int>._
                    )
                )
                .Returns(new[] { new Objective(1, new List<int> { 1 }, 9) });

            // When
            var result = trackerActionService.GetObjectiveArray(customisationId, sectionId);

            // Then
            result.Should().Be(null);
        }

        [Test]
        public void GetObjectiveArrayCc_returns_results_in_specified_json_format()
        {
            // Given
            var sampleCcObjectiveArrayResult = new[]
            {
                new CcObjective(1, "name1", 4),
                new CcObjective(1, "name2", 0),
            };
            A.CallTo(() => tutorialContentDataService.GetNonArchivedCcObjectivesBySectionAndCustomisationId(1, 1, true))
                .Returns(sampleCcObjectiveArrayResult);

            // When
            var result = trackerActionService.GetObjectiveArrayCc(1, 1, true);

            // Then
            result.Should().BeEquivalentTo(new TrackerObjectiveArrayCc(sampleCcObjectiveArrayResult));
        }

        [Test]
        public void GetObjectiveArrayCc_returns_empty_object_json_if_no_results_found()
        {
            // Given
            A.CallTo(
                    () => tutorialContentDataService.GetNonArchivedCcObjectivesBySectionAndCustomisationId(
                        A<int>._,
                        A<int>._,
                        A<bool>._
                    )
                )
                .Returns(new List<CcObjective>());

            // When
            var result = trackerActionService.GetObjectiveArrayCc(1, 1, true);

            // Then
            result.Should().Be(null);
        }

        [Test]
        [TestCase(null, 1, true)]
        [TestCase(1, null, true)]
        [TestCase(1, 1, null)]
        public void GetObjectiveArrayCc_returns_null_if_parameter_missing(
            int? customisationId,
            int? sectionId,
            bool? isPostLearning
        )
        {
            // When
            var result = trackerActionService.GetObjectiveArrayCc(customisationId, sectionId, isPostLearning);

            // Then
            result.Should().Be(null);
            A.CallTo(
                () => tutorialContentDataService.GetNonArchivedCcObjectivesBySectionAndCustomisationId(
                    A<int>._,
                    A<int>._,
                    A<bool>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void StoreDiagnosticJson_returns_success_response_if_successful()
        {
            // Given
            const string diagnosticOutcome = "[{'tutorialid':425,'myscore':4},{'tutorialid':424,'myscore':3}]";

            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).DoesNothing();

            // When
            var result = trackerActionService.StoreDiagnosticJson(DefaultProgressId, diagnosticOutcome);

            // Then
            result.Should().Be(TrackerEndpointResponse.Success);
            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    DefaultProgressId,
                    424,
                    3
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    DefaultProgressId,
                    425,
                    4
                )
            ).MustHaveHappenedOnceExactly();
        }

        [TestCase(1, null)]
        [TestCase(null, "[{'tutorialid':425,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, "[{'unexpectedkey':425,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, "[{'tutorialid':999999999999999999,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, "[{'tutorialid':x,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, "[{'tutorialid':425,'myscore':x},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, "[{'tutorialid':0,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        public void
            StoreDiagnosticJson_returns_StoreDiagnosticScoreException_if_error_when_deserializing_json_or_updating_score(
                int? progressId,
                string? diagnosticOutcome
            )
        {
            // When
            var result = trackerActionService.StoreDiagnosticJson(progressId, diagnosticOutcome);

            // Then
            result.Should().Be(TrackerEndpointResponse.StoreDiagnosticScoreException);
            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void StoreAspProgressV2_returns_exception_from_validation()
        {
            // Given
            A.CallTo(
                () => storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._
                )
            ).Returns((TrackerEndpointResponse.StoreAspProgressException, null));

            // When
            var result = trackerActionService.StoreAspProgressV2(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            result.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
            A.CallTo(
                () => storeAspService.StoreAspProgressAndSendEmailIfComplete(
                    A<DetailedCourseProgress>._,
                    A<int>._,
                    A<string?>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void StoreAspProgressV2_stores_progress_when_valid_and_returns_success_response_if_successful()
        {
            // Given
            StoreAspServiceReturnsDefaultDetailedCourseProgressOnValidation();

            // When
            var result = trackerActionService.StoreAspProgressV2(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                CallsToStoreAspProgressV2MethodsMustHaveHappened();
            }
        }

        [Test]
        public void StoreAspProgressNoSession_returns_exception_from_query_and_progress_validation()
        {
            // Given
            A.CallTo(
                () => storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._
                )
            ).Returns((TrackerEndpointResponse.StoreAspProgressException, null));

            // When
            var result = trackerActionService.StoreAspProgressNoSession(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId,
                DefaultSessionId.ToString()
            );

            // Then
            result.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
            A.CallTo(
                () => storeAspService.StoreAspProgressAndSendEmailIfComplete(
                    A<DetailedCourseProgress>._,
                    A<int>._,
                    A<string?>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void StoreAspProgressNoSession_returns_exception_from_session_validation()
        {
            // Given
            StoreAspServiceReturnsDefaultDetailedCourseProgressOnValidation();
            A.CallTo(
                () => storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                    A<string?>._,
                    A<int>._,
                    A<int>._,
                    A<TrackerEndpointResponse>._
                )
            ).Returns((TrackerEndpointResponse.StoreAspProgressException, null));

            // When
            var result = trackerActionService.StoreAspProgressNoSession(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId,
                DefaultSessionId.ToString()
            );

            // Then
            result.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
            A.CallTo(
                () => storeAspService.StoreAspProgressAndSendEmailIfComplete(
                    A<DetailedCourseProgress>._,
                    A<int>._,
                    A<string?>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            A.CallTo(
                () => storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                    DefaultSessionId.ToString(),
                    DefaultDelegateId,
                    DefaultCustomisationId,
                    TrackerEndpointResponse.StoreAspProgressException
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void
            StoreAspProgressNoSession_updates_learning_time_and_stores_progress_when_valid_and_returns_success_response_if_successful()
        {
            // Given
            StoreAspServiceReturnsDefaultDetailedCourseProgressOnValidation();
            StoreAspServiceReturnsDefaultSessionOnValidation();

            // When
            var result = trackerActionService.StoreAspProgressNoSession(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId,
                DefaultSessionId.ToString()
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                A.CallTo(
                    () => sessionDataService.AddTutorialTimeToSessionDuration(DefaultSessionId, DefaultTutorialTime)
                ).MustHaveHappenedOnceExactly();
                CallsToStoreAspProgressV2MethodsMustHaveHappened();
            }
        }

        [Test]
        public void
            StoreAspAssessNoSession_returns_exception_from_query_param_and_progress_validation()
        {
            // Given
            A.CallTo(
                () => storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._
                )
            ).Returns((TrackerEndpointResponse.StoreAspAssessException, null));

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                DefaultSessionId.ToString()
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.StoreAspAssessException);
                A.CallTo(
                    () => storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                        DefaultCustomisationVersion,
                        DefaultScore,
                        DefaultDelegateId,
                        DefaultCustomisationId
                    )
                ).MustHaveHappenedOnceExactly();
                A.CallTo(() => storeAspService.GetAndValidateSectionAssessmentDetails(A<int?>._, A<int>._))
                    .MustNotHaveHappened();
                A.CallTo(
                    () => storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                        A<string?>._,
                        A<int>._,
                        A<int>._,
                        A<TrackerEndpointResponse>._
                    )
                ).MustNotHaveHappened();
                CallsAfterStoreAspAssessValidationMustNotHaveHappened();
            }
        }

        [Test]
        public void StoreAspAssessNoSession_returns_exception_from_section_validation()
        {
            // Given
            A.CallTo(
                () => storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._
                )
            ).Returns((null, delegateCourseInfo));
            A.CallTo(() => storeAspService.GetAndValidateSectionAssessmentDetails(A<int?>._, A<int>._))
                .Returns((TrackerEndpointResponse.StoreAspAssessException, null));

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                DefaultSessionId.ToString()
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.StoreAspAssessException);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues();
                A.CallTo(
                    () => storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                        A<string?>._,
                        A<int>._,
                        A<int>._,
                        A<TrackerEndpointResponse>._
                    )
                ).MustNotHaveHappened();
                CallsAfterStoreAspAssessValidationMustNotHaveHappened();
            }
        }

        [Test]
        public void StoreAspAssessNoSession_returns_exception_from_session_validation()
        {
            // Given
            GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults();
            A.CallTo(
                () => storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                    A<string?>._,
                    A<int>._,
                    A<int>._,
                    A<TrackerEndpointResponse>._
                )
            ).Returns((TrackerEndpointResponse.StoreAspAssessException, null));

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                DefaultSessionId.ToString()
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.StoreAspAssessException);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues();
                A.CallTo(
                    () => storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                        DefaultSessionId.ToString(),
                        DefaultDelegateId,
                        DefaultCustomisationId,
                        TrackerEndpointResponse.StoreAspAssessException
                    )
                ).MustHaveHappenedOnceExactly();
                A.CallTo(() => sessionDataService.UpdateDelegateSessionDuration(A<int>._, A<DateTime>._))
                    .MustNotHaveHappened();
                CallsAfterStoreAspAssessValidationMustNotHaveHappened();
            }
        }

        [Test]
        public void StoreAspAssessNoSession_updates_session_duration_when_session_is_valid()
        {
            // Given
            GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults();
            GivenSessionValidationPassesAndReturnsDefaultSessionId();

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                DefaultSessionId.ToString()
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues();
                SessionValidationAndSessionDurationUpdateMustHaveBeenCalledOnce();
            }
        }

        [Test]
        public void StoreAspAssessNoSession_does_not_validate_or_update_session_duration_when_session_id_is_null()
        {
            // Given
            GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults();

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                null
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues();
                A.CallTo(
                    () => storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                        A<string?>._,
                        A<int>._,
                        A<int>._,
                        A<TrackerEndpointResponse>._
                    )
                ).MustNotHaveHappened();
                A.CallTo(() => sessionDataService.UpdateDelegateSessionDuration(A<int>._, A<DateTime>._))
                    .MustNotHaveHappened();
            }
        }

        [TestCase(0)]
        [TestCase(30)]
        [TestCase(46)]
        [TestCase(60)]
        public void StoreAspAssessNoSession_does_not_create_new_AssessAttempt_record_if_duplicate_exists(
            int secondsInPast
        )
        {
            // Given
            GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults();
            var duplicateAssessAttempt = Builder<AssessAttempt>.CreateListOfSize(1).TheFirst(1)
                .With(a => a.Score = DefaultScore)
                .And(a => a.Date = currentTime.AddSeconds(-secondsInPast)).Build();
            A.CallTo(
                () => progressDataService.GetAssessAttemptsForProgressSection(
                    DefaultProgressId,
                    DefaultSectionNumber
                )
            ).Returns(duplicateAssessAttempt);

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                null
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues();
                NewAssessAttemptMustNotHaveBeenInserted();
            }
        }

        [TestCase(DefaultPlaPassThreshold - 1, false)]
        [TestCase(DefaultPlaPassThreshold, true)]
        [TestCase(DefaultPlaPassThreshold + 1, true)]
        public void
            StoreAspAssessNoSession_creates_new_AssessAttempt_record_with_correct_status_if_duplicate_does_not_exist(
                int newlySubmittedScore,
                bool newAttemptHasPassed
            )
        {
            // Given
            GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults();
            var existingAssessAttempts = Builder<AssessAttempt>.CreateListOfSize(2).TheFirst(1)
                .With(a => a.Score = DefaultScore)
                .And(a => a.Date = currentTime.AddSeconds(-61))
                .TheNext(1).With(a => a.Score = DefaultScore + 1)
                .And(a => a.Date = currentTime.AddSeconds(-10)).Build();
            A.CallTo(
                () => progressDataService.GetAssessAttemptsForProgressSection(
                    DefaultProgressId,
                    DefaultSectionNumber
                )
            ).Returns(existingAssessAttempts);

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                newlySubmittedScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                null
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues(newlySubmittedScore);
                A.CallTo(
                    () => progressDataService.GetAssessAttemptsForProgressSection(
                        DefaultProgressId,
                        DefaultSectionNumber
                    )
                ).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => progressDataService.InsertAssessAttempt(
                        DefaultDelegateId,
                        DefaultCustomisationId,
                        DefaultCustomisationVersion,
                        currentTime,
                        DefaultSectionNumber,
                        newlySubmittedScore,
                        newAttemptHasPassed,
                        DefaultProgressId
                    )
                ).MustHaveHappenedOnceExactly();
            }
        }

        [TestCase(DefaultAssessAttemptLimit - 1)]
        [TestCase(DefaultAssessAttemptLimit)]
        [TestCase(DefaultAssessAttemptLimit + 1)]
        public void
            StoreAspAssessNoSession_locks_progress_when_new_attempt_is_failure_causes_threshold_to_be_met_or_breached(
                int numbersOfPreviouslyFailedAttempts
            )
        {
            // Given
            GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults();
            var previousAssessmentAttempts = Builder<AssessAttempt>.CreateListOfSize(numbersOfPreviouslyFailedAttempts)
                .All()
                .With(a => a.Status = false).Build();
            A.CallTo(
                () => progressDataService.GetAssessAttemptsForProgressSection(
                    DefaultProgressId,
                    DefaultSectionNumber
                )
            ).Returns(previousAssessmentAttempts);

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                null
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues();
                NewAssessAttemptMustHaveBeenInserted();
                CallsToLockProgressMustHaveBeenCalledOnce();
            }
        }

        [TestCase(DefaultAssessAttemptLimit)]
        [TestCase(DefaultAssessAttemptLimit + 1)]
        public void
            StoreAspAssessNoSession_locks_progress_when_new_attempt_is_duplicate_but_existing_failures_meet_or_breach_limit(
                int numbersOfPreviouslyFailedAttempts
            )
        {
            // Given
            GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults();
            var previousAssessmentAttempts = Builder<AssessAttempt>.CreateListOfSize(numbersOfPreviouslyFailedAttempts)
                .All()
                .With(a => a.Status = false)
                .And(a => a.Score = DefaultScore)
                .And(a => a.Date = currentTime)
                .Build();
            A.CallTo(
                () => progressDataService.GetAssessAttemptsForProgressSection(
                    DefaultProgressId,
                    DefaultSectionNumber
                )
            ).Returns(previousAssessmentAttempts);

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                null
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues();
                NewAssessAttemptMustNotHaveBeenInserted();
                CallsToLockProgressMustHaveBeenCalledOnce();
            }
        }

        [Test]
        public void
            StoreAspAssessNoSession_does_not_locks_progress_when_failed_attempts_do_not_meet_or_breach_limit()
        {
            // Given
            GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults();
            var previousAssessmentAttempts = Builder<AssessAttempt>.CreateListOfSize(10)
                .TheFirst(DefaultAssessAttemptLimit - 1)
                .With(a => a.Status = false)
                .And(a => a.Score = DefaultScore)
                .And(a => a.Date = currentTime)
                .TheRest().With(a => a.Status = true)
                .Build();
            A.CallTo(
                () => progressDataService.GetAssessAttemptsForProgressSection(
                    DefaultProgressId,
                    DefaultSectionNumber
                )
            ).Returns(previousAssessmentAttempts);

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                null
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues();
                NewAssessAttemptMustNotHaveBeenInserted();
                CallsToCheckCompletionMustHaveBeenMade();
            }
        }

        [Test]
        public void
            StoreAspAssessNoSession_does_not_lock_progress_when_new_attempt_would_meet_threshold_but_the_attempt_succeeded()
        {
            // Given
            GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults();
            var previousAssessmentAttempts = Builder<AssessAttempt>.CreateListOfSize(2).All()
                .With(a => a.Status = false)
                .Build();
            A.CallTo(
                () => progressDataService.GetAssessAttemptsForProgressSection(
                    DefaultProgressId,
                    DefaultSectionNumber
                )
            ).Returns(previousAssessmentAttempts);

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultPlaPassThreshold,
                DefaultDelegateId,
                DefaultCustomisationId,
                null
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues(DefaultPlaPassThreshold);
                NewAssessAttemptMustHaveBeenInserted(DefaultPlaPassThreshold);
                CallsToCheckCompletionMustHaveBeenMade();
            }
        }

        [Test]
        public void StoreAspAssessNoSession_does_not_lock_progress_when_the_threshold_is_unlimited()
        {
            // Given
            GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults(0);
            var previousAssessmentAttempts = Builder<AssessAttempt>.CreateListOfSize(100).All()
                .With(a => a.Status = false).And(a => a.Score = 1).Build();
            A.CallTo(
                () => progressDataService.GetAssessAttemptsForProgressSection(
                    DefaultProgressId,
                    DefaultSectionNumber
                )
            ).Returns(previousAssessmentAttempts);

            // When
            var result = trackerActionService.StoreAspAssessNoSession(
                DefaultCustomisationVersion,
                DefaultSectionId,
                DefaultScore,
                DefaultDelegateId,
                DefaultCustomisationId,
                null
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues();
                NewAssessAttemptMustHaveBeenInserted();
                CallsToCheckCompletionMustHaveBeenMade();
            }
        }

        private void StoreAspServiceReturnsDefaultDetailedCourseProgressOnValidation()
        {
            A.CallTo(
                () => storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
                    DefaultProgressId,
                    DefaultCustomisationVersion,
                    DefaultTutorialId,
                    DefaultTutorialTime,
                    DefaultTutorialStatus,
                    DefaultDelegateId,
                    DefaultCustomisationId
                )
            ).Returns((null, detailedCourseProgress));
        }

        private void StoreAspServiceReturnsDefaultSessionOnValidation()
        {
            A.CallTo(
                () => storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                    DefaultSessionId.ToString(),
                    DefaultDelegateId,
                    DefaultCustomisationId,
                    A<TrackerEndpointResponse>._
                )
            ).Returns((null, DefaultSessionId));
        }

        private void CallsToStoreAspProgressV2MethodsMustHaveHappened()
        {
            A.CallTo(
                () => storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
                    DefaultProgressId,
                    DefaultCustomisationVersion,
                    DefaultTutorialId,
                    DefaultTutorialTime,
                    DefaultTutorialStatus,
                    DefaultDelegateId,
                    DefaultCustomisationId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => storeAspService.StoreAspProgressAndSendEmailIfComplete(
                    detailedCourseProgress,
                    DefaultCustomisationVersion,
                    DefaultLmGvSectionRow,
                    DefaultTutorialId,
                    DefaultTutorialTime,
                    DefaultTutorialStatus
                )
            ).MustHaveHappenedOnceExactly();
        }

        private void CallsAfterStoreAspAssessValidationMustNotHaveHappened()
        {
            A.CallTo(() => progressDataService.GetAssessAttemptsForProgressSection(A<int>._, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(
                    () => progressDataService.InsertAssessAttempt(
                        A<int>._,
                        A<int>._,
                        A<int>._,
                        A<DateTime>._,
                        A<int>._,
                        A<int>._,
                        A<bool>._,
                        A<int>._
                    )
                )
                .MustNotHaveHappened();
            A.CallTo(() => progressDataService.LockProgress(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => progressService.CheckProgressForCompletionAndSendEmailIfCompleted(A<DelegateCourseInfo>._))
                .MustNotHaveHappened();
        }

        private void GivenRequiredValidationForStoreAspAssessPassesAndReturnsDefaults(
            int assessAttemptsLimit = DefaultAssessAttemptLimit
        )
        {
            A.CallTo(
                () => storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._
                )
            ).Returns((null, delegateCourseInfo));
            assessmentDetails.AssessAttempts = assessAttemptsLimit;
            A.CallTo(() => storeAspService.GetAndValidateSectionAssessmentDetails(A<int?>._, A<int>._))
                .Returns((null, assessmentDetails));
        }

        public void GivenSessionValidationPassesAndReturnsDefaultSessionId()
        {
            A.CallTo(
                () => storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                    A<string?>._,
                    A<int>._,
                    A<int>._,
                    A<TrackerEndpointResponse>._
                )
            ).Returns((null, DefaultSessionId));
        }

        private void RequiredValidationForStoreAspAssessMustHaveBeenCalledOnceWithDefaultValues(
            int score = DefaultScore
        )
        {
            A.CallTo(
                () => storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                    DefaultCustomisationVersion,
                    score,
                    DefaultDelegateId,
                    DefaultCustomisationId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => storeAspService.GetAndValidateSectionAssessmentDetails(
                    DefaultSectionId,
                    DefaultCustomisationId
                )
            ).MustHaveHappenedOnceExactly();
        }

        private void SessionValidationAndSessionDurationUpdateMustHaveBeenCalledOnce()
        {
            A.CallTo(
                () => storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                    DefaultSessionId.ToString(),
                    DefaultDelegateId,
                    DefaultCustomisationId,
                    TrackerEndpointResponse.StoreAspAssessException
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionDataService.UpdateDelegateSessionDuration(DefaultSessionId, currentTime))
                .MustHaveHappenedOnceExactly();
        }

        private void NewAssessAttemptMustHaveBeenInserted(int score = DefaultScore)
        {
            A.CallTo(
                () => progressDataService.GetAssessAttemptsForProgressSection(
                    DefaultProgressId,
                    DefaultSectionNumber
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => progressDataService.InsertAssessAttempt(
                    DefaultDelegateId,
                    DefaultCustomisationId,
                    DefaultCustomisationVersion,
                    currentTime,
                    DefaultSectionNumber,
                    score,
                    score >= DefaultPlaPassThreshold,
                    DefaultProgressId
                )
            ).MustHaveHappenedOnceExactly();
        }

        private void NewAssessAttemptMustNotHaveBeenInserted()
        {
            A.CallTo(
                () => progressDataService.GetAssessAttemptsForProgressSection(
                    DefaultProgressId,
                    DefaultSectionNumber
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => progressDataService.InsertAssessAttempt(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<DateTime>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        private void CallsToLockProgressMustHaveBeenCalledOnce()
        {
            A.CallTo(() => progressDataService.LockProgress(DefaultProgressId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => progressService.CheckProgressForCompletionAndSendEmailIfCompleted(A<DelegateCourseInfo>._))
                .MustNotHaveHappened();
        }

        private void CallsToCheckCompletionMustHaveBeenMade()
        {
            A.CallTo(() => progressDataService.LockProgress(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => progressService.CheckProgressForCompletionAndSendEmailIfCompleted(delegateCourseInfo))
                .MustHaveHappenedOnceExactly();
        }
    }
}
