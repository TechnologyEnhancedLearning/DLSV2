namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public partial class LearningMenuControllerTests
    {
        [Test]
        public void Section_should_StartOrUpdate_course_sessions_if_valid_section()
        {
            // Given
            const int progressId = 299;
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentDataService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Section_should_not_StartOrUpdate_course_sessions_if_session_not_found()
        {
            // Given
            A.CallTo(() => sectionContentDataService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Section_should_not_StartOrUpdate_course_sessions_if_unable_to_enrol()
        {
            // Given
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentDataService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Section_should_UpdateProgress_if_valid_section()
        {
            // Given
            const int progressId = 299;
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentDataService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustHaveHappened();
        }

        [Test]
        public void Section_should_not_UpdateProgress_if_invalid_section()
        {
            // Given
            A.CallTo(() => sectionContentDataService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Section_should_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentDataService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Section_should_render_view()
        {
            // Given
            const int progressId = 299;
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentDataService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).
                Returns(progressId);

            // When
            var result = controller.Section(CustomisationId, SectionId);

            // Then
            var expectedModel = new SectionContentViewModel(config, defaultSectionContent, CustomisationId, SectionId);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Section_should_redirect_to_tutorial_page_if_one_tutorial_in_section()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Tutorial")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId)
                .WithRouteValue("tutorialId", tutorialId);
        }

        [Test]
        public void Section_should_redirect_to_tutorial_page_if_one_tutorial_and_has_no_diagnostic_tutorials()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                diagStatus: false,
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Tutorial")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId)
                .WithRouteValue("tutorialId", tutorialId);
        }

        [Test]
        public void Section_should_redirect_to_tutorial_page_if_one_tutorial_and_has_diagnostic_but_no_path()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                diagStatus: true,
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Tutorial")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId)
                .WithRouteValue("tutorialId", tutorialId);
        }

        [Test]
        public void Section_should_redirect_to_post_learning_assessment_if_only_post_learning_in_section()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials is empty

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("PostLearning")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Section_should_redirect_to_post_learning_assessment_if_there_is_diagnostic_path_but_no_diagnostic_tutorials()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                diagStatus: false,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials is empty

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("PostLearning")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Section_should_redirect_to_post_learning_assessment_if_there_is_diagnostic_tutorial_but_no_path()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                diagStatus: true,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials; viewable tutorials, is empty

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("PostLearning")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Section_should_redirect_to_diagnostic_assessment_if_only_diagnostic_in_section()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                diagStatus: true,
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials is empty

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Diagnostic")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Section_should_redirect_to_diagnostic_assessment_if_there_is_post_learning_path_but_is_not_assessed()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                diagStatus: true,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: false,
                consolidationPath: null,
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials is empty

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Diagnostic")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Section_should_redirect_to_diagnostic_assessment_if_is_assessed_but_there_is_no_post_learning_path()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                diagStatus: true,
                plAssessPath: null,
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials; viewable tutorials, is empty

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Diagnostic")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Section_should_return_section_page_if_there_is_diagnostic_and_tutorial()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                diagStatus: true,
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Section_should_return_section_page_if_there_is_diagnostic_and_post_learning_assessments()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                diagStatus: true,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials; viewable tutorials, is empty

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Section_should_return_section_page_if_there_is_post_learning_assessment_and_tutorial()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Section_should_redirect_to_tutorial_page_if_one_tutorial_and_is_not_assessed()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: false,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Tutorial")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId)
                .WithRouteValue("tutorialId", tutorialId);
        }

        [Test]
        public void Section_should_redirect_to_tutorial_page_if_one_tutorial_and_is_assessed_but_has_no_path()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: null,
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Tutorial")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId)
                .WithRouteValue("tutorialId", tutorialId);
        }

        [Test]
        public void Section_should_return_section_page_if_there_is_one_tutorial_and_consolidation()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: null,
                consolidationPath: "some/consolidation/path.pdf",
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Section_should_return_section_page_if_there_is_post_learning_assessment_and_consolidation()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: "some/consolidation/path.pdf",
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials; viewable tutorials, is empty

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Section_should_return_section_page_if_there_is_diagnostic_assessment_and_consolidation()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                diagStatus: true,
                plAssessPath: null,
                consolidationPath: "some/consolidation/path.pdf",
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials; viewable tutorials, is empty

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Section_should_return_section_page_if_more_than_one_tutorial_in_section()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial1 = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId + 1);
            var tutorial2 = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId + 2);
            var tutorial3 = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId + 3);

            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.AddRange(new[] { tutorial1, tutorial2, tutorial3 });

            A.CallTo(() => sectionContentDataService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Section_should_404_if_section_not_found()
        {
            // Given
            A.CallTo(() => sectionContentDataService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            var result = controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.GetOrCreateProgressId(A<int>._, A<int>._, A<int>._)).MustNotHaveHappened();
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Section_should_404_if_failed_to_enrol()
        {
            // Given
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentDataService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            var result = controller.Section(CustomisationId, SectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }
    }
}
