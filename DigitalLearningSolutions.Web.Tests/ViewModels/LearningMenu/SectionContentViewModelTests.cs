﻿namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class SectionContentViewModelTests
    {
        private IConfiguration config;
        private const int CustomisationId = 5;
        private const int SectionId = 5;
        private const string BaseUrl = "https://example.com";

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);
        }

        [Test]
        public void Section_content_should_have_title()
        {
            // Given
            const string applicationName = "Application name";
            const string customisationName = "Customisation name";
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.CourseTitle.Should().Be($"{applicationName} - {customisationName}");
        }

        [Test]
        public void Section_content_should_have_section_name()
        {
            // Given
            const string sectionName = "Section name";
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                sectionName: sectionName
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.SectionName.Should().Be(sectionName);
        }

        [Test]
        public void Section_content_without_has_learning_should_have_empty_percent_complete()
        {
            // Given
            const bool hasLearning = false;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0)
            };
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be("");
        }

        [Test]
        public void Section_content_with_no_complete_tutorials_should_have_zero_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0)
            };
            const int expectedPercentComplete = 0;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{expectedPercentComplete}% Complete");
        }

        [Test]
        public void Section_content_with_all_complete_tutorials_should_have_one_hundred_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2)
            };
            const int expectedPercentComplete = 100;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{expectedPercentComplete}% Complete");
        }

        [Test]
        public void Section_content_with_all_started_tutorials_should_have_fifty_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 1),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 1),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 1)
            };
            const int expectedPercentComplete = 50;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{expectedPercentComplete}% Complete");
        }

        [Test]
        public void Section_content_with_mixed_status_tutorials_that_dont_need_rounding_returns_correct_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 1),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 1),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2)
            };
            const int expectedPercentComplete = 50;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{expectedPercentComplete}% Complete");
        }

        [Test]
        public void Section_content_with_mixed_status_tutorials_that_need_rounding_returns_correct_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0)
            };
            const int roundedPercentComplete = 33;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{roundedPercentComplete}% Complete");
        }

        [Test]
        public void Section_content_should_have_customisation_id()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent();

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.CustomisationId.Should().Be(CustomisationId);
        }

        [Test]
        public void Section_content_post_learning_passed_should_be_false_if_pl_passes_is_zero()
        {
            // When
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(plPasses: 0);

            // Then
            sectionContent.PostLearningPassed.Should().BeFalse();
        }

        [Test]
        public void Section_content_post_learning_passed_should_be_true_if_pl_passes_is_one()
        {
            // When
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(plPasses: 1);

            // Then
            sectionContent.PostLearningPassed.Should().BeTrue();
        }

        [Test]
        public void Section_content_post_learning_passed_should_be_true_if_pl_passes_is_more_than_one()
        {
            // When
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(plPasses: 3);

            // Then
            sectionContent.PostLearningPassed.Should().BeTrue();
        }

        [Test]
        public void Post_learning_should_not_be_shown_if_no_post_learning_path()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(plAssessPath: null);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.ShowPostLearning.Should().BeFalse();
        }

        [Test]
        public void Post_learning_should_not_be_shown_if_is_assessed_is_false()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(isAssessed: false);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.ShowPostLearning.Should().BeFalse();
        }

        [Test]
        public void Post_learning_should_be_shown_if_there_is_post_learning_path_and_is_assessed_is_true()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                plAssessPath: "https://www.dls.nhs.uk/CMS/CMSContent/Course308/PLAssess/02-PLA-Entering-data/itspplayer.html",
                isAssessed: true);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.ShowPostLearning.Should().BeTrue();
        }

        [Test]
        public void Diagnostic_assessment_should_not_be_shown_if_no_diagnostic_path()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(diagAssessPath: null);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.ShowDiagnostic.Should().BeFalse();
        }

        [Test]
        public void Diagnostic_assessment_should_not_be_shown_if_diag_status_is_false()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(diagStatus: false);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.ShowDiagnostic.Should().BeFalse();
        }

        [Test]
        public void Diagnostic_assessment_should_be_shown_if_diag_status_is_true_and_diag_assessment_path_exists()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "https://www.dls.nhs.uk/CMS/CMSContent/Course308/Diagnostic/02-DIAG-Entering-data/itspplayer.html",
                diagStatus: true);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.ShowDiagnostic.Should().BeTrue();
        }

        [Test]
        public void Post_learning_status_should_be_not_attempted_if_pl_attempts_is_zero()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(attemptsPl: 0);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PostLearningStatus.Should().Be("Not Attempted");
        }

        [Test]
        public void Post_learning_status_should_be_failed_if_pl_attempts_is_more_than_zero_and_pl_passes_is_zero()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(attemptsPl: 3, plPasses: 0);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PostLearningStatus.Should().Be("Failed");
        }

        [Test]
        public void Post_learning_status_should_be_passed_if_pl_attempts_is_more_than_zero_and_pl_passes_is_more_than_zero()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(attemptsPl: 3, plPasses: 1);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PostLearningStatus.Should().Be("Passed");
        }

        [Test]
        public void Diagnostic_assessment_completion_status_is_not_attempted_if_diag_attempts_is_zero()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(diagAttempts: 0);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.DiagnosticCompletionStatus.Should().Be("Not Attempted");
        }

        [Test]
        public void Diagnostic_assessment_completion_status_shows_score_and_attempts_if_diag_attempts_is_more_than_0()
        {
            // Given
            const int diagAttempts = 4;
            const int secScore = 10;
            const int secOutOf = 15;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAttempts: diagAttempts,
                secScore: secScore,
                secOutOf: secOutOf);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.DiagnosticCompletionStatus.Should().Be($"{secScore}/{secOutOf} - {diagAttempts} attempts");
        }

        [Test]
        public void Diagnostic_assessment_separator_is_true_if_diagnostic_assessment_and_tutorials_exist()
        {
            // Given
            const string diagAssessPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course308/Diagnostic/02-DIAG-Entering-data/itspplayer.html";
            const bool diagStatus = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(),
                SectionTutorialHelper.CreateDefaultSectionTutorial()
            };
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: diagAssessPath,
                diagStatus: diagStatus
            );
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.DisplayDiagnosticSeparator.Should().BeTrue();

        }

        [Test]
        public void Diagnostic_assessment_separator_is_true_if_diagnostic_assessment_and_post_learning_assessment_exist_but_no_tutorials()
        {
            // Given
            const string diagAssessPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course308/Diagnostic/02-DIAG-Entering-data/itspplayer.html";
            const bool diagStatus = true;
            const string plAssessPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course308/PLAssess/02-PLA-Entering-data/itspplayer.html";
            const bool isAssessed = true;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: diagAssessPath,
                diagStatus: diagStatus,
                plAssessPath: plAssessPath,
                isAssessed: isAssessed
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);
            
            // Then
            sectionContentViewModel.DisplayDiagnosticSeparator.Should().BeTrue();
        }

        [Test]
        public void Diagnostic_assessment_separator_is_false_if_no_tutorials_or_post_learning_assessment()
        {
            // Given
            const string diagAssessPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course308/Diagnostic/02-DIAG-Entering-data/itspplayer.html";
            const bool diagStatus = true;
            const bool isAssessed = false;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: diagAssessPath,
                diagStatus: diagStatus,
                isAssessed: isAssessed
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.DisplayDiagnosticSeparator.Should().BeFalse();
        }

        [Test]
        public void Tutorial_separator_is_true_if_tutorials_and_post_learning_assessment_exist()
        {
            // Given
            const string plAssessPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course308/PLAssess/02-PLA-Entering-data/itspplayer.html";
            const bool isAssessed = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(),
                SectionTutorialHelper.CreateDefaultSectionTutorial()
            };
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                plAssessPath: plAssessPath,
                isAssessed: isAssessed
            );
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.DisplayTutorialSeparator.Should().BeTrue();
        }

        
        [Test]
        public void Tutorial_separator_is_false_if_no_tutorials_exist()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent();

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.DisplayTutorialSeparator.Should().BeFalse();
        }

        [Test]
        public void Tutorial_separator_is_false_if_tutorials_exist_but_no_post_learning()
        {
            // Given
            const bool isAssessed = false;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(),
                SectionTutorialHelper.CreateDefaultSectionTutorial()
            };
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(isAssessed: isAssessed);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.DisplayTutorialSeparator.Should().BeFalse();
        }

        [Test]
        public void Diagnostic_assessment_completion_status_does_not_use_plural_if_attempts_is_one()
        {
            // Given
            const int diagAttempts = 1;
            const int secScore = 10;
            const int secOutOf = 15;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAttempts: diagAttempts,
                secScore: secScore,
                secOutOf: secOutOf);

            // When
            var sectionContentViewModel = new SectionContentViewModel(config, sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.DiagnosticCompletionStatus.Should().Be($"{secScore}/{secOutOf} - {diagAttempts} attempt");
        }
    }
}
