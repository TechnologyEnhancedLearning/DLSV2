﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    internal class CourseContentControllerTests
    {
        private readonly ICourseDataService courseDataService = A.Fake<ICourseDataService>();
        private readonly ISectionService sectionService = A.Fake<ISectionService>();
        private readonly ITutorialService tutorialService = A.Fake<ITutorialService>();
        private CourseContentController controller = null!;

        [SetUp]
        public void Setup()
        {
            controller = new CourseContentController(courseDataService, sectionService, tutorialService)
                .WithDefaultContext()
                .WithMockUser(true, 101);
        }

        [Test]
        public void Index_returns_NotFound_when_no_appropriate_course_found()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._))
                .Returns(null);

            // When
            var result = controller.Index(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_returns_Index_page_when_appropriate_course_found()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._))
                .Returns(CourseDetailsTestHelper.GetDefaultCourseDetails());
            A.CallTo(() => sectionService.GetSectionsAndTutorialsForCustomisation(A<int>._, A<int>._))
                .Returns(new List<Section>());

            // When
            var result = controller.Index(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<CourseContentViewModel>();
        }

        [Test]
        public void EditSection_returns_NotFound_when_no_appropriate_course_found()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._))
                .Returns(null);
            A.CallTo(() => sectionService.GetSectionAndTutorialsBySectionIdForCustomisation(A<int>._, A<int>._))
                .Returns(new Section(1, "Section"));

            // When
            var result = controller.EditSection(1, 1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void EditSection_returns_NotFound_when_no_matching_section_found()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._))
                .Returns(CourseDetailsTestHelper.GetDefaultCourseDetails());
            A.CallTo(() => sectionService.GetSectionAndTutorialsBySectionIdForCustomisation(A<int>._, A<int>._))
                .Returns(null);

            // When
            var result = controller.EditSection(1, 1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void EditSection_returns_EditSection_page_when_appropriate_course_and_section_found()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._))
                .Returns(CourseDetailsTestHelper.GetDefaultCourseDetails());
            A.CallTo(() => sectionService.GetSectionAndTutorialsBySectionIdForCustomisation(A<int>._, A<int>._))
                .Returns(new Section(1, "Section", new List<Tutorial>()));

            // When
            var result = controller.EditSection(1, 1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<EditCourseSectionViewModel>();
        }

        [Test]
        public void EditSection_post_with_save_action_calls_services_and_redirects_to_index()
        {
            // Given
            var postData = GetDefaultEditCourseSectionFormData();
            A.CallTo(() => tutorialService.UpdateSectionTutorialsStatuses(A<IEnumerable<Tutorial>>._, A<int>._))
                .DoesNothing();

            // When
            var result = controller.EditSection(postData, 1, CourseContentController.SaveAction);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
            A.CallTo(() => tutorialService.UpdateSectionTutorialsStatuses(A<IEnumerable<Tutorial>>._, A<int>._))
                .MustHaveHappened();
        }

        [Test]
        public void
            EditSection_post_with_select_all_learning_action_returns_EditSection_page_with_all_learning_selected()
        {
            // Given
            var postData = GetDefaultEditCourseSectionFormData();

            // When
            var result = controller.EditSection(postData, 1, CourseContentController.SelectAllLearningAction);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<EditCourseSectionViewModel>();
            result.As<ViewResult>().Model.As<EditCourseSectionViewModel>().Tutorials.All(t => t.LearningEnabled)
                .Should().BeTrue();
        }

        [Test]
        public void
            EditSection_post_with_select_all_diagnostic_action_returns_EditSection_page_with_all_diagnostic_selected()
        {
            // Given
            var postData = GetDefaultEditCourseSectionFormData();

            // When
            var result = controller.EditSection(postData, 1, CourseContentController.SelectAllDiagnosticAction);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<EditCourseSectionViewModel>();
            result.As<ViewResult>().Model.As<EditCourseSectionViewModel>().Tutorials.All(t => t.DiagnosticEnabled)
                .Should().BeTrue();
        }

        [Test]
        public void
            EditSection_post_with_deselect_all_learning_action_returns_EditSection_page_with_all_learning_deselected()
        {
            // Given
            var postData = GetDefaultEditCourseSectionFormData();

            // When
            var result = controller.EditSection(postData, 1, CourseContentController.DeselectAllLearningAction);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<EditCourseSectionViewModel>();
            result.As<ViewResult>().Model.As<EditCourseSectionViewModel>().Tutorials
                .All(t => t.LearningEnabled == false)
                .Should().BeTrue();
        }

        [Test]
        public void
            EditSection_post_with_deselect_all_diagnostic_action_returns_EditSection_page_with_all_diagnostic_deselected()
        {
            // Given
            var postData = GetDefaultEditCourseSectionFormData();

            // When
            var result = controller.EditSection(postData, 1, CourseContentController.DeselectAllDiagnosticAction);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<EditCourseSectionViewModel>();
            result.As<ViewResult>().Model.As<EditCourseSectionViewModel>().Tutorials
                .All(t => t.DiagnosticEnabled == false)
                .Should().BeTrue();
        }

        [Test]
        public void EditSection_post_with_unexpected_action_returns_internal_server_error_response()
        {
            // Given
            var postData = GetDefaultEditCourseSectionFormData();

            // When
            var result = controller.EditSection(postData, 1, "Incorrect string");

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(400);
        }

        private EditCourseSectionFormData GetDefaultEditCourseSectionFormData()
        {
            return new EditCourseSectionFormData
            {
                CourseName = "Course",
                SectionName = "Section",
                Tutorials = new List<EditCourseTutorialViewModel>
                {
                    new EditCourseTutorialViewModel
                    {
                        TutorialId = 1,
                        TutorialName = "Tutorial 1",
                        LearningEnabled = false,
                        DiagnosticEnabled = true
                    },
                    new EditCourseTutorialViewModel
                    {
                        TutorialId = 2,
                        TutorialName = "Tutorial 2",
                        LearningEnabled = true,
                        DiagnosticEnabled = false
                    }
                }
            };
        }
    }
}
