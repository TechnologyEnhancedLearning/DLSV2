namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        private const int CustomisationId = 1379;
        private const int SectionId = 74;

        [Test]
        public void GetTutorialsBySectionId_returns_tutorials_correctly()
        {
            // When
            var result = tutorialContentDataService.GetTutorialsBySectionId(SectionId, CustomisationId).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(4);
                result.First().TutorialId.Should().Be(49);
                result.First().TutorialName.Should().Be("View documents");
                result.First().Status.Should().BeTrue();
                result.First().DiagStatus.Should().BeTrue();
            }
        }

        [Test]
        public void GetTutorialIdsByCourse_returns_correct_tutorials()
        {
            // Given
            const int customisationId = 27240;
            var expectedTutorials = new List<int> { 9378, 9379, 9380, 9381, 9382, 9383, 9384, 9385, 9386, 9387 };

            // When
            var result = tutorialContentDataService.GetTutorialIdsForCourse(customisationId).ToList();

            // Then
            result.Should().BeEquivalentTo(expectedTutorials);
        }

        [Test]
        public void UpdateOrInsertCustomisationTutorialStatuses_updates_both_statuses_on_existing_CustomisationTutorial()
        {
            // When
            using var transaction = new TransactionScope();
            try
            {
                tutorialContentDataService.UpdateOrInsertCustomisationTutorialStatuses(49, CustomisationId, false, false);
                var result = tutorialContentDataService.GetTutorialsBySectionId(SectionId, CustomisationId).ToList();

                using (new AssertionScope())
                {
                    result.First().TutorialId.Should().Be(49);
                    result.First().TutorialName.Should().Be("View documents");
                    result.First().Status.Should().BeFalse();
                    result.First().DiagStatus.Should().BeFalse();
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateOrInsertCustomisationTutorialStatuses_inserts_new_CustomisationTutorial()
        {
            // Given
            const int sectionId = 3059;
            const int tutorialId = 12732;
            const int customisationId = 14019;

            // When
            using var transaction = new TransactionScope();
            try
            {
                var initialResult = tutorialContentDataService.GetTutorialsBySectionId(sectionId, customisationId)
                    .ToList();
                tutorialContentDataService.UpdateOrInsertCustomisationTutorialStatuses(tutorialId, customisationId, true, true);
                var result = tutorialContentDataService.GetTutorialsBySectionId(sectionId, customisationId).ToList();

                using (new AssertionScope())
                {
                    initialResult.First().Status.Should().BeNull();
                    initialResult.First().DiagStatus.Should().BeNull();
                    result.First().TutorialId.Should().Be(tutorialId);
                    result.First().TutorialName.Should().Be("Create a presentation from an outline");
                    result.First().Status.Should().BeTrue();
                    result.First().DiagStatus.Should().BeTrue();
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
