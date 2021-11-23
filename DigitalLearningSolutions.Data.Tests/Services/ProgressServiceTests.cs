namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using NUnit.Framework;

    public class ProgressServiceTests
    {
        private ICourseDataService courseDataService = null!;
        private IProgressDataService progressDataService = null!;
        private IProgressService progressService = null!;

        [SetUp]
        public void SetUp()
        {
            courseDataService = A.Fake<ICourseDataService>();
            progressDataService = A.Fake<IProgressDataService>();

            progressService = new ProgressService(courseDataService, progressDataService);
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
                () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(progressId, newSupervisorId, A<DateTime?>._)
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
            A.CallTo(() => courseDataService.SetCompletionDate(progressId, completeByDate))
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
    }
}
