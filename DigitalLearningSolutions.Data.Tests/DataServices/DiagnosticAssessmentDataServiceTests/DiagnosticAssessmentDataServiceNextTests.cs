namespace DigitalLearningSolutions.Data.Tests.DataServices.DiagnosticAssessmentDataServiceTests
{
    using System.Transactions;
    using FluentAssertions;
    using NUnit.Framework;

    internal partial class DiagnosticAssessmentDataServiceTests
    {
        [Test]
        public void Get_diagnostic_assessment_should_return_null_nextTutorial_if_no_tutorials_in_section()
        {
            // Given
            const int candidateId = 254480;
            const int customisationId = 5694;
            const int sectionId = 104;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextTutorialId.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_null_nextSection_if_last_section_in_course()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 82;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_assessment_nextTutorial_should_skip_tutorials_not_in_customisation()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 210934;
                const int customisationId = 18366;
                const int sectionId = 973;

                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 4255);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 4256);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 4257);

                // The next tutorial ID in this section is 4258, but the next tutorial selected in CustomisationTutorials is 4263
                const int nextTutorialId = 4263;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextTutorialId.Should().Be(nextTutorialId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_should_skip_empty_sections()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 18366;
            const int sectionId = 974;

            // The next section ID in this Application is 975, but the next section with a tutorial selected in
            // CustomisationTutorials is 978
            const int nextSectionId = 978;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_diagnostic_assessment_nextTutorial_should_skip_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 11;
                const int customisationId = 15937;
                const int sectionId = 392;

                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 1534);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 1535);

                const int nextTutorialId = 1583; // Skipping over archived 1536, 1537, 1581

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextTutorialId.Should().Be(nextTutorialId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_can_return_smaller_sectionId()
        {
            // Given
            const int candidateId = 210962;
            const int customisationId = 17668;
            const int sectionId = 747;

            const int nextSectionId = 746;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_skips_archived_sections()
        {
            // Given
            const int candidateId = 118178;
            const int customisationId = 22416;
            const int sectionId = 1958;

            const int nextSectionId = 1960; // Skips archived section 1959

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_skips_sections_full_of_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 210962;
                const int customisationId = 15062;
                const int sectionId = 246;

                // The tutorials of what would be the next section, 247;
                tutorialContentTestHelper.ArchiveTutorial(1136);

                const int expectedNextSectionId = 248;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_returns_section_with_only_diagnostic_assessment()
        {
            // Given
            const int candidateId = 74411;
            const int customisationId = 5852;
            const int sectionId = 150;

            const int expectedNextSectionId = 151; // All tutorials are CustomisationTutorials.Status = 0, though some DiagStatus = 1

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_returns_section_with_only_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;
            const int expectedNextSectionId = 105; // All tutorials are CustomisationTutorials.Status and DiagStatus = 0
                                                   // Customisations.IsAssessed = 1 and Sections.PLAssessPath is not null
                                                   // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_skips_assessed_section_with_no_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 10820;
                const int candidateId = 1;
                const int sectionId = 104;

                const int originalNextSectionId = 105; // All tutorials are CustomisationTutorials.Status and DiagStatus = 0
                                                       // Customisations.IsAssessed = 1
                sectionContentTestHelper.UpdatePostLearningAssessmentPath(originalNextSectionId, null);
                const int expectedNextSectionId = 106;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextTutorial_returns_smaller_tutorialId_for_shared_orderByNumber()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 1;
                const int customisationId = 8194;
                const int sectionId = 216;

                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 923);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 924);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 925);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 926);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 927);

                // All in section 216
                // Tutorial: 927  OrderByNumber 34
                // Tutorial: 928  OrderByNumber 35
                // Tutorial: 929  OrderByNumber 35
                const int nextTutorialId = 928;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextTutorialId.Should().Be(nextTutorialId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextTutorial_returns_next_tutorialId_with_shared_orderByNumber()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 1;
                const int customisationId = 8194;
                const int sectionId = 216;

                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 923);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 924);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 925);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 926);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 927);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 928);

                // All in section 216
                // Tutorial: 927  OrderByNumber 34
                // Tutorial: 928  OrderByNumber 35
                // Tutorial: 929  OrderByNumber 35
                // Tutorial: 930  OrderByNumber 36
                const int nextTutorialId = 929;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextTutorialId.Should().Be(nextTutorialId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextTutorial_returns_tutorialId_after_shared_orderByNumber()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 1;
                const int customisationId = 8194;
                const int sectionId = 216;

                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 923);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 924);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 925);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 926);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 927);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 928);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 929);

                // All in section 216
                // Tutorial: 927  OrderByNumber 34
                // Tutorial: 928  OrderByNumber 35
                // Tutorial: 929  OrderByNumber 35
                // Tutorial: 930  OrderByNumber 36
                const int nextTutorialId = 930;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextTutorialId.Should().Be(nextTutorialId);
            }
        }
    }
}
