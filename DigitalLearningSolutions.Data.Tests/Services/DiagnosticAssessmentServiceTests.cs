namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class DiagnosticAssessmentServiceTests
    {
        private IDiagnosticAssessmentDataService diagnosticAssessmentDataService;
        private IDiagnosticAssessmentService diagnosticAssessmentService;
        private DiagnosticAssessmentTestHelper diagnosticAssessmentTestHelper;
        private const int CustomisationId = 1;
        private const int CandidateId = 2;
        private const int SectionId = 3;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<DiagnosticAssessmentService>>();
            diagnosticAssessmentDataService = A.Fake<IDiagnosticAssessmentDataService>();
            diagnosticAssessmentService = new DiagnosticAssessmentService(logger, diagnosticAssessmentDataService);
            diagnosticAssessmentTestHelper = new DiagnosticAssessmentTestHelper(connection);
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_diagnostic_assessment_from_data_service()
        {
            // Given
            var diagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment();
            A.CallTo(() => diagnosticAssessmentDataService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(diagnosticAssessment);

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId);

            // Then
            result.Should().BeEquivalentTo(diagnosticAssessment);
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_null_when_data_service_returns_null()
        {
            // Given
            A.CallTo(() => diagnosticAssessmentDataService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_content_should_return_null_when_data_service_returns_null()
        {
            // Given
            var emptySelectedTutorials = new List<int>();
            A.CallTo(() => diagnosticAssessmentDataService.GetDiagnosticContent(CustomisationId, SectionId))
                .Returns(null);

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_content_should_return_diagnostic_content_from_data_service_if_selection_is_disabled()
        {
            // Given
            var emptySelectedTutorials = new List<int>();
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(canSelectTutorials: false);
            A.CallTo(() => diagnosticAssessmentDataService.GetDiagnosticContent(CustomisationId, SectionId))
                .Returns(diagnosticContent);

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            result.Should().BeEquivalentTo(diagnosticContent);
        }

        [Test]
        public void Get_diagnostic_content_should_return_diagnostic_content_from_data_service_if_selected_tutorials_are_invalid_but_selection_is_disabled()
        {
            // Given
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(canSelectTutorials: false);
            diagnosticContent.Tutorials.AddRange(new[] {
                1, 2, 3
            });
            var selectedTutorials = new List<int>(new[]
            {
                4, 5, 6
            });
            A.CallTo(() => diagnosticAssessmentDataService.GetDiagnosticContent(CustomisationId, SectionId))
                .Returns(diagnosticContent);

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, selectedTutorials);

            // Then
            result.Should().BeEquivalentTo(diagnosticContent);
        }

        [Test]
        public void Get_diagnostic_content_should_return_diagnostic_content_from_data_service_if_selected_tutorials_equal_valid_tutorials()
        {
            // Given
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(canSelectTutorials: true);
            diagnosticContent.Tutorials.AddRange(new[] {
                1, 2, 3
            });
            var selectedTutorials = new List<int>(new[]
            {
                1, 2, 3
            });
            A.CallTo(() => diagnosticAssessmentDataService.GetDiagnosticContent(CustomisationId, SectionId))
                .Returns(diagnosticContent);

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, selectedTutorials);

            // Then
            result.Should().BeEquivalentTo(diagnosticContent);
        }

        [Test]
        public void Get_diagnostic_content_should_return_diagnostic_content_from_data_service_if_no_selected_tutorials_are_invalid()
        {
            // Given
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(canSelectTutorials: true);
            diagnosticContent.Tutorials.AddRange(new[] {
                1, 2, 3
            });
            var selectedTutorials = new List<int>(new[]
            {
                1, 2
            });
            A.CallTo(() => diagnosticAssessmentDataService.GetDiagnosticContent(CustomisationId, SectionId))
                .Returns(diagnosticContent);

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, selectedTutorials);

            // Then
            result.Should().BeEquivalentTo(diagnosticContent);
        }

        [Test]
        public void Get_diagnostic_content_should_return_null_if_all_selected_tutorials_are_invalid()
        {
            // Given
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(canSelectTutorials: true);
            diagnosticContent.Tutorials.AddRange(new[] {
                1, 2, 3
            });
            var selectedTutorials = new List<int>(new[]
            {
                4, 5, 6
            });
            A.CallTo(() => diagnosticAssessmentDataService.GetDiagnosticContent(CustomisationId, SectionId))
                .Returns(diagnosticContent);

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, selectedTutorials);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_content_should_return_null_if_some_selected_tutorials_are_invalid()
        {
            // Given
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(canSelectTutorials: true);
            diagnosticContent.Tutorials.AddRange(new[] {
                1, 2, 3
            });
            var selectedTutorials = new List<int>(new[]
            {
                3, 4, 5
            });
            A.CallTo(() => diagnosticAssessmentDataService.GetDiagnosticContent(CustomisationId, SectionId))
                .Returns(diagnosticContent);

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, selectedTutorials);

            // Then
            result.Should().BeNull();
        }
    }
}
