namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class DelegateProgressControllerTests
    {
        private ICourseService courseService = null!;
        private DelegateProgressController delegateProgressController = null!;
        private IProgressService progressService = null!;
        private IUserService userService = null!;

        private static IEnumerable<TestCaseData> EditEndpointRedirectTestData
        {
            get
            {
                yield return new TestCaseData(DelegateAccessRoute.CourseDelegates, null, "Index")
                    .SetName("EditPost_redirects_to_course_delegates_progress");
                yield return
                    new TestCaseData(DelegateAccessRoute.ViewDelegate, "ViewDelegate", "Index").SetName(
                        "EditPost_redirects_to_view_delegate"
                    );
            }
        }

        private static IEnumerable<TestCaseData> UnlockCourseProgressData
        {
            get
            {
                yield return new TestCaseData(DelegateAccessRoute.CourseDelegates, "CourseDelegates", "Index")
                    .SetName("UnlockCourseProgress_redirects_to_course_delegates_progress");
                yield return
                    new TestCaseData(DelegateAccessRoute.ViewDelegate, "ViewDelegate", "Index").SetName(
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
            var config = A.Fake<IConfiguration>();
            delegateProgressController = new DelegateProgressController(
                    courseService,
                    userService,
                    progressService,
                    config
                )
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        [TestCaseSource(
            typeof(DelegateProgressControllerTests),
            nameof(EditEndpointRedirectTestData)
        )]
        public void EditSupervisorPost_redirects_to_correct_action(
            DelegateAccessRoute accessedVia,
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
            nameof(EditEndpointRedirectTestData)
        )]
        public void EditCompleteByDatePost_redirects_to_correct_action(
            DelegateAccessRoute accessedVia,
            string expectedController,
            string expectedAction
        )
        {
            // Given
            const int progressId = 1;
            var formData = new EditCompleteByDateFormData { Day = 1, Month = 1, Year = 2021 };
            A.CallTo(() => progressService.UpdateCompleteByDate(progressId, A<DateTime?>._)).DoesNothing();

            // When
            var result = delegateProgressController.EditCompleteByDate(formData, progressId, accessedVia);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName(expectedController)
                .WithActionName(expectedAction);
        }

        [Test]
        [TestCaseSource(
            typeof(DelegateProgressControllerTests),
            nameof(EditEndpointRedirectTestData)
        )]
        public void EditCompletionDatePost_redirects_to_correct_action(
            DelegateAccessRoute accessedVia,
            string expectedController,
            string expectedAction
        )
        {
            // Given
            const int progressId = 1;
            var formData = new EditCompletionDateFormData { Day = 1, Month = 1, Year = 2021 };
            A.CallTo(() => progressService.UpdateCompletionDate(progressId, A<DateTime?>._)).DoesNothing();

            // When
            var result = delegateProgressController.EditCompletionDate(formData, progressId, accessedVia);

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
            DelegateAccessRoute accessedVia,
            string expectedController,
            string expectedAction
        )
        {
            // Given
            const int progressId = 1;
            const int delegateId = 1;
            const int customisationId = 1;

            A.CallTo(() => progressService.UnlockProgress(progressId)).DoesNothing();

            // When
            var result = delegateProgressController.UnlockProgress(
                progressId,
                customisationId,
                delegateId,
                accessedVia
            );

            // Then
            A.CallTo(() => progressService.UnlockProgress(progressId)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithControllerName(expectedController)
                .WithActionName(expectedAction);
        }
    }
}
