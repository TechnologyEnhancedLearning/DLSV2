namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;
    using System.Collections.Generic;

    public partial class LearningMenuControllerTests
    {
        [Test]
        public void Index_should_render_view()
        {
            // Given
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            var course = CourseContentHelper.CreateDefaultCourse();
            A.CallTo(() => courseService.GetCourse(CustomisationId)).Returns(course);
            A.CallTo(() => progressService.GetDelegateProgressForCourse(CandidateId, CustomisationId)).Returns(
               new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
           );
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
             .Returns(expectedCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(10);
            A.CallTo(() => courseContentService.GetProgressId(CandidateId, CustomisationId)).Returns(10);

            // When
            var result = controller.Index(CustomisationId, progressID);

            // Then
            var expectedModel = new InitialMenuViewModel(expectedCourseContent);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Index_should_redirect_to_section_page_if_one_section_in_course()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var section = CourseContentHelper.CreateDefaultCourseSection(id: sectionId);
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(customisationId);
            expectedCourseContent.Sections.Add(section);
            var course = CourseContentHelper.CreateDefaultCourse();
            course.CustomisationId = customisationId;

            A.CallTo(() => courseService.GetCourse(customisationId)).Returns(course);
            A.CallTo(() => progressService.GetDelegateProgressForCourse(CandidateId, customisationId)).Returns(
                new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
            );
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, customisationId))
                .Returns(expectedCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Index(customisationId, progressID);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Section")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Index_should_not_redirect_to_section_page_if_more_than_one_section_in_course()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var section1 = CourseContentHelper.CreateDefaultCourseSection(id: sectionId + 1);
            var section2 = CourseContentHelper.CreateDefaultCourseSection(id: sectionId + 2);
            var section3 = CourseContentHelper.CreateDefaultCourseSection(id: sectionId + 3);

            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(customisationId);
            expectedCourseContent.Sections.AddRange(new[] { section1, section2, section3 });
            var course = CourseContentHelper.CreateDefaultCourse();
            course.CustomisationId = customisationId;

            A.CallTo(() => courseService.GetCourse(customisationId)).Returns(course);
            A.CallTo(() => progressService.GetDelegateProgressForCourse(CandidateId, customisationId)).Returns(
               new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
           );

            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, customisationId))
                .Returns(expectedCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);
            A.CallTo(() => courseContentService.GetProgressId(CandidateId, customisationId)).Returns(10);

            // When
            var result = controller.Index(customisationId, progressID);

            // Then
            var expectedModel = new InitialMenuViewModel(expectedCourseContent);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Index_should_return_404_if_unknown_course()
        {
            // Given
            var course = CourseContentHelper.CreateDefaultCourse();

            A.CallTo(() => courseService.GetCourse(CustomisationId)).Returns(course);
            A.CallTo(() => progressService.GetDelegateProgressForCourse(CandidateId, CustomisationId)).Returns(
               new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
           );
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId)).Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(3);

            // When
            var result = controller.Index(CustomisationId, progressID);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Index_should_return_404_if_unable_to_enrol()
        {
            // Given
            var defaultCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            var course = CourseContentHelper.CreateDefaultCourse();

            A.CallTo(() => courseService.GetCourse(CustomisationId)).Returns(course);
            A.CallTo(() => progressService.GetDelegateProgressForCourse(CandidateId, CustomisationId)).Returns(
               new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
           );
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
                .Returns(defaultCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            var result = controller.Index(CustomisationId, progressID);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Index_always_calls_get_course_content()
        {
            // Given
            const int customisationId = 1;
            var course = CourseContentHelper.CreateDefaultCourse();

            A.CallTo(() => courseService.GetCourse(customisationId)).Returns(course);
            A.CallTo(() => progressService.GetDelegateProgressForCourse(CandidateId, customisationId)).Returns(
                new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
            );


            // When
            controller.Index(1,2);

            // Then
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, customisationId)).MustHaveHappened();
        }

        [Test]
        public void Index_valid_customisation_id_should_update_login_and_duration()
        {
            // Given
            const int progressId = 13;
            var defaultCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            var course = CourseContentHelper.CreateDefaultCourse();

            A.CallTo(() => courseService.GetCourse(CustomisationId)).Returns(course);
            A.CallTo(() => progressService.GetDelegateProgressForCourse(CandidateId, CustomisationId)).Returns(
                new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
            );
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
             .Returns(defaultCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(progressId);
            A.CallTo(() => courseContentService.GetProgressId(CandidateId, CustomisationId)).Returns(progressId);

            // When
            controller.Index(CustomisationId, progressID);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(CandidateId, CustomisationId, A<ISession>._)).MustHaveHappened();
        }

        [Test]
        public void Index_invalid_customisation_id_should_not_insert_new_progress()
        {
            // Given
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId)).Returns(null);

            // When
            controller.Index(CustomisationId, progressID);

            // Then
            A.CallTo(() => courseContentService.GetOrCreateProgressId(A<int>._, A<int>._, A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Index_invalid_customisation_id_should_not_update_login_and_duration()
        {
            // Given
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId)).Returns(null);

            // When
            controller.Index(CustomisationId, progressID);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Index_failing_to_insert_progress_should_not_update_progress()
        {
            // Given
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            controller.Index(CustomisationId, progressID);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Index_valid_customisationId_should_StartOrUpdate_course_sessions()
        {
            // Given
            var defaultCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            var course = CourseContentHelper.CreateDefaultCourse();

            A.CallTo(() => courseService.GetCourse(CustomisationId)).Returns(course);
            A.CallTo(() => progressService.GetDelegateProgressForCourse(CandidateId, CustomisationId)).Returns(
                new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
            );
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
                .Returns(defaultCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);
            A.CallTo(() => courseContentService.GetProgressId(CandidateId, CustomisationId)).Returns(1);

            // When
            controller.Index(CustomisationId, progressID);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Index_invalid_customisationId_should_not_StartOrUpdate_course_sessions()
        {
            // Given
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId)).Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);

            // When
            controller.Index(CustomisationId, progressID);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Index_unable_to_enrol_should_not_StartOrUpdate_course_sessions()
        {
            // Given
            var defaultCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId)).Returns(defaultCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            controller.Index(CustomisationId, progressID);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }
        //Deprecated in response to TD-3838 - a bug caused by this id manipulation detection functionality

        //[Test]
        //public void Index_detects_id_manipulation_no_progress_id()
        //{
        //    // Given
        //    var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
        //    A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
        //     .Returns(expectedCourseContent);
        //    A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(10);
        //    A.CallTo(() => courseContentService.GetProgressId(CandidateId, CustomisationId)).Returns(null);

        //    // When
        //    var result = controller.Index(CustomisationId);

        //    // Then
        //    result.Should()
        //        .BeRedirectToActionResult()
        //        .WithControllerName("LearningSolutions")
        //        .WithActionName("StatusCode")
        //        .WithRouteValue("code", 404);
        //}

        //[Test]
        //public void Index_detects_id_manipulation_self_register_false()
        //{
        //    // Given
        //    var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
        //    A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
        //     .Returns(expectedCourseContent);
        //    A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(10);
        //    A.CallTo(() => courseContentService.GetProgressId(CandidateId, CustomisationId)).Returns(null);
        //    A.CallTo(() => courseDataService.GetSelfRegister(CustomisationId)).Returns(false);

        //    // When
        //    var result = controller.Index(CustomisationId);

        //    // Then
        //    result.Should()
        //        .BeRedirectToActionResult()
        //        .WithControllerName("LearningSolutions")
        //        .WithActionName("StatusCode")
        //        .WithRouteValue("code", 404);
        //}

        [Test]
        public void Index_not_detects_id_manipulation_self_register_true()
        {
            // Given
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);

            var course = CourseContentHelper.CreateDefaultCourse();

            A.CallTo(() => courseService.GetCourse(CustomisationId)).Returns(course);
            A.CallTo(() => progressService.GetDelegateProgressForCourse(CandidateId, CustomisationId)).Returns(
                new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
            );
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
             .Returns(expectedCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(10);
            A.CallTo(() => courseContentService.GetProgressId(CandidateId, CustomisationId)).Returns(null);
            A.CallTo(() => courseService.GetSelfRegister(CustomisationId)).Returns(true);

            // When
            var result = controller.Index(CustomisationId, progressID);

            // Then
            result.Should()
                .BeViewResult();
        }
    }
}
