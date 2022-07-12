namespace DigitalLearningSolutions.Data.Tests.DataServices.DiagnosticAssessmentDataServiceTests
{
    using System.Linq;
    using System.Transactions;
    using FluentAssertions;
    using NUnit.Framework;

    internal partial class DiagnosticAssessmentDataServiceTests
    {
        [Test]
        public void Get_diagnostic_learning_assessment_should_have_other_items_in_section_if_a_tutorial_has_status_1()
        {
            // Given
            const int candidateId = 286695;
            const int customisationId = 26254;
            const int sectionId = 2479;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_diagnostic_assessment_has_other_items_in_section_if_just_has_section_consolidation()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 145944;
                const int customisationId = 26468;
                const int sectionId = 2549;
                sectionContentTestHelper.UpdateConsolidationPath(sectionId, "some/consolidation/path.pdf");

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherItemsInSectionExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_diagnostic_assessment_has_other_items_in_section_if_just_has_post_learning_assessment()
        {
            // Given
            const int candidateId = 270363;
            const int customisationId = 23666;
            const int sectionId = 201;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_diagnostic_assessment_has_no_other_items_in_section_if_is_assessed_but_has_no_post_learning_path()
        {
            // Given
            const int candidateId = 22045;
            const int customisationId = 16930;
            const int sectionId = 526;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherItemsInSectionExist.Should().BeFalse();
        }

        [Test]
        public void Get_diagnostic_assessment_has_no_other_items_in_section_if_has_no_post_learning_path_but_is_not_assessed()
        {
            // Given
            const int candidateId = 145881;
            const int customisationId = 13335;
            const int sectionId = 174;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherItemsInSectionExist.Should().BeFalse();
        }

        [Test]
        public void Get_diagnostic_assessment_has_no_other_items_in_section()
        {
            // Given
            const int candidateId = 145944;
            const int customisationId = 26468;
            const int sectionId = 2548;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherItemsInSectionExist.Should().BeFalse();
        }

        [Test]
        public void Get_diagnostic_assessment_has_no_other_items_in_section_if_tutorials_are_archived()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 145944;
                const int customisationId = 26468;
                const int sectionId = 2548;
                const int tutorialId = 11450;

                // Set a tutorial to have status 1 in section that would only have a diagnostic otherwise
                tutorialContentTestHelper.UpdateTutorialStatus(tutorialId, customisationId, 1);

                // Then archive it to check there still are no other items in the section
                tutorialContentTestHelper.ArchiveTutorial(tutorialId);

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherItemsInSectionExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_diagnostic_assessment_should_have_other_sections_in_course_if_last_section_in_course()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 82;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_diagnostic_assessment_should_have_other_sections_in_course_if_in_middle_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 383;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_diagnostic_assessment_should_have_other_sections_in_course_if_at_end_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 386;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [TestCase(2474)]
        [TestCase(2475)]
        [TestCase(2476)]
        public void Get_diagnostic_assessment_should_have_other_sections_in_course_when_shared_section_number(int sectionId)
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 26254;
                const int candidateId = 286695;
                sectionContentTestHelper.UpdateSectionNumber(2475, 7);

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_diagnostic_assessment_should_have_other_sections_in_course_when_only_other_section_shares_section_number()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 17456;
                const int candidateId = 210934;
                const int sectionId = 664;
                sectionContentTestHelper.UpdateSectionNumber(668, 1); // Section 664 also has SectionNumber 1, and is
                                                                      // the only other section on the course

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_diagnostic_assessment_should_have_other_sections_in_course_when_other_section_only_has_diagnostic_assessment()
        {
            // Given
            const int customisationId = 5694;
            const int candidateId = 1;
            const int sectionId = 103;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_diagnostic_assessment_should_have_other_sections_in_course_when_other_sections_only_have_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_have_other_sections_in_course_when_only_section_in_application()
        {
            // Given
            const int customisationId = 7967;
            const int candidateId = 11;
            const int sectionId = 210;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeFalse();
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_have_other_sections_in_course_when_only_section_not_archived_in_course()
        {
            // Given
            const int customisationId = 21727;
            const int candidateId = 210934;
            const int sectionId = 1806;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeFalse();
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_have_other_sections_in_course_when_other_section_is_full_of_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 17456;
                const int candidateId = 210934;
                const int sectionId = 664;

                // The tutorials of what would be the next section, 668
                // This is the only other section on this course
                tutorialContentTestHelper.ArchiveTutorial(2713);
                tutorialContentTestHelper.ArchiveTutorial(2714);
                tutorialContentTestHelper.ArchiveTutorial(2715);
                tutorialContentTestHelper.ArchiveTutorial(2716);

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_have_other_sections_in_course_when_other_section_has_no_diagnostic_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5694;
                const int candidateId = 1;
                const int sectionId = 103;

                // Remove assessment paths from other sections
                int[] otherSections = { 104, 105, 106, 107, 108, 109, 110, 111 };
                otherSections.ToList().ForEach(section =>
                {
                    sectionContentTestHelper.UpdateDiagnosticAssessmentPath(section, null);
                    sectionContentTestHelper.UpdatePostLearningAssessmentPath(section, null);
                });

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_have_other_sections_in_course_when_other_section_has_no_post_learning_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 10820;
                const int candidateId = 1;
                const int sectionId = 104;

                // Remove post learning assessment paths from other sections
                int[] otherSections = { 103, 105, 106, 107, 108, 109, 110, 111 };
                otherSections.ToList().ForEach(section =>
                    sectionContentTestHelper.UpdatePostLearningAssessmentPath(section, null)
                );

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeFalse();
            }
        }
    }
}
