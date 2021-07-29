namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using System.Transactions;
    using FluentAssertions;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        [Test]
        public void Get_tutorial_information_should_return_null_nextTutorial_if_last_tutorial_in_section()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 52;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_nextSection_if_last_section_in_course()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 82;
            const int tutorialId = 94;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_should_skip_tutorials_not_in_customisation()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 18366;
            const int sectionId = 973;
            const int tutorialId = 4257;

            // The next tutorial ID in this section is 4258, but the next tutorial selected in CustomisationTutorials is 4263
            const int nextTutorialId = 4263;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_should_skip_empty_sections()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 18366;
            const int sectionId = 974;
            const int tutorialId = 4262;

            // The next section ID in this Application is 975, but the next section with a tutorial selected in
            // CustomisationTutorials is 978
            const int nextSectionId = 978;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_can_return_smaller_tutorialId()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 17731;
            const int sectionId = 801;
            const int tutorialId = 3334;

            const int nextTutorialId = 3333;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_skips_archived_tutorial()
        {
            // Given
            const int candidateId = 11;
            const int customisationId = 15937;
            const int sectionId = 392;
            const int tutorialId = 1535;

            const int nextTutorialId = 1583; // Skipping over archived 1536, 1537, 1581

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_can_return_smaller_sectionId()
        {
            // Given
            const int candidateId = 210962;
            const int customisationId = 24057;
            const int sectionId = 2201;
            const int tutorialId = 10184;

            const int nextSectionId = 2193;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_skips_archived_sections()
        {
            // Given
            const int candidateId = 118178;
            const int customisationId = 22416;
            const int sectionId = 1958;
            const int tutorialId = 9349;

            const int nextSectionId = 1960; // Skips archived section 1959

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_skips_sections_full_of_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 210962;
                const int customisationId = 24057;
                const int sectionId = 2201;
                const int tutorialId = 10184;

                // The tutorials of what would be the next section, 2193;
                tutorialContentTestHelper.ArchiveTutorial(10161);
                tutorialContentTestHelper.ArchiveTutorial(10195);

                const int expectedNextSectionId = 2088;

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_tutorial_information_nextSection_returns_section_with_only_diagnostic_assessment()
        {
            using (new TransactionScope())
            {

                // Given
                const int candidateId = 74411;
                const int customisationId = 5852;
                const int sectionId = 150;
                const int tutorialId = 634;

                const int expectedNextSectionId = 151; // All tutorials are CustomisationTutorials.Status = 0, though some DiagStatus = 1
                // Remove post learning assessment
                sectionContentTestHelper.UpdatePostLearningAssessmentPath(expectedNextSectionId, null);

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_tutorial_information_nextSection_skips_section_with_just_diagnostic_assessment_but_no_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 74411;
                const int customisationId = 5852;
                const int sectionId = 148;
                const int tutorialId = 628;

                const int originalNextSectionId = 149; // All tutorials are CustomisationTutorials.Status = 0, though some DiagStatus = 1
                // Remove diagnostic and post learning paths
                sectionContentTestHelper.UpdateDiagnosticAssessmentPath(originalNextSectionId, null);
                sectionContentTestHelper.UpdatePostLearningAssessmentPath(originalNextSectionId, null);

                const int expectedNextSectionId = 150;

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_tutorial_information_nextSection_returns_section_with_only_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;
            const int tutorialId = 331;

            const int expectedNextSectionId = 105; // All tutorials are CustomisationTutorials.Status and DiagStatus = 0
                                                   // Customisations.IsAssessed = 1 and Sections.PLAssessPath is not null
                                                   // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_skips_assessed_section_with_no_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 10820;
                const int candidateId = 1;
                const int sectionId = 104;
                const int tutorialId = 331;

                const int originalNextSectionId = 105; // All tutorials are CustomisationTutorials.Status and DiagStatus = 0
                                                       // Customisations.IsAssessed = 1
                sectionContentTestHelper.UpdatePostLearningAssessmentPath(originalNextSectionId, null);
                const int expectedNextSectionId = 106;

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [TestCase(2087, 10166, 2195)]
        [TestCase(2195, 10168, 2199)]
        [TestCase(2199, 10169, 2086)]
        public void Get_tutorial_information_next_section_id_has_correct_ids_when_shared_section_number(
            int sectionId,
            int tutorialId,
            int expectedNextSectionId
        )
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 24057;
                const int candidateId = 1;
                sectionContentTestHelper.UpdateSectionNumber(2195, 10);
                // Doing this should result in the following sequence:
                // SectionID: 2087, SectionNumber: 6
                // SectionID: 2195, SectionNumber: 10
                // SectionID: 2199, SectionNumber: 10
                // SectionID: 2086, SectionNumber: 11

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_returns_smaller_tutorialId_for_shared_orderByNumber()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 8194;
            const int sectionId = 216;
            const int tutorialId = 927;

            // All in section 216
            // Tutorial: 927  OrderByNumber 34
            // Tutorial: 928  OrderByNumber 35
            // Tutorial: 929  OrderByNumber 35
            const int nextTutorialId = 928;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_returns_next_tutorialId_with_shared_orderByNumber()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 8194;
            const int sectionId = 216;
            const int tutorialId = 928;

            // All in section 216
            // Tutorial: 927  OrderByNumber 34
            // Tutorial: 928  OrderByNumber 35
            // Tutorial: 929  OrderByNumber 35
            // Tutorial: 930  OrderByNumber 36
            const int nextTutorialId = 929;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_returns_tutorialId_after_shared_orderByNumber()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 8194;
            const int sectionId = 216;
            const int tutorialId = 929;

            // All in section 216
            // Tutorial: 927  OrderByNumber 34
            // Tutorial: 928  OrderByNumber 35
            // Tutorial: 929  OrderByNumber 35
            // Tutorial: 930  OrderByNumber 36
            const int nextTutorialId = 930;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }
    }
}
