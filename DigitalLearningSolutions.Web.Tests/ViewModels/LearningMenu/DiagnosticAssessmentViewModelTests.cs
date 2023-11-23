namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using System;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    public class DiagnosticAssessmentViewModelTests
    {
        private const int CustomisationId = 5;
        private const int SectionId = 5;

        [Test]
        public void Diagnostic_assessment_should_have_title()
        {
            // Given
            const string applicationName = "Application name";
            const string customisationName = "Customisation name";
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.CourseTitle.Should().Be(!String.IsNullOrEmpty(customisationName) ? $"{applicationName} - {customisationName}" : applicationName);
        }

        [Test]
        public void Diagnostic_assessment_should_have_section_name()
        {
            // Given
            const string sectionName = "Section name";
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                sectionName: sectionName
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.SectionName.Should().Be(sectionName);
        }

        [Test]
        public void Diagnostic_assessment_should_have_path()
        {
            // Given
            const string diagnosticAssessmentPath =
                "https://www.dls.nhs.uk/tracking/MOST/Excel07Core/Assess/L2_Excel_2007_Diag_7.dcr";
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                diagnosticAssessmentPath: diagnosticAssessmentPath
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.DiagnosticAssessmentPath.Should().Be(diagnosticAssessmentPath);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Diagnostic_assessment_select_tutorials_with_no_tutorials_should_always_be_false(
            bool selectTutorials
        )
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                canSelectTutorials: selectTutorials
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.CanSelectTutorials.Should().BeFalse();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Diagnostic_assessment_select_tutorials_with_tutorials_should_be_canSelectTutorials(
            bool selectTutorials
        )
        {
            // Given
            var tutorials = new[]
            {
                new DiagnosticTutorial("Tutorial 1", 1),
                new DiagnosticTutorial("Tutorial 2", 2)
            };
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                canSelectTutorials: selectTutorials
            );
            diagnosticAssessment.Tutorials.AddRange(tutorials);

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.CanSelectTutorials.Should().Be(selectTutorials);
        }

        [Test]
        public void Diagnostic_assessment_select_tutorials_can_be_false()
        {
            // Given
            const bool selectTutorials = false;
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                canSelectTutorials: selectTutorials
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.CanSelectTutorials.Should().BeFalse();
        }

        [Test]
        public void Diagnostic_assessment_attempts_information_with_no_attempts_should_be_not_attempted()
        {
            // Given
            const int diagnosticAttempts = 0;
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                diagnosticAttempts: diagnosticAttempts
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.AttemptsInformation.Should().Be("Not attempted");
        }

        [Test]
        public void Diagnostic_assessment_attempts_information_with_attempts_should_be_last_score()
        {
            // Given
            const int diagnosticAttempts = 3;
            const int sectionScore = 10;
            const int maxSectionScore = 20;
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                diagnosticAttempts: diagnosticAttempts,
                sectionScore: sectionScore,
                maxSectionScore: maxSectionScore
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.AttemptsInformation.Should().Be("10/20 - 3 attempts");
        }

        [Test]
        public void Diagnostic_assessment_can_have_next_tutorial_id()
        {
            // Given
            const int nextTutorialId = 501;
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                nextTutorialId: nextTutorialId
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Diagnostic_assessment_can_have_no_next_tutorial_id()
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                nextTutorialId: null
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.NextTutorialId.Should().BeNull();
        }

        [Test]
        public void Diagnostic_assessment_can_have_next_section_id()
        {
            // Given
            const int nextSectionId = 103;
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                nextSectionId: nextSectionId
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Diagnostic_assessment_can_have_no_next_section_id()
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                nextSectionId: null
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.NextSectionId.Should().BeNull();
        }

        [Test]
        public void Diagnostic_assessment_can_have_post_learning()
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                postLearningAssessmentPath: "https://www.dls.nhs.uk/tracking/MOST/Excel10Core/Assess/L2_Excel_2010_Post_4.dcr",
                isAssessed: true
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.HasPostLearningAssessment.Should().BeTrue();
        }

        [TestCase("https://www.dls.nhs.uk/tracking/MOST/Excel10Core/Assess/L2_Excel_2010_Post_4.dcr", false)]
        [TestCase(null, true)]
        [TestCase(null, false)]
        public void Diagnostic_assessment_can_have_no_post_learning(
            string? postLearningAssessmentPath,
            bool isAssessed
        )
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                postLearningAssessmentPath: postLearningAssessmentPath,
                isAssessed: isAssessed
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.HasPostLearningAssessment.Should().BeFalse();
        }

        [Test]
        public void Diagnostic_assessment_should_have_customisation_id()
        {
            // Given
            const int customisationId = 11;
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment();

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, customisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.CustomisationId.Should().Be(customisationId);
        }

        [Test]
        public void Diagnostic_assessment_should_have_section_id()
        {
            // Given
            const int sectionId = 22;
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment();

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, sectionId);

            // Then
            diagnosticAssessmentViewModel.SectionId.Should().Be(sectionId);
        }

        [Test]
        public void Diagnostic_assessment_should_have_tutorials()
        {
            // Given
            var tutorials = new[]
            {
                new DiagnosticTutorial("Tutorial 1", 1),
                new DiagnosticTutorial("Tutorial 2", 2)
            };
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment();
            diagnosticAssessment.Tutorials.AddRange(tutorials);

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.Tutorials.Should().BeEquivalentTo<DiagnosticTutorial>(tutorials);
        }

        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, false)]
        public void Diagnostic_assessment_should_have_onlyItemInOnlySection(
            bool otherSectionsExist,
            bool otherItemsInSectionExist,
            bool expectedOnlyItemInOnlySection
        )
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                otherSectionsExist: otherSectionsExist,
                otherItemsInSectionExist: otherItemsInSectionExist
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.OnlyItemInOnlySection.Should().Be(expectedOnlyItemInOnlySection);
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Diagnostic_assessment_should_have_onlyItemInThisSection(
            bool otherItemsInSectionExist,
            bool expectedOnlyItemInThisSection
        )
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                otherItemsInSectionExist: otherItemsInSectionExist
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.OnlyItemInThisSection.Should().Be(expectedOnlyItemInThisSection);
        }

        [TestCase(false, false, false, false)]
        [TestCase(false, false, true, true)]
        [TestCase(false, true, false, false)]
        [TestCase(false, true, true, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, true, false, false)]
        [TestCase(true, true, true, false)]
        public void Diagnostic_assessment_should_have_showCompletionSummary(
            bool otherSectionsExist,
            bool otherItemsInSectionExist,
            bool includeCertification,
            bool expectedShowCompletionSummary
        )
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                otherSectionsExist: otherSectionsExist,
                otherItemsInSectionExist: otherItemsInSectionExist,
                includeCertification: includeCertification
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.ShowCompletionSummary.Should().Be(expectedShowCompletionSummary);
        }

        [TestCase(2, "2020-12-25T15:00:00Z", 1, true, 75, 80, 85)]
        [TestCase(3, null, 0, true, 75, 80, 85)]
        [TestCase(4, null, 3, true, 75, 80, 85)]
        [TestCase(5, null, 3, false, 75, 80, 85)]
        [TestCase(6, null, 3, false, 75, 80, 0)]
        [TestCase(7, null, 3, false, 75, 0, 85)]
        [TestCase(8, null, 3, false, 75, 0, 0)]
        public void Diagnostic_assessment_should_have_completion_summary_card_view_model(
            int customisationId,
            string? completed,
            int maxPostLearningAssessmentAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold
        )
        {
            // Given
            var completedDateTime = completed != null ? DateTime.Parse(completed) : (DateTime?)null;

            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                completed: completedDateTime,
                maxPostLearningAssessmentAttempts: maxPostLearningAssessmentAttempts,
                isAssessed: isAssessed,
                postLearningAssessmentPassThreshold: postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold: diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold: tutorialsCompletionThreshold
            );

            var expectedCompletionSummaryViewModel = new CompletionSummaryCardViewModel(
                customisationId,
                completedDateTime,
                maxPostLearningAssessmentAttempts,
                isAssessed,
                postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, customisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.CompletionSummaryCardViewModel
                .Should().BeEquivalentTo(expectedCompletionSummaryViewModel);
        }

        [Test]
        public void Diagnostic_assessment_start_button_colour_should_be_grey_if_attempts_is_more_than_zero()
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                diagnosticAttempts: 2
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.DiagnosticStartButtonAdditionalStyling.Should().Be("nhsuk-button--secondary");
        }

        [Test]
        public void Diagnostic_assessment_start_button_should_have_no_colour_if_attempts_is_zero()
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                diagnosticAttempts: 0
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.DiagnosticStartButtonAdditionalStyling.Should().Be("");
        }

        [Test]
        public void Diagnostic_assessment_start_button_should_say_restart_if_attempts_is_more_than_zero()
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                diagnosticAttempts: 2
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.DiagnosticStartButtonText.Should().Be("Restart assessment");
        }

        [Test]
        public void Diagnostic_assessment_start_button_should_say_start_if_attempts_is_zero()
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                diagnosticAttempts: 0
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.DiagnosticStartButtonText.Should().Be("Start assessment");
        }

        [TestCase(false, false, 0, false)]
        [TestCase(false, true, 0, false)]
        [TestCase(true, false, 0, false)]
        [TestCase(true, true, 0, false)]
        [TestCase(false, false, 1, false)]
        [TestCase(false, true, 1, true)]
        [TestCase(true, false, 1, true)]
        [TestCase(true, true, 1, true)]
        public void Diagnostic_assessment_should_have_showNextButton(
            bool otherSectionsExist,
            bool otherItemsInSectionExist,
            int diagnosticAttempts,
            bool expectedShowNextButton
        )
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment(
                otherSectionsExist: otherSectionsExist,
                otherItemsInSectionExist: otherItemsInSectionExist,
                diagnosticAttempts: diagnosticAttempts
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.ShowNextButton.Should().Be(expectedShowNextButton);
        }
    }
}
