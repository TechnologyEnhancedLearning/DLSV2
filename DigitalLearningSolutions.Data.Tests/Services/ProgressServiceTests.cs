namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class ProgressServiceTests
    {
        private IConfiguration configuration = null!;
        private ICourseDataService courseDataService = null!;
        private ICourseService courseService = null!;
        private IEmailService emailService = null!;
        private IFeatureManager featureManager = null!;
        private IProgressDataService progressDataService = null!;
        private IProgressService progressService = null!;
        private ISessionService sessionService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            courseDataService = A.Fake<ICourseDataService>();
            progressDataService = A.Fake<IProgressDataService>();
            configuration = A.Fake<IConfiguration>();
            courseService = A.Fake<ICourseService>();
            featureManager = A.Fake<IFeatureManager>();
            userService = A.Fake<IUserService>();
            emailService = A.Fake<IEmailService>();
            sessionService = A.Fake<ISessionService>();

            progressService = new ProgressService(
                configuration,
                courseDataService,
                courseService,
                emailService,
                featureManager,
                progressDataService,
                sessionService,
                userService
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
            var testCourseInfo = new DelegateCourseInfo
            {
                DelegateLastName = "lastName",
                DelegateEmail = "email",
                DelegateId = 99,
                DelegateNumber = "five",
                LastUpdated = DateTime.UnixEpoch,
                Enrolled = DateTime.MinValue,
                Completed = DateTime.Today,
                CompleteBy = DateTime.Now,
            };

            A.CallTo(() => progressDataService.GetProgressByProgressId(1)).Returns(testCourseProgress);
            A.CallTo(() => progressDataService.GetSectionProgressDataForProgressEntry(1)).Returns(testSectionProgress);
            A.CallTo(() => progressDataService.GetTutorialProgressDataForSection(1, 2)).Returns(testTutorialProgress1);
            A.CallTo(() => progressDataService.GetTutorialProgressDataForSection(1, 3)).Returns(testTutorialProgress2);
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(1)).Returns(testCourseInfo);

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
    }
}
