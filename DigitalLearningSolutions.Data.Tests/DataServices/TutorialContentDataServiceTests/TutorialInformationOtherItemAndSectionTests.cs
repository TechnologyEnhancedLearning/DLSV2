namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using System.Linq;
    using System.Transactions;
    using FluentAssertions;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        [Test]
        public void Get_tutorial_information_should_have_other_items_in_section_if_another_tutorial_has_status_1()
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
            tutorial!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_has_other_items_in_section_if_single_tutorial_just_has_diagnostic()
        {
            // Given
            const int candidateId = 210962;
            const int customisationId = 14961;
            const int sectionId = 350;
            const int tutorialId = 1360;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_has_other_items_in_section_if_single_tutorial_just_has_post_learning()
        {
            // Given
            const int candidateId = 245614;
            const int customisationId = 24001;
            const int sectionId = 2094;
            const int tutorialId = 9705;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_has_other_items_in_section_if_single_tutorial_just_has_section_consolidation()
        {
            // Given
            const int candidateId = 267014;
            const int customisationId = 21669;
            const int sectionId = 1802;
            const int tutorialId = 8593;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_has_no_other_items_in_section()
        {
            // Given
            const int candidateId = 272596;
            const int customisationId = 23048;
            const int sectionId = 2027;
            const int tutorialId = 9526;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherItemsInSectionExist.Should().BeFalse();
        }

        [Test]
        public void Get_tutorial_information_has_no_other_items_in_section_if_other_tutorials_are_archived()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 14895;
                const int candidateId = 22045;
                const int sectionId = 333;
                const int tutorialId = 1339;

                // The tutorials in this section, which does not have a post learning assessment or consolidation path
                tutorialContentTestHelper.ArchiveTutorial(1340);
                tutorialContentTestHelper.ArchiveTutorial(1341);

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherItemsInSectionExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_tutorial_information_has_other_items_in_section_if_another_tutorial_has_diagnostic()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5468;
                const int candidateId = 94705;
                const int sectionId = 102;
                const int tutorialId = 316;

                // Remove diagnostic from this tutorial
                tutorialContentTestHelper.UpdateDiagnosticStatus(tutorialId, customisationId, 0);

                // Make other tutorials in this section inaccessible, but they still have a diagnostic status 1
                tutorialContentTestHelper.UpdateTutorialStatus(317, customisationId, 0);
                tutorialContentTestHelper.UpdateTutorialStatus(318, customisationId, 0);

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherItemsInSectionExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_tutorial_information_should_have_other_sections_in_course_if_last_section_in_course()
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
            tutorial!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_should_have_otherSectionsExist_if_in_middle_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 383;
            const int tutorialId = 1465;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_should_have_otherSectionsExist_if_at_end_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 386;
            const int tutorialId = 1485;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherSectionsExist.Should().BeTrue();
        }

        [TestCase(2087, 10166)]
        [TestCase(2195, 10168)]
        [TestCase(2199, 10169)]
        public void Get_tutorial_information_should_have_otherSectionsExist_when_shared_section_number(
            int sectionId,
            int tutorialId
        )
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 24057;
                const int candidateId = 1;
                sectionContentTestHelper.UpdateSectionNumber(2195, 10);

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_tutorial_information_should_have_otherSectionsExist_when_only_other_section_shares_section_number()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 17456;
                const int candidateId = 210934;
                const int sectionId = 664;
                const int tutorialId = 2718;

                sectionContentTestHelper.UpdateSectionNumber(668, 1); // Section 664 also has SectionNumber 1, and is
                                                                      // the only other section on the course

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_tutorial_information_should_not_have_otherSectionsExist_when_only_section_in_application()
        {
            // Given
            const int customisationId = 7967;
            const int candidateId = 11;
            const int sectionId = 210;
            const int tutorialId = 885;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherSectionsExist.Should().BeFalse();
        }

        [Test]
        public void Get_tutorial_information_should_not_have_otherSectionsExist_when_only_section_not_archived_in_course()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 21727;
                const int candidateId = 210934;
                const int sectionId = 1806;
                const int tutorialId = 8621;

                // Make this tutorial viewable
                tutorialContentTestHelper.UpdateTutorialStatus(tutorialId, customisationId, 1);

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_tutorial_information_should_not_have_otherSectionsExist_when_other_section_is_full_of_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 17456;
                const int candidateId = 210934;
                const int sectionId = 664;
                const int tutorialId = 2717;

                // The tutorials of what would be the next section, 668
                // This is the only other section on this course
                tutorialContentTestHelper.ArchiveTutorial(2713);
                tutorialContentTestHelper.ArchiveTutorial(2714);
                tutorialContentTestHelper.ArchiveTutorial(2715);
                tutorialContentTestHelper.ArchiveTutorial(2716);

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_tutorial_information_should_have_otherSectionsExist_when_other_section_only_has_diagnostic_assessment()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5694;
                const int candidateId = 1;
                const int sectionId = 103;
                const int tutorialId = 322;

                // Make this tutorial viewable
                tutorialContentTestHelper.UpdateTutorialStatus(tutorialId, customisationId, 1);

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_tutorial_information_should_not_have_otherSectionsExist_when_other_section_has_no_diagnostic_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5694;
                const int candidateId = 1;
                const int sectionId = 103;
                const int tutorialId = 322;

                // Make this tutorial viewable
                tutorialContentTestHelper.UpdateTutorialStatus(tutorialId, customisationId, 1);

                // Remove diagnostic assessment paths from other sections
                int[] otherSections = { 104, 105, 106, 107, 108, 109, 110, 111 };
                otherSections.ToList().ForEach(section =>
                    sectionContentTestHelper.UpdateDiagnosticAssessmentPath(section, null)
                );

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_tutorial_information_should_have_otherSectionsExist_when_other_sections_only_have_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;
            const int tutorialId = 326;

            // When
            var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_should_not_have_otherSectionsExist_when_other_section_has_no_post_learning_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 10820;
                const int candidateId = 1;
                const int sectionId = 104;
                const int tutorialId = 326;

                // Remove post learning assessment paths from other sections
                int[] otherSections = { 103, 105, 106, 107, 108, 109, 110, 111 };
                otherSections.ToList().ForEach(section =>
                    sectionContentTestHelper.UpdatePostLearningAssessmentPath(section, null)
                );

                // When
                var tutorial = tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeFalse();
            }
        }
    }
}
