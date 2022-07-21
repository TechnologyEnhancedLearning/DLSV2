namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using System.Transactions;
    using FluentAssertions;
    using NUnit.Framework;

    internal partial class PostLearningAssessmentTests
    {
        [Test]
        public void Get_post_learning_assessment_should_have_other_items_in_section_if_a_tutorial_has_status_1()
        {
            // Given
            const int candidateId = 86972;
            const int customisationId = 26655;
            const int sectionId = 2739;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_post_learning_assessment_has_other_items_in_section_if_just_has_section_consolidation()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 273836;
                const int customisationId = 27088;
                const int sectionId = 2002;

                sectionContentTestHelper.UpdateConsolidationPath(sectionId, "some/consolidation/path.pdf");

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherItemsInSectionExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_post_learning_assessment_has_no_other_items_in_section_if_just_has_diagnostic_but_no_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 187246;
                const int customisationId = 15073;
                const int sectionId = 366;

                // Add post learning path to section
                sectionContentTestHelper.UpdatePostLearningAssessmentPath(sectionId, "some/post-learning/path");

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherItemsInSectionExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_post_learning_assessment_has_no_other_items_in_section()
        {
            // Given
            const int candidateId = 274374;
            const int customisationId = 22709;
            const int sectionId = 2002;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherItemsInSectionExist.Should().BeFalse();
        }

        [Test]
        public void Get_post_learning_assessment_has_no_other_items_in_section_if_tutorials_are_archived()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 210962;
                const int customisationId = 27858;
                const int sectionId = 3071;

                // Set a tutorial to have status 0, already wih diagnostic status of 0.
                // At least one tutorial (even with both statuses as 0) is required for the
                // post-learning assessment to be valid
                tutorialContentTestHelper.UpdateTutorialStatus(12817, customisationId, 0);

                // Archive the rest of the section's tutorials (with status 1)
                tutorialContentTestHelper.ArchiveTutorial(12818);
                tutorialContentTestHelper.ArchiveTutorial(12819);

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherItemsInSectionExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_post_learning_assessment_should_have_other_sections_in_course_if_last_section_in_course()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 82;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_post_learning_assessment_should_have_other_sections_in_course_if_in_middle_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 383;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_post_learning_assessment_should_have_other_sections_in_course_if_at_end_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 386;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [TestCase(3000)]
        [TestCase(3001)]
        [TestCase(3002)]
        public void Get_post_learning_assessment_should_have_other_sections_in_course_when_shared_section_number(int sectionId)
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 27440;
                const int candidateId = 207900;
                sectionContentTestHelper.UpdateSectionNumber(3001, 16);

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_post_learning_assessment_should_have_other_sections_in_course_when_only_other_section_shares_section_number()
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
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_post_learning_assessment_should_have_other_sections_in_course_when_other_section_only_has_diagnostic_assessment()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5694;
                const int candidateId = 1;
                const int sectionId = 103;

                // Make this course assessed
                courseContentTestHelper.UpdateIsAssessed(customisationId, true);

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_post_learning_assessment_should_have_other_sections_in_course_when_other_sections_only_have_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_post_learning_assessment_should_not_have_other_sections_in_course_when_only_section_in_application()
        {
            // Given
            const int customisationId = 7967;
            const int candidateId = 11;
            const int sectionId = 210;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeFalse();
        }

        [Test]
        public void Get_post_learning_assessment_should_not_have_other_sections_in_course_when_only_section_not_archived_in_course()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 21727;
                const int candidateId = 210934;
                const int sectionId = 1806;

                // Make this course assessed
                courseContentTestHelper.UpdateIsAssessed(customisationId, true);

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_post_learning_assessment_should_not_have_other_sections_in_course_when_other_section_is_full_of_archived_tutorials()
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
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_post_learning_assessment_should_not_have_other_sections_in_course_when_other_section_has_no_diagnostic_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5694;
                const int candidateId = 1;
                const int sectionId = 103;

                // Make this course assessed
                courseContentTestHelper.UpdateIsAssessed(customisationId, true);

                // Remove assessment paths from other sections
                int[] otherSections = { 104, 105, 106, 107, 108, 109, 110, 111 };
                otherSections.ToList().ForEach(section =>
                {
                    sectionContentTestHelper.UpdateDiagnosticAssessmentPath(section, null);
                    sectionContentTestHelper.UpdatePostLearningAssessmentPath(section, null);
                });

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_post_learning_assessment_should_not_have_other_sections_in_course_when_other_section_has_no_post_learning_assessment_path()
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
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeFalse();
            }
        }
    }
}
