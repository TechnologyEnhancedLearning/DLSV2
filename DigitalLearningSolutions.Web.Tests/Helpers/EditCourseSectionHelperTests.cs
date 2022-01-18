﻿namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class EditCourseSectionHelperTests
    {
        [Test]
        public void
            ProcessBulkSelect_with_select_all_learning_action_selects_all_learning_and_returns_null()
        {
            // Given
            var formData = GetDefaultEditCourseSectionFormData();

            // When
            var result = EditCourseSectionHelper.ProcessBulkSelect(
                formData,
                CourseContentController.SelectAllLearningAction
            );

            // Then
            result.Should().BeNull();
            formData.Tutorials
                .All(t => t.LearningEnabled)
                .Should().BeTrue();
        }

        [Test]
        public void
            ProcessBulkSelect_with_select_all_diagnostic_action_selects_all_diagnostic_and_returns_null()
        {
            // Given
            var formData = GetDefaultEditCourseSectionFormData();

            // When
            var result = EditCourseSectionHelper.ProcessBulkSelect(
                formData,
                CourseContentController.SelectAllDiagnosticAction
            );

            // Then
            result.Should().BeNull();
            formData.Tutorials
                .All(t => t.DiagnosticEnabled)
                .Should().BeTrue();
        }

        [Test]
        public void
            ProcessBulkSelect_with_deselect_all_learning_action_deselects_all_learning_and_returns_null()
        {
            // Given
            var formData = GetDefaultEditCourseSectionFormData();

            // When
            var result = EditCourseSectionHelper.ProcessBulkSelect(
                formData,
                CourseContentController.DeselectAllLearningAction
            );

            // Then
            result.Should().BeNull();
            formData.Tutorials
                .All(t => !t.LearningEnabled)
                .Should().BeTrue();
        }

        [Test]
        public void
            ProcessBulkSelect_with_deselect_all_diagnostic_action_deselects_all_diagnostic_and_returns_null()
        {
            // Given
            var formData = GetDefaultEditCourseSectionFormData();

            // When
            var result = EditCourseSectionHelper.ProcessBulkSelect(
                formData,
                CourseContentController.DeselectAllDiagnosticAction
            );

            // Then
            result.Should().BeNull();
            formData.Tutorials
                .All(t => !t.DiagnosticEnabled)
                .Should().BeTrue();
        }

        [Test]
        public void ProcessBulkSelect_with_unexpected_action_returns_internal_server_error_response()
        {
            // Given
            var formData = GetDefaultEditCourseSectionFormData();

            // When
            var result = EditCourseSectionHelper.ProcessBulkSelect(formData, "Incorrect string");

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(400);
        }

        private static EditCourseSectionFormData GetDefaultEditCourseSectionFormData()
        {
            return new EditCourseSectionFormData
            {
                SectionName = "Section",
                Tutorials = new List<CourseTutorialViewModel>
                {
                    new CourseTutorialViewModel
                    {
                        TutorialId = 1,
                        TutorialName = "Tutorial 1",
                        LearningEnabled = false,
                        DiagnosticEnabled = true,
                    },
                    new CourseTutorialViewModel
                    {
                        TutorialId = 2,
                        TutorialName = "Tutorial 2",
                        LearningEnabled = true,
                        DiagnosticEnabled = false,
                    },
                },
            };
        }
    }
}
