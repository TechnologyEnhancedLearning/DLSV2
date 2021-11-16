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

        [SetUp]
        public void Setup()
        {
            courseService = A.Fake<ICourseService>();
            userService = A.Fake<IUserService>();
            progressService = A.Fake<IProgressService>();
            delegateProgressController = new DelegateProgressController(courseService, userService, progressService)
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
    }
}
