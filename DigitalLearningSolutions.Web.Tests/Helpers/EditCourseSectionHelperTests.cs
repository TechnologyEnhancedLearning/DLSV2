namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
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
                EditCourseSectionHelper.SelectAllLearningAction
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                formData.Tutorials.Should().OnlyContain(t => t.LearningEnabled);
            }
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
                EditCourseSectionHelper.SelectAllDiagnosticAction
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                formData.Tutorials.Should().OnlyContain(t => t.DiagnosticEnabled);
            }
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
                EditCourseSectionHelper.DeselectAllLearningAction
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                formData.Tutorials.Should().OnlyContain(t => t.LearningEnabled == false);
            }
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
                EditCourseSectionHelper.DeselectAllDiagnosticAction
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                formData.Tutorials.Should().OnlyContain(t => t.DiagnosticEnabled == false);
            }
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
