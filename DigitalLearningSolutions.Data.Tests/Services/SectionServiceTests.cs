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
            var tutorialOne = new Tutorial(1, "Test", true, true, null, null);
            var tutorialTwo = new Tutorial(2, "Case", false, false, null, null);
            var tutorials = new List<Tutorial> { tutorialOne, tutorialTwo };
            var sectionOne = new Section(1, "Section");
            var sectionTwo = new Section(2, "Second Section");
            A.CallTo(() => sectionContentDataService.GetSectionsForApplication(1))
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
            var tutorialOne = new Tutorial(1, "Test", true, true, null, null);
            var tutorialTwo = new Tutorial(2, "Case", false, false, null, null);
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

        [Test]
        public void GetSectionsThatHaveTutorialsForApplication_calls_data_services_and_returns_expected_sections()
        {
            // Given
            var sectionOne = new Section(1, "Section");
            var sectionTwo = new Section(2, "Second Section");
            var sections = new List<Section> { sectionOne, sectionTwo };

            var tutorial = new Tutorial(1, "Tutorial", true, true, null, null);
            var tutorials = new List<Tutorial> { tutorial };

            A.CallTo(
                () => sectionContentDataService.GetSectionsForApplication(1)
            ).Returns(sections);

            A.CallTo(
                () => tutorialContentDataService.GetTutorialsForSection(A<int>._)
            ).Returns(tutorials);

            // When
            var result = sectionService.GetSectionsThatHaveTutorialsForApplication(1);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => sectionContentDataService.GetSectionsForApplication(1))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => tutorialContentDataService.GetTutorialsForSection(1)
                ).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => tutorialContentDataService.GetTutorialsForSection(2)
                ).MustHaveHappenedOnceExactly();
                result.Should().BeEquivalentTo(sections);
            }
        }

        [Test]
        public void GetSectionsThatHaveTutorialsForApplication_returns_only_sections_with_tutorials()
        {
            // Given
            var sectionOne = new Section(1, "Section");
            var sectionTwo = new Section(2, "Second Section");
            var sections = new List<Section> { sectionOne, sectionTwo };

            var tutorial = new Tutorial(1, "Tutorial", true, true, null, null);
            var tutorials = new List<Tutorial> { tutorial };

            A.CallTo(
                () => sectionContentDataService.GetSectionsForApplication(1)
            ).Returns(sections);

            A.CallTo(
                () => tutorialContentDataService.GetTutorialsForSection(1)
            ).Returns(tutorials);

            A.CallTo(
                () => tutorialContentDataService.GetTutorialsForSection(2)
            ).Returns(new List<Tutorial>());

            // When
            var result = sectionService.GetSectionsThatHaveTutorialsForApplication(1);

            // Then
            result.Count().Should().Be(1);
        }

        [Test]
        public void
            GetSectionsThatHaveTutorialsAndPopulateTutorialsForApplication_calls_data_services_and_returns_expected_sections()
        {
            // Given
            var tutorialsOne = new List<Tutorial>
            {
                new Tutorial(1, "Tutorial", null, null, null, null),
            };
            var tutorialsTwo = new List<Tutorial>
            {
                new Tutorial(2, "Second Tutorial", null, null, null, null),
            };
            var sectionOne = new Section(1, "Section");
            var sectionTwo = new Section(2, "Second Section");
            var sections = new List<Section> { sectionOne, sectionTwo };
            var sectionOneWithTutorials = new Section(1, "Section", tutorialsOne);
            var sectionTwoWithTutorials = new Section(2, "Second Section", tutorialsTwo);
            var sectionsWithTutorials = new List<Section> { sectionOneWithTutorials, sectionTwoWithTutorials };

            A.CallTo(
                () => sectionContentDataService.GetSectionsForApplication(1)
            ).Returns(sections);

            A.CallTo(
                () => tutorialContentDataService.GetTutorialsForSection(1)
            ).Returns(tutorialsOne);

            A.CallTo(
                () => tutorialContentDataService.GetTutorialsForSection(2)
            ).Returns(tutorialsTwo);

            // When
            var result = sectionService.GetSectionsThatHaveTutorialsAndPopulateTutorialsForApplication(1);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => sectionContentDataService.GetSectionsForApplication(1))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => tutorialContentDataService.GetTutorialsForSection(1)
                ).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => tutorialContentDataService.GetTutorialsForSection(2)
                ).MustHaveHappenedOnceExactly();
                result.Should().BeEquivalentTo(sectionsWithTutorials);
            }
        }

        [Test]
        public void
            GetSectionsThatHaveTutorialsAndPopulateTutorialsForApplication_returns_only_sections_with_tutorials()
        {
            // Given
            var sectionOne = new Section(1, "Section");
            var sectionTwo = new Section(2, "Second Section");
            var sections = new List<Section> { sectionOne, sectionTwo };

            var tutorial = new Tutorial(1, "Tutorial", true, true, null, null);
            var tutorials = new List<Tutorial> { tutorial };

            A.CallTo(
                () => sectionContentDataService.GetSectionsForApplication(1)
            ).Returns(sections);

            A.CallTo(
                () => tutorialContentDataService.GetTutorialsForSection(1)
            ).Returns(tutorials);

            A.CallTo(
                () => tutorialContentDataService.GetTutorialsForSection(2)
            ).Returns(new List<Tutorial>());

            // When
            var result = sectionService.GetSectionsThatHaveTutorialsAndPopulateTutorialsForApplication(1);

            // Then
            result.Count().Should().Be(1);
        }
    }
}
