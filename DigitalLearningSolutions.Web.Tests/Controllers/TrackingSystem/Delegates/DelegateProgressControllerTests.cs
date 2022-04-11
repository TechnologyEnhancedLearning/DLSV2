namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
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
        private ICourseAdminFieldsService courseAdminFieldsService = null!;
        private ICourseService courseService = null!;
        private DelegateProgressController delegateProgressController = null!;
        private IProgressService progressService = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
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
                yield return new TestCaseData(
                        DelegateAccessRoute.CourseDelegates,
                        "CourseDelegates",
                        "Index",
                        ReturnPageQueryHelper.GetDefaultReturnPageQuery()
                    )
                    .SetName("UnlockCourseProgress_redirects_to_course_delegates_progress");
                yield return
                    new TestCaseData(DelegateAccessRoute.ViewDelegate, "ViewDelegate", "Index", null).SetName(
                        "UnlockCourseProgress_redirects_to_view_delegate"
                    );
            }
        }

        [SetUp]
        public void Setup()
        {
            courseService = A.Fake<ICourseService>();
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();
            userService = A.Fake<IUserService>();
            progressService = A.Fake<IProgressService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            var config = A.Fake<IConfiguration>();
            delegateProgressController = new DelegateProgressController(
                    courseService,
                    courseAdminFieldsService,
                    userService,
                    progressService,
                    config,
                    searchSortFilterPaginateService
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
        public void
            EditDelegateCourseAdminField_GET_returns_not_found_if_no_course_admin_field_corresponds_to_prompt_number()
        {
            // Given
            const int progressId = 1;
            const int customisationId = 100;
            const int validPromptNumber = 1;
            const int invalidPromptNumber = 2;
            const string promptText = "Prompt text";
            const string options = "Answer";

            var delegateCourseInfo = new DelegateCourseInfo();
            var courseAdminFieldWithAnswer = new CourseAdminFieldWithAnswer(
                validPromptNumber,
                promptText,
                options,
                options
            );
            var attemptStats = new AttemptStats(1, 1);
            var delegateCourseDetails = new DelegateCourseDetails(
                delegateCourseInfo,
                new List<CourseAdminFieldWithAnswer> { courseAdminFieldWithAnswer },
                attemptStats
            );

            var courseAdminField = new CourseAdminField(validPromptNumber, promptText, options);
            var courseAdminFieldsForCourse = new CourseAdminFields(
                customisationId,
                new List<CourseAdminField> { courseAdminField }
            );

            A.CallTo(() => courseService.GetDelegateCourseProgress(progressId))
                .Returns(delegateCourseDetails);
            A.CallTo(() => courseAdminFieldsService.GetCourseAdminFieldsForCourse(customisationId))
                .Returns(courseAdminFieldsForCourse);

            // When
            var result = delegateProgressController.EditDelegateCourseAdminField(
                invalidPromptNumber,
                progressId,
                DelegateAccessRoute.CourseDelegates
            );

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        [TestCaseSource(
            typeof(DelegateProgressControllerTests),
            nameof(EditEndpointRedirectTestData)
        )]
        public void EditDelegateCourseAdminField_POST_calls_service_and_redirects_to_correct_action(
            DelegateAccessRoute accessedVia,
            string expectedController,
            string expectedAction
        )
        {
            // Given
            const int progressId = 1;
            const int promptNumber = 1;
            const string answer = "Test Answer";
            var formData = new EditDelegateCourseAdminFieldFormData { Answer = answer };

            A.CallTo(() => progressService.UpdateCourseAdminFieldForDelegate(A<int>._, A<int>._, A<string>._))
                .DoesNothing();

            // When
            var result = delegateProgressController.EditDelegateCourseAdminField(
                formData,
                promptNumber,
                progressId,
                accessedVia
            );

            // Then
            A.CallTo(() => progressService.UpdateCourseAdminFieldForDelegate(progressId, promptNumber, answer))
                .MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithControllerName(expectedController)
                .WithActionName(expectedAction);
        }

        [Test]
        public void EditDelegateCourseAdminField_POST_does_not_call_service_with_invalid_model()
        {
            // Given
            const int progressId = 1;
            const int promptNumber = 1;
            const string answer = "Test Answer";

            var delegateCourseInfo = new DelegateCourseInfo();
            var courseAdminField = new CourseAdminFieldWithAnswer(promptNumber, "Prompt text", "Answer", null);
            var attemptStats = new AttemptStats(1, 1);
            var delegateCourseDetails = new DelegateCourseDetails(
                delegateCourseInfo,
                new List<CourseAdminFieldWithAnswer> { courseAdminField },
                attemptStats
            );

            var formData = new EditDelegateCourseAdminFieldFormData { Answer = answer };

            A.CallTo(() => courseService.GetDelegateCourseProgress(progressId))
                .Returns(delegateCourseDetails);
            delegateProgressController.ModelState.AddModelError("Answer", "Test error message");

            // When
            var result = delegateProgressController.EditDelegateCourseAdminField(
                formData,
                promptNumber,
                progressId,
                DelegateAccessRoute.CourseDelegates
            );

            // Then
            A.CallTo(() => progressService.UpdateCourseAdminFieldForDelegate(A<int>._, A<int>._, A<string>._))
                .MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditDelegateCourseAdminFieldViewModel>();
        }

        [Test]
        [TestCaseSource(
            typeof(DelegateProgressControllerTests),
            nameof(UnlockCourseProgressData)
        )]
        public void UnlockCourseProgress_redirects_to_correct_action_and_unlocks_progress_and_sends_notification(
            DelegateAccessRoute accessedVia,
            string expectedController,
            string expectedAction,
            ReturnPageQuery? returnPageQuery
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
                accessedVia,
                returnPageQuery
            );

            // Then
            A.CallTo(() => progressService.UnlockProgress(progressId)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithControllerName(expectedController)
                .WithActionName(expectedAction);
        }
    }
}
