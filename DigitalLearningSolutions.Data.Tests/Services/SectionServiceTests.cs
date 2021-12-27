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
            A.CallTo(() => tutorialContentDataService.GetTutorialsBySectionIdAndCustomisationId(A<int>._, 1))
                .Returns(tutorials);

            // When
            var result = sectionService.GetSectionsAndTutorialsForCustomisation(1, 1).ToList();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => tutorialContentDataService.GetTutorialsBySectionIdAndCustomisationId(A<int>._, 1))
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
            A.CallTo(() => tutorialContentDataService.GetTutorialsBySectionIdAndCustomisationId(A<int>._, 1))
                .Returns(tutorials);

            // When
            var result = sectionService.GetSectionAndTutorialsBySectionIdForCustomisation(1, 1);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => tutorialContentDataService.GetTutorialsBySectionIdAndCustomisationId(A<int>._, 1))
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
                A.CallTo(() => tutorialContentDataService.GetTutorialsBySectionIdAndCustomisationId(A<int>._, 1))
                    .MustNotHaveHappened();
            }
        }

        // TODO: Get this test working
        /*[Test]
        public void GetSectionsForApplication_returns_fully_populated_Section()
        {
            // Given
            var tutorialOne = new Tutorial(1, "Test", false, false);
            var tutorialTwo = new Tutorial(2, "Case", false, false);
            var tutorialsOne = new List<Tutorial> { tutorialOne, tutorialTwo };

            var tutorialThree = new Tutorial(3, "Test Two", false, false);
            var tutorialFour = new Tutorial(4, "Case Two", false, false);
            var tutorialsTwo = new List<Tutorial> { tutorialThree, tutorialFour };

            var sectionOne = new Section(1, "Section One");
            var sectionTwo = new Section(2, "Section Two");

            A.CallTo(() => sectionContentDataService.GetSectionsByApplicationId(1))
                .Returns(new List<Section> {sectionOne, sectionTwo});
            A.CallTo(() => tutorialContentDataService.GetTutorialsForSection(1))
                .Returns(tutorialsOne);
            A.CallTo(() => tutorialContentDataService.GetTutorialsForSection(2))
                .Returns(tutorialsTwo);

            // When
            var result = sectionService.GetSectionsForApplication(1).ToList();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => sectionContentDataService.GetSectionsByApplicationId(1))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => tutorialContentDataService.GetTutorialsForSection(1))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => tutorialContentDataService.GetTutorialsForSection(2))
                    .MustHaveHappenedOnceExactly();

                result.Count.Should().Be(2);
                var first = result.First();
                var second = result.Last();
                first?.SectionId.Should().Be(1);
                first?.SectionName.Should().Be("Section One");
                first?.Should().BeEquivalentTo(tutorialsOne);
                second?.SectionId.Should().Be(2);
                second?.SectionName.Should().Be("Section Two");
                second?.Should().BeEquivalentTo(tutorialsTwo);
            }
        }*/
    }
}
