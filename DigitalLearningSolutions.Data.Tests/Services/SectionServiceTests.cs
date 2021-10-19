namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class SectionServiceTests
    {
        private ISectionContentDataService sectionContentDataService = null!;
        private ISectionService sectionService = null!;
        private ITutorialContentDataService tutorialContentDataService = null!;

        [SetUp]
        public void Setup()
        {
            sectionContentDataService = A.Fake<ISectionContentDataService>();
            tutorialContentDataService = A.Fake<ITutorialContentDataService>();

            sectionService = new SectionService(
                sectionContentDataService,
                tutorialContentDataService
            );
        }

        [Test]
        public void GetSectionsAndTutorialsForCustomisation_returns_fully_populated_list()
        {
            // Given
            var tutorialOne = new Tutorial(1, "Test", true, true);
            var tutorialTwo = new Tutorial(2, "Case", false, false);
            var tutorials = new List<Tutorial> { tutorialOne, tutorialTwo };
            var sectionOne = new Section(1, "Section");
            var sectionTwo = new Section(2, "Second Section");
            A.CallTo(() => sectionContentDataService.GetSectionsByApplicationId(1))
                .Returns(new List<Section> { sectionOne, sectionTwo });
            A.CallTo(() => tutorialContentDataService.GetTutorialsBySectionId(A<int>._, 1))
                .Returns(tutorials);

            // When
            var result = sectionService.GetSectionsAndTutorialsForCustomisation(1, 1).ToList();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => tutorialContentDataService.GetTutorialsBySectionId(A<int>._, 1))
                    .MustHaveHappenedTwiceExactly();
                result.Count.Should().Be(2);
                result.First().SectionId.Should().Be(1);
                result.First().SectionName.Should().Be("Section");
                result.First().Tutorials.Should().BeEquivalentTo(tutorials);
            }
        }

        [Test]
        public void GetSectionAndTutorialsBySectionIdForCustomisation_returns_fully_populated_Section()
        {
            // Given
            var tutorialOne = new Tutorial(1, "Test", true, true);
            var tutorialTwo = new Tutorial(2, "Case", false, false);
            var tutorials = new List<Tutorial> { tutorialOne, tutorialTwo };
            var section = new Section(1, "Section");
            A.CallTo(() => sectionContentDataService.GetSectionById(1))
                .Returns(section);
            A.CallTo(() => tutorialContentDataService.GetTutorialsBySectionId(A<int>._, 1))
                .Returns(tutorials);

            // When
            var result = sectionService.GetSectionAndTutorialsBySectionIdForCustomisation(1, 1);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => tutorialContentDataService.GetTutorialsBySectionId(A<int>._, 1))
                    .MustHaveHappenedOnceExactly();
                result?.SectionId.Should().Be(1);
                result?.SectionName.Should().Be("Section");
                result?.Tutorials.Should().BeEquivalentTo(tutorials);
            }
        }

        [Test]
        public void GetSectionAndTutorialsBySectionIdForCustomisation_returns_null_if_section_not_found()
        {
            // Given
            A.CallTo(() => sectionContentDataService.GetSectionById(1))
                .Returns(null);

            // When
            var result = sectionService.GetSectionAndTutorialsBySectionIdForCustomisation(1, 1);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                A.CallTo(() => tutorialContentDataService.GetTutorialsBySectionId(A<int>._, 1))
                    .MustNotHaveHappened();
            }
        }
    }
}
