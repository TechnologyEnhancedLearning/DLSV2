namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class ProgressServiceTests
    {
        private IClockService clockService = null!;
        private ICourseAdminFieldsService courseAdminFieldsService = null!;
        private ICourseDataService courseDataService = null!;
        private ILearningLogItemsDataService learningLogItemsDataService = null!;
        private INotificationService notificationService = null!;
        private IProgressDataService progressDataService = null!;
        private IProgressService progressService = null!;

        [SetUp]
        public void SetUp()
        {
            courseDataService = A.Fake<ICourseDataService>();
            progressDataService = A.Fake<IProgressDataService>();
            notificationService = A.Fake<INotificationService>();
            learningLogItemsDataService = A.Fake<ILearningLogItemsDataService>();
            clockService = A.Fake<IClockService>();
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();

            progressService = new ProgressService(
                courseDataService,
                progressDataService,
                notificationService,
                learningLogItemsDataService,
                clockService, 
                courseAdminFieldsService
            );
        }

        [Test]
        public void UpdateSupervisor_does_not_update_records_if_new_supervisor_matches_current()
        {
            // Given
            const int supervisorId = 1;
            const int progressId = 1;
            var courseInfo = new DelegateCourseInfo { SupervisorAdminId = supervisorId };
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId)).Returns(courseInfo);

            // When
            progressService.UpdateSupervisor(progressId, supervisorId);

            // Then
            A.CallTo(
                () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(A<int>._, A<int>._, A<DateTime?>._)
            ).MustNotHaveHappened();
            A.CallTo(
                () => progressDataService.ClearAspProgressVerificationRequest(A<int>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void UpdateSupervisor_updates_records_if_new_supervisor_does_not_match_current()
        {
            // Given
            const int oldSupervisorId = 1;
            const int newSupervisorId = 6;
            const int progressId = 1;
            var courseInfo = new DelegateCourseInfo { SupervisorAdminId = oldSupervisorId };
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId)).Returns(courseInfo);

            // When
            progressService.UpdateSupervisor(progressId, newSupervisorId);

            // Then
            A.CallTo(
                () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                    progressId,
                    newSupervisorId,
                    A<DateTime?>._
                )
            ).MustHaveHappened();
            A.CallTo(
                () => progressDataService.ClearAspProgressVerificationRequest(progressId)
            ).MustHaveHappened();
        }

        [Test]
        public void UpdateSupervisor_sets_supervisor_id_to_0_if_new_supervisor_is_null()
        {
            // Given
            const int oldSupervisorId = 1;
            const int progressId = 1;
            var courseInfo = new DelegateCourseInfo { SupervisorAdminId = oldSupervisorId };
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId)).Returns(courseInfo);

            // When
            progressService.UpdateSupervisor(progressId, null);

            // Then
            A.CallTo(
                () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(progressId, 0, A<DateTime?>._)
            ).MustHaveHappened();
            A.CallTo(
                () => progressDataService.ClearAspProgressVerificationRequest(progressId)
            ).MustHaveHappened();
        }

        [Test]
        public void UpdateSupervisor_throws_exception_if_no_progress_record_found()
        {
            // Given
            const int progressId = 1;
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId)).Returns(null);

            // Then
            Assert.Throws<ProgressNotFoundException>(() => progressService.UpdateSupervisor(progressId, null));
            A.CallTo(
                () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(A<int>._, A<int>._, A<DateTime?>._)
            ).MustNotHaveHappened();
            A.CallTo(
                () => progressDataService.ClearAspProgressVerificationRequest(A<int>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void UpdateCompleteByDate_calls_data_service()
        {
            // Given
            const int progressId = 1;
            const int delegateId = 1;
            var completeByDate = new DateTime(2021, 09, 01);
            var courseInfo = new DelegateCourseInfo { DelegateId = delegateId };
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId)).Returns(courseInfo);

            // When
            progressService.UpdateCompleteByDate(progressId, completeByDate);

            // Then
            A.CallTo(() => courseDataService.SetCompleteByDate(progressId, delegateId, completeByDate))
                .MustHaveHappened();
        }

        [Test]
        public void UpdateCompleteByDate_throws_exception_if_no_progress_record_found()
        {
            // Given
            const int progressId = 1;
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId)).Returns(null);

            // Then
            Assert.Throws<ProgressNotFoundException>(() => progressService.UpdateCompleteByDate(progressId, null));
            A.CallTo(
                () => courseDataService.SetCompleteByDate(A<int>._, A<int>._, A<DateTime?>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void UpdateCompletionDate_calls_data_service()
        {
            // Given
            const int progressId = 1;
            const int delegateId = 1;
            var completeByDate = new DateTime(2021, 09, 01);
            var courseInfo = new DelegateCourseInfo { DelegateId = delegateId };
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId)).Returns(courseInfo);

            // When
            progressService.UpdateCompletionDate(progressId, completeByDate);

            // Then
            A.CallTo(() => progressDataService.SetCompletionDate(progressId, completeByDate))
                .MustHaveHappened();
        }

        [Test]
        public void UpdateCompletionDate_throws_exception_if_no_progress_record_found()
        {
            // Given
            const int progressId = 1;
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId)).Returns(null);

            // Then
            Assert.Throws<ProgressNotFoundException>(() => progressService.UpdateCompletionDate(progressId, null));
            A.CallTo(
                () => courseDataService.SetCompleteByDate(A<int>._, A<int>._, A<DateTime?>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void UpdateDiagnosticScore_calls_data_service()
        {
            // Given
            const int progressId = 1;
            const int tutorialId = 1;
            const int myScore = 1;
            A.CallTo(() => progressDataService.UpdateDiagnosticScore(A<int>._, A<int>._, A<int>._))
                .DoesNothing();

            // When
            progressService.UpdateDiagnosticScore(progressId, tutorialId, myScore);

            // Then
            A.CallTo(() => progressDataService.UpdateDiagnosticScore(progressId, tutorialId, myScore))
                .MustHaveHappened();
        }

        [Test]
        public void GetDetailedCourseProgress_returns_null_if_no_progress_found()
        {
            // Given
            A.CallTo(() => progressDataService.GetProgressByProgressId(A<int>._)).Returns(null);

            // When
            var result = progressService.GetDetailedCourseProgress(1);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetDetailedCourseProgress_returns_progress_composed_from_data_if_progress_found()
        {
            // Given
            var testCourseProgress = new Progress
            {
                ProgressId = 1, CustomisationId = 4, CandidateId = 5, DiagnosticScore = 42, Completed = null,
                RemovedDate = null,
            };
            var testSectionProgress = new List<DetailedSectionProgress>
            {
                new DetailedSectionProgress { SectionId = 2, SectionName = "Section2" },
                new DetailedSectionProgress { SectionId = 3, SectionName = "Section3" },
            };
            var testTutorialProgress1 = new List<DetailedTutorialProgress>
            {
                new DetailedTutorialProgress
                {
                    TutorialName = "Tut1", TutorialStatus = "Passed", AvgTime = 2, TimeTaken = 9, DiagnosticScore = 7,
                    PossibleScore = 8,
                },
                new DetailedTutorialProgress
                {
                    TutorialName = "Tut2", TutorialStatus = "Not Passed", AvgTime = 3, TimeTaken = 8,
                    DiagnosticScore = 5, PossibleScore = 5,
                },
            };
            var testTutorialProgress2 = new List<DetailedTutorialProgress>
            {
                new DetailedTutorialProgress
                {
                    TutorialName = "Tut3", TutorialStatus = "Not Passed", AvgTime = 4, TimeTaken = 7,
                    DiagnosticScore = 0, PossibleScore = 1,
                },
                new DetailedTutorialProgress
                {
                    TutorialName = "Tut4", TutorialStatus = "Passed", AvgTime = 5, TimeTaken = 6, DiagnosticScore = 0,
                    PossibleScore = 5,
                },
            };
            var adminField = PromptsTestHelper.GetDefaultCourseAdminFieldWithAnswer(
                2,
                "Priority Access",
                answer: "answer2"
            );
            var testCourseAdminFields = new List<CourseAdminFieldWithAnswer> { adminField };
            var testCourseInfo = new DelegateCourseInfo
            {
                DelegateLastName = "lastName",
                DelegateEmail = "email",
                DelegateId = 99,
                CandidateNumber = "five",
                LastUpdated = DateTime.UnixEpoch,
                Enrolled = DateTime.MinValue,
                Completed = DateTime.Today,
                CompleteBy = DateTime.Now,
                CustomisationId = 10,
                IsAssessed = true,
                CourseAdminFields = testCourseAdminFields
            };

            A.CallTo(() => progressDataService.GetProgressByProgressId(1)).Returns(testCourseProgress);
            A.CallTo(() => progressDataService.GetSectionProgressDataForProgressEntry(1)).Returns(testSectionProgress);
            A.CallTo(() => progressDataService.GetTutorialProgressDataForSection(1, 2)).Returns(testTutorialProgress1);
            A.CallTo(() => progressDataService.GetTutorialProgressDataForSection(1, 3)).Returns(testTutorialProgress2);
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(1)).Returns(testCourseInfo);
            A.CallTo(
                () => courseAdminFieldsService.GetCourseAdminFieldsWithAnswersForCourse(
                    testCourseInfo
                )
            ).Returns(testCourseAdminFields);

            var testSectionProgressWithTutorials = new List<DetailedSectionProgress>
            {
                new DetailedSectionProgress
                    { SectionId = 2, SectionName = "Section2", Tutorials = testTutorialProgress1 },
                new DetailedSectionProgress
                    { SectionId = 3, SectionName = "Section3", Tutorials = testTutorialProgress2 },
            };
            var expectedResult = new DetailedCourseProgress(
                testCourseProgress,
                testSectionProgressWithTutorials,
                testCourseInfo
            );

            // When
            var result = progressService.GetDetailedCourseProgress(1);

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void UpdateAdminFieldForCourse_calls_data_service_with_correct_values()
        {
            // Given
            const int progressId = 101;
            const int promptNumber = 1;
            const string answer = "Test answer";

            A.CallTo(() => progressDataService.UpdateCourseAdminFieldForDelegate(A<int>._, A<int>._, A<string>._))
                .DoesNothing();

            // When
            progressService.UpdateCourseAdminFieldForDelegate(progressId, promptNumber, answer);

            // Then
            A.CallTo(() => progressDataService.UpdateCourseAdminFieldForDelegate(progressId, promptNumber, answer))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void StoreAspProgressV2_calls_data_service_with_correct_values()
        {
            // Given
            const int progressId = 101;
            const int version = 1;
            const string? lmGvSectionRow = "Test";
            const int tutorialId = 123;
            const int tutorialTime = 2;
            const int tutorialStatus = 3;
            var timeNow = new DateTime(2022, 1, 1, 1, 1, 1, 1);

            A.CallTo(() => progressDataService.UpdateCourseAdminFieldForDelegate(A<int>._, A<int>._, A<string>._))
                .DoesNothing();
            A.CallTo(() => clockService.UtcNow)
                .Returns(timeNow);

            // When
            progressService.StoreAspProgressV2(
                progressId,
                version,
                lmGvSectionRow,
                tutorialId,
                tutorialTime,
                tutorialStatus
            );

            // Then
            A.CallTo(
                    () => progressDataService.UpdateProgressDetailsForStoreAspProgressV2(
                        progressId,
                        version,
                        timeNow,
                        lmGvSectionRow
                    )
                )
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => progressDataService.UpdateAspProgressTutTime(tutorialId, progressId, tutorialTime))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => progressDataService.UpdateAspProgressTutStat(tutorialId, progressId, tutorialStatus))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void CheckProgressForCompletion_does_nothing_if_progress_is_already_completed()
        {
            // Given
            var completedDate = new DateTime(2022, 1, 1, 1, 1, 1);
            var detailedCourseProgress = ProgressTestHelper.GetDefaultDetailedCourseProgress(completed: completedDate);

            // When
            progressService.CheckProgressForCompletionAndSendEmailIfCompleted(detailedCourseProgress);

            // Then
            A.CallTo(
                () => progressDataService.GetCompletionStatusForProgress(A<int>._)
            ).MustNotHaveHappened();
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void CheckProgressForCompletion_does_not_call_data_services_if_completionStatus_is_zero_or_less(
            int completionStatus
        )
        {
            // Given
            var detailedCourseProgress = ProgressTestHelper.GetDefaultDetailedCourseProgress();

            A.CallTo(() => progressDataService.GetCompletionStatusForProgress(detailedCourseProgress.ProgressId))
                .Returns(completionStatus);

            // When
            progressService.CheckProgressForCompletionAndSendEmailIfCompleted(detailedCourseProgress);

            // Then
            A.CallTo(
                () => progressDataService.SetCompletionDate(
                    A<int>._,
                    A<DateTime>._
                )
            ).MustNotHaveHappened();
            A.CallTo(
                () => learningLogItemsDataService.MarkLearningLogItemsCompleteByProgressId(
                    A<int>._
                )
            ).MustNotHaveHappened();
            A.CallTo(
                () => notificationService.SendProgressCompletionNotificationEmail(
                    A<DetailedCourseProgress>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void
            CheckProgressForCompletion_calls_data_services_with_correct_values_if_completionStatus_is_greater_than_zero()
        {
            // Given
            var detailedCourseProgress = ProgressTestHelper.GetDefaultDetailedCourseProgress();
            var numLearningLogItemsAffected = 3;

            A.CallTo(() => progressDataService.GetCompletionStatusForProgress(detailedCourseProgress.ProgressId))
                .Returns(1);
            A.CallTo(
                () => progressDataService.SetCompletionDate(
                    A<int>._,
                    A<DateTime>._
                )
            ).DoesNothing();
            A.CallTo(
                () => learningLogItemsDataService.MarkLearningLogItemsCompleteByProgressId(
                    A<int>._
                )
            ).Returns(numLearningLogItemsAffected);
            A.CallTo(
                () => notificationService.SendProgressCompletionNotificationEmail(
                    A<DetailedCourseProgress>._,
                    A<int>._,
                    A<int>._
                )
            ).DoesNothing();

            // When
            progressService.CheckProgressForCompletionAndSendEmailIfCompleted(detailedCourseProgress);

            // Then
            A.CallTo(() => progressDataService.GetCompletionStatusForProgress(detailedCourseProgress.ProgressId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => progressDataService.SetCompletionDate(A<int>._, A<DateTime>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => learningLogItemsDataService.MarkLearningLogItemsCompleteByProgressId(A<int>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(
                () => notificationService.SendProgressCompletionNotificationEmail(
                    A<DetailedCourseProgress>._,
                    A<int>._,
                    A<int>._
                )
            ).MustHaveHappenedOnceExactly();
        }
    }
}
