namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    internal class TutorialContentServiceTests
    {
        private TutorialContentService tutorialContentService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            tutorialContentService = new TutorialContentService(connection);
        }

        [Test]
        public void Get_tutorial_content_of_consistent_tutorial_should_return_tutorial()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorial = tutorialContentService.GetTutorialContent(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeEquivalentTo(new TutorialContent(
                50,
                "Navigate documents",
                "Complete",
                3,
                5,
                0,
                3,
                true,
                0,
                "<html><head><title>Tutorial Objective</title></head><body>In this tutorial you will learn to:" +
                "<ul><li>use the Go To feature to jump to a particular page</li><li>browse a document by a specific element</li></ul></body></html>",
                "/MOST/Word07Core/swf/1_1_02_Navigate_documents.swf",
                "/MOST/Word07Core/MOST_Word07_1_1_02.dcr",
                "/MOST/Word07Core/support.html?popup=1&item=navigateDocs"
            ));
        }

        [Test]
        public void Get_tutorial_content_of_inconsistent_tutorial_should_return_null()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 75;
            const int tutorialId = 50;

            // When
            var tutorial = tutorialContentService.GetTutorialContent(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }
    }
}
