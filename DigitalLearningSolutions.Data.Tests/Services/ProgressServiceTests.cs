namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
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
    }
}
