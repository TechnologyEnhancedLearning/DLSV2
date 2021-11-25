namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class DelegateProgressControllerTests
    {
        private ICourseService courseService = null!;
        private DelegateProgressController delegateProgressController = null!;
        private INotificationService notificationService = null;
        private IProgressService progressService = null!;
        private IUserService userService = null!;

        private static IEnumerable<TestCaseData> EditSupervisorPostTestData
        {
            get
            {
                yield return new TestCaseData(DelegateProgressAccessRoute.CourseDelegates, null, "Index")
                    .SetName("EditSupervisorPost_redirects_to_course_delegates_progress");
                yield return
                    new TestCaseData(DelegateProgressAccessRoute.ViewDelegate, "ViewDelegate", "Index").SetName(
                        "EditSupervisorPost_redirects_to_view_delegate"
                    );
            }
        }

        private static IEnumerable<TestCaseData> UnlockCourseProgressData
        {
            get
            {
                yield return new TestCaseData(DelegateProgressAccessRoute.CourseDelegates, "CourseDelegates", "Index")
                    .SetName("UnlockCourseProgress_redirects_to_course_delegates_progress");
                yield return
                    new TestCaseData(DelegateProgressAccessRoute.ViewDelegate, "ViewDelegate", "Index").SetName(
                        "UnlockCourseProgress_redirects_to_view_delegate"
                    );
            }
        }

        [SetUp]
        public void Setup()
        {
            courseService = A.Fake<ICourseService>();
            userService = A.Fake<IUserService>();
            progressService = A.Fake<IProgressService>();
            notificationService = A.Fake<INotificationService>();
            delegateProgressController = new DelegateProgressController(
                    courseService,
                    userService,
                    progressService,
                    notificationService
                )
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        [TestCaseSource(
            typeof(DelegateProgressControllerTests),
            nameof(EditSupervisorPostTestData)
        )]
        public void EditSupervisorPost_redirects_to_correct_action(
            DelegateProgressAccessRoute accessedVia,
            string expectedController,
            string expectedAction
        )
        {
            // Given
            const int progressId = 1;
            const int supervisorId = 1;
            var formData = new EditSupervisorFormData { SupervisorId = supervisorId };
            A.CallTo(() => progressService.UpdateSupervisor(progressId, supervisorId)).DoesNothing();

            // When
            var result = delegateProgressController.EditSupervisor(formData, progressId, accessedVia);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName(expectedController)
                .WithActionName(expectedAction);
        }

        [Test]
        [TestCaseSource(
            typeof(DelegateProgressControllerTests),
            nameof(UnlockCourseProgressData)
        )]
        public void UnlockCourseProgress_redirects_to_correct_action_and_unlocks_progress_and_sends_notification(
            DelegateProgressAccessRoute accessedVia,
            string expectedController,
            string expectedAction
        )
        {
            // Given
            const int progressId = 1;
            const int delegateId = 1;
            const int customisationId = 1;

            A.CallTo(() => progressService.UnlockProgress(progressId)).DoesNothing();
            A.CallTo(() => notificationService.SendUnlockRequest(progressId)).DoesNothing();

            // When
            var result = delegateProgressController.UnlockProgress(
                progressId,
                customisationId,
                delegateId,
                accessedVia
            );

            // Then
            A.CallTo(() => progressService.UnlockProgress(progressId)).MustHaveHappened();
            A.CallTo(() => notificationService.SendUnlockRequest(progressId)).MustHaveHappened();

            result.Should().BeRedirectToActionResult().WithControllerName(expectedController)
                .WithActionName(expectedAction);
        }
    }
}
