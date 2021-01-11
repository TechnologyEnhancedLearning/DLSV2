namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
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
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.CourseTitle.Should().Be($"{applicationName} - {customisationName}");
        }

        [Test]
        public void Diagnostic_assessment_should_have_section_name()
        {
            // Given
            const string sectionName = "Section name";
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment(
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
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment(
                diagAssessPath: diagnosticAssessmentPath
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
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment(
                diagObjSelect: selectTutorials
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.CanSelectTutorials.Should().BeFalse();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Diagnostic_assessment_select_tutorials_with_tutorials_should_be_diagObjSelect(
            bool selectTutorials
        )
        {
            // Given
            var tutorials = new[]
            {
                new DiagnosticTutorial("Tutorial 1", 1),
                new DiagnosticTutorial("Tutorial 2", 2)
            };
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment(
                diagObjSelect: selectTutorials
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
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment(
                diagObjSelect: selectTutorials
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
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment(
                diagAttempts: diagnosticAttempts
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
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment(
                diagAttempts: diagnosticAttempts,
                diagLast: sectionScore,
                diagAssessOutOf: maxSectionScore
            );

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.AttemptsInformation.Should().Be("10/20 - 3 attempts");
        }

        [Test]
        public void Diagnostic_assessment_should_have_customisation_id()
        {
            // Given
            const int customisationId = 11;
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment();

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
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment();

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
            var diagnosticAssessment = DiagnosticAssessmentHelper.CreateDefaultDiagnosticAssessment();
            diagnosticAssessment.Tutorials.AddRange(tutorials);

            // When
            var diagnosticAssessmentViewModel =
                new DiagnosticAssessmentViewModel(diagnosticAssessment, CustomisationId, SectionId);

            // Then
            diagnosticAssessmentViewModel.Tutorials.Should().BeEquivalentTo<DiagnosticTutorial>(tutorials);
        }
    }
}
