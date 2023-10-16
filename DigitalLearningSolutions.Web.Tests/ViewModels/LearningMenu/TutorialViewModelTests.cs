namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class TutorialViewModelTests
    {
        private IConfiguration config;
        private const string BaseUrl = "https://example.com";
        private const int CustomisationId = 1;
        private const int SectionId = 10;
        private const int TutorialId = 1000;

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);
        }

        [Test]
        public void Tutorial_should_have_tutorialName()
        {
            // Given
            const string tutorialName = "Tutorial Name";
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                name: tutorialName
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.TutorialName.Should().Be(tutorialName);
        }

        [Test]
        public void Tutorial_should_have_sectionName()
        {
            // Given
            const string sectionName = "Section Name";
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                sectionName: sectionName
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.SectionName.Should().Be(sectionName);
        }

        [Test]
        public void Tutorial_should_have_courseTitle()
        {
            // Given
            const string applicationName = "Application";
            const string customisationName = "Customisation";
            var courseTitle = !String.IsNullOrEmpty(customisationName) ? $"{applicationName} - {customisationName}" : applicationName;

            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.CourseTitle.Should().Be(courseTitle);
        }

        [Test]
        public void Tutorial_should_have_status()
        {
            // Given
            const string status = "Not started";
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                status: status
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.Status.Should().Be(status);
        }

        [Test]
        public void Tutorial_should_have_tutorialPath()
        {
            // Given
            const string tutorialPath = "Tutorial path";
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                tutorialPath: tutorialPath
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.TutorialPath.Should().Be(tutorialPath);
        }

        [Test]
        public void Tutorial_should_have_videoPath()
        {
            // Given
            const string videoPath = "Video path";
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                videoPath: videoPath
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.VideoPath.Should().Be(videoPath);
        }

        [Test]
        public void Tutorial_should_have_supportingMaterialsLabel_from_courseSetting()
        {
            // Given
            const string supportingMaterialsText = "Different supporting information description";
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                courseSettings: "{\"lm.si\":\"" + supportingMaterialsText + "\"}"
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.SupportingMaterialsLabel.Should().Be(supportingMaterialsText);
        }

        [Test]
        public void Tutorial_should_have_default_supportingMaterialsLabel_when_courseSetting_is_empty()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                courseSettings: null
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.SupportingMaterialsLabel.Should().Be("Download supporting materials");
        }

        [Test]
        public void Tutorial_should_have_showLearnStatus_from_courseSetting()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                courseSettings: "{\"lm.sl\":false}"
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.ShowLearnStatus.Should().BeFalse();
        }

        [Test]
        public void Tutorial_should_have_default_showLearnStatus_when_courseSetting_is_empty()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                courseSettings: null
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.ShowLearnStatus.Should().BeTrue();
        }

        [Test]
        public void Tutorial_should_have_customisationId()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation();

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.CustomisationId.Should().Be(CustomisationId);
        }

        [Test]
        public void Tutorial_should_have_sectionId()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation();

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.SectionId.Should().Be(SectionId);
        }

        [Test]
        public void Tutorial_should_have_tutorialId()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.TutorialId.Should().Be(TutorialId);
        }

        [Test]
        public void Tutorial_should_have_nextLinkViewModel()
        {
            // Given
            const string postLearningAssessmentPath = "/assessment";
            const int nextTutorialId = 101;
            const int nextSectionId = 100;
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                nextTutorialId: nextTutorialId,
                nextSectionId: nextSectionId,
                postLearningAssessmentPath: postLearningAssessmentPath
            );
            var expectedNextLinkViewModel = new TutorialNextLinkViewModel(
                CustomisationId,
                SectionId,
                postLearningAssessmentPath,
                nextTutorialId,
                nextSectionId
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.NextLinkViewModel.Should().BeEquivalentTo(expectedNextLinkViewModel);
        }

        [TestCase(0, 1, true, true)]
        [TestCase(1, 30, true, false)]
        [TestCase(30, 120, false, true)]
        [TestCase(120, 61, false, false)]
        [TestCase(61, 195, true, true)]
        [TestCase(195, 0, true, false)]
        public void Tutorial_should_have_timeSummary(
            int timeSpent,
            int averageTutorialDuration,
            bool showTime,
            bool showLearnStatus
        )
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                timeSpent: timeSpent,
                averageTutorialDuration: averageTutorialDuration,
                courseSettings: "{\"lm.st\":" + showTime.ToString().ToLower()
                             + ", \"lm.sl\":" + showLearnStatus.ToString().ToLower() + "}"

            );
            var expectedTimeSummary = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTutorialDuration,
                showTime,
                showLearnStatus
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.TimeSummary.Should().BeEquivalentTo(expectedTimeSummary);
        }

        [Test]
        public void Tutorial_should_summarise_score()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                currentScore: 9,
                possibleScore: 10
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.ScoreSummary.Should().Be("(score: 9 out of 10)");
        }

        [Test]
        public void Tutorial_should_decide_to_show_progress()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                canShowDiagnosticStatus: true,
                attemptCount: 1
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.CanShowProgress.Should().BeTrue();
        }

        [Test]
        public void Tutorial_should_not_decide_to_show_progress_when_attempt_count_is_0()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                canShowDiagnosticStatus: true,
                attemptCount: 0
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.CanShowProgress.Should().BeFalse();
        }

        [Test]
        public void Tutorial_should_not_decide_to_show_progress_when_canShowDiagnosticStatus_is_false()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                canShowDiagnosticStatus: false,
                attemptCount: 1
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.CanShowProgress.Should().BeFalse();
        }

        [Test]
        public void Tutorial_should_not_decide_to_show_progress_when_showLearnStatus_courseSetting_is_false()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                canShowDiagnosticStatus: true,
                attemptCount: 1,
                courseSettings: "{\"lm.sl\":false}"
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.CanShowProgress.Should().BeFalse();
        }

        [Test]
        public void Tutorial_should_recommend_a_tutorial_when_score_is_not_max()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                currentScore: 9,
                possibleScore: 10
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.TutorialRecommendation.Should().Be("recommended");
        }

        [Test]
        public void Tutorial_should_not_recommend_a_tutorial_when_score_is_max()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                currentScore: 10,
                possibleScore: 10
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.TutorialRecommendation.Should().Be("optional");
        }

        [Test]
        public void Tutorial_handles_null_objectives()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                objectives: null
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.Objectives.Should().BeNull();
        }

        [Test]
        public void Tutorial_parses_inline_html_objectives()
        {
            // Given
            const string objectives =
                "Here are some example objectives: <ul> <li> objective 1 </li> <li> objective 2 </li> </ul>";
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                objectives: objectives
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.Objectives.Should().Be(objectives);
        }

        [Test]
        public void Tutorial_parses_html_document_of_objectives()
        {
            // Given
            const string objectives =
                "<html> <head> <title>Tutorial Objective</title> </head>" +
                "<body> In this tutorial you will learn to: " +
                  "<ul>" +
                    "<li>open another window on to a workbook</li>" +
                    "<li>arrange workbook windows</li>" +
                    "<li>hide and show windows</li>" +
                 "</ul>" +
                "</body> </html>";
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                objectives: objectives
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.Objectives.Should().Be(" In this tutorial you will learn to: " +
                                                     "<ul>" +
                                                       "<li>open another window on to a workbook</li>" +
                                                       "<li>arrange workbook windows</li>" +
                                                       "<li>hide and show windows</li>" +
                                                     "</ul>");
        }

        [TestCase("https://example.com/testVideo.mp4")]
        [TestCase("example.com/testVideo.mp4")]
        [TestCase("/testVideo.mp4")]
        [TestCase(null)]
        public void Tutorial_parses_supporting_materials_path(string? supportingMaterialPath)
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                supportingMaterialPath: supportingMaterialPath
            );
            var expectedParsedPath = ContentUrlHelper.GetNullableContentPath(config, supportingMaterialPath);

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.SupportingMaterialPath.Should().Be(expectedParsedPath);
        }

        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, false)]
        public void Tutorial_should_have_onlyItemInOnlySection(
            bool otherSectionsExist,
            bool otherItemsInSectionExist,
            bool expectedOnlyItemInOnlySection
        )
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                otherSectionsExist: otherSectionsExist,
                otherItemsInSectionExist: otherItemsInSectionExist
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.OnlyItemInOnlySection.Should().Be(expectedOnlyItemInOnlySection);
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Tutorial_should_have_onlyItemInThisSection(
            bool otherItemsInSectionExist,
            bool expectedOnlyItemInThisSection
        )
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                otherItemsInSectionExist: otherItemsInSectionExist
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.OnlyItemInThisSection.Should().Be(expectedOnlyItemInThisSection);
        }

        [TestCase(false, false, false, false)]
        [TestCase(false, false, true, true)]
        [TestCase(false, true, false, false)]
        [TestCase(false, true, true, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, true, false, false)]
        [TestCase(true, true, true, false)]
        public void Tutorial_should_have_showCompletionSummary(
            bool otherSectionsExist,
            bool otherItemsInSectionExist,
            bool includeCertification,
            bool expectedShowCompletionSummary
        )
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                otherSectionsExist: otherSectionsExist,
                otherItemsInSectionExist: otherItemsInSectionExist,
                includeCertification: includeCertification
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.ShowCompletionSummary.Should().Be(expectedShowCompletionSummary);
        }

        [TestCase("Not started", false, false, false)]
        [TestCase("Not started", false, true, false)]
        [TestCase("Not started", true, false, false)]
        [TestCase("Not started", true, true, false)]
        [TestCase("Started", false, false, false)]
        [TestCase("Started", false, true, false)]
        [TestCase("Started", true, false, false)]
        [TestCase("Started", true, true, false)]
        [TestCase("Complete", false, false, false)]
        [TestCase("Complete", false, true, true)]
        [TestCase("Complete", true, false, true)]
        [TestCase("Complete", true, true, true)]
        public void Tutorial_should_have_showNextButton(
            string status,
            bool otherSectionsExist,
            bool otherItemsInSectionExist,
            bool expectedShowNextButton
        )
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                status: status,
                otherSectionsExist: otherSectionsExist,
                otherItemsInSectionExist: otherItemsInSectionExist
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.ShowNextButton.Should().Be(expectedShowNextButton);
        }

        [Test]
        public void Tutorial_should_show_completion_summary_when_include_certification_and_only_tutorial_and_section()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                otherSectionsExist: false,
                otherItemsInSectionExist: false,
                includeCertification: true
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.ShowCompletionSummary.Should().BeTrue();
        }

        [Test]
        public void Tutorial_should_not_show_completion_summary_when_include_certification_is_false()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                otherSectionsExist: false,
                otherItemsInSectionExist: false,
                includeCertification: false
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.ShowCompletionSummary.Should().BeFalse();
        }

        [Test]
        public void Tutorial_should_not_show_completion_summary_when_other_sections_exist()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                otherSectionsExist: true,
                otherItemsInSectionExist: false,
                includeCertification: true
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.ShowCompletionSummary.Should().BeFalse();
        }

        [Test]
        public void Tutorial_should_not_show_completion_summary_when_other_items_in_section_exist()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                otherSectionsExist: false,
                otherItemsInSectionExist: true,
                includeCertification: true
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.ShowCompletionSummary.Should().BeFalse();
        }

        [TestCase(2, "2020-12-25T15:00:00Z", 1, true, 75, 80, 85)]
        [TestCase(3, null, 0, true, 75, 80, 85)]
        [TestCase(4, null, 3, true, 75, 80, 85)]
        [TestCase(5, null, 3, false, 75, 80, 85)]
        [TestCase(6, null, 3, false, 75, 80, 0)]
        [TestCase(7, null, 3, false, 75, 0, 85)]
        [TestCase(8, null, 3, false, 75, 0, 0)]
        public void Tutorial_should_have_completion_summary_card_view_model(
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

            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
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
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                customisationId,
                SectionId
            );

            // Then
            tutorialViewModel.CompletionSummaryCardViewModel
                .Should().BeEquivalentTo(expectedCompletionSummaryViewModel);
        }

        [Test]
        public void Tutorial_start_button_should_be_grey_if_tutorial_status_is_complete()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                status: "Complete"
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.TutorialStartButtonAdditionalStyling.Should().Be("nhsuk-button--secondary");
        }

        [TestCase("Started")]
        [TestCase("Not started")]
        public void Tutorial_start_button_should_have_no_extra_colour_if_tutorial_status_is_not_complete(string status)
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                status: status
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.TutorialStartButtonAdditionalStyling.Should().Be("");
        }

        [Test]
        public void Tutorial_start_button_should_say_restart_tutorial_if_tutorial_status_is_complete()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                status: "Complete"
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.TutorialStartButtonText.Should().Be("Restart tutorial");
        }

        [TestCase("Started")]
        [TestCase("Not started")]
        public void Tutorial_start_button_should_say_start_tutorial_if_tutorial_status_is_not_complete(string status)
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                status: status
            );

            // When
            var tutorialViewModel = new TutorialViewModel(
                config,
                expectedTutorialInformation,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialViewModel.TutorialStartButtonText.Should().Be("Start tutorial");
        }
    }
}
