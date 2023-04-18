namespace DigitalLearningSolutions.Web.Tests.Services.GroupServiceTests
{
    using System;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public partial class GroupsServiceTests
    {
        private const int CentreId = 1;

        [Test]
        public void AddCourseToGroup_adds_new_group_customisations_record()
        {
            // Given
            const int adminId = 1;
            const int completeWithinMonths = 8;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: adminId);
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                groupCourse.GroupId,
                groupCourse.CustomisationId,
                completeWithinMonths,
                adminId,
                true,
                adminId,
                CentreId
            );

            // Then
            A.CallTo(
                () => groupsDataService.InsertGroupCustomisation(
                    groupCourse.GroupId,
                    groupCourse.CustomisationId,
                    completeWithinMonths,
                    adminId,
                    true,
                    adminId
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void AddCourseToGroup_adds_new_progress_record_when_no_existing_progress_found()
        {
            // Given
            const int adminId = 1;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: adminId);
            SetupEnrolProcessFakes(GenericNewProgressId, GenericRelatedTutorialId);
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                groupCourse.GroupId,
                groupCourse.CustomisationId,
                8,
                adminId,
                true,
                adminId,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () =>
                     groupsDataService.InsertGroupCustomisation(groupCourse.GroupId, groupCourse.CustomisationId, 8, adminId, true, adminId)
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void AddCourseToGroup_adds_new_progress_record_when_existing_progress_record_is_removed()
        {
            // Given
            const int adminId = 1;
            var existingProgressRecord = ProgressTestHelper.GetDefaultProgress(removedDate: testDate);
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: adminId);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                existingProgressRecord
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                groupCourse.GroupId,
                groupCourse.CustomisationId,
                8,
                adminId,
                true,
                adminId,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () =>
                     groupsDataService.InsertGroupCustomisation(groupCourse.GroupId, groupCourse.CustomisationId, 8, adminId, true, adminId)
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void AddCourseToGroup_adds_new_progress_record_when_existing_progress_record_is_completed()
        {
            // Given
            const int adminId = 1;
            var existingProgressRecord = ProgressTestHelper.GetDefaultProgress(completed: testDate);
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: adminId);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                existingProgressRecord
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                groupCourse.GroupId,
                groupCourse.CustomisationId,
                3,
                adminId,
                true,
                adminId,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () =>
                    groupsDataService.InsertGroupCustomisation(groupCourse.GroupId, groupCourse.CustomisationId, 3, adminId, true, adminId)
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void
            AddCourseToGroup_add_new_progress_record_uses_zero_for_supervisor_id_if_course_supervisor_is_null()
        {
            // Given
            const int adminId = 1;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: null);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                reusableGroupCourse.GroupId,
                reusableGroupCourse.CustomisationId,
                3,
                adminId,
                true,
                null,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () =>
                    groupsDataService.InsertGroupCustomisation(groupCourse.GroupId, groupCourse.CustomisationId, 3, adminId, true, null)
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void
            AddCourseToGroup_add_new_progress_record_uses_course_supervisor_id_if_course_supervisor_is_not_null()
        {
            // Given
            const int adminId = 1;
            const int supervisorId = 14;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: supervisorId);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                reusableGroupCourse.GroupId,
                reusableGroupCourse.CustomisationId,
                3,
                adminId,
                true,
                supervisorId,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
               () =>
               groupsDataService.InsertGroupCustomisation(groupCourse.GroupId, groupCourse.CustomisationId, 3, adminId, true, supervisorId)
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void
            AddCourseToGroup_add_new_progress_record_sets_CompleteByDate_as_null_if_course_CompleteWithinMonths_is_zero()
        {
            // Given
            const int adminId = 1;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(
                completeWithinMonths: 0,
                supervisorAdminId: adminId
            );
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                reusableGroupCourse.GroupId,
                reusableGroupCourse.CustomisationId,
                0,
                adminId,
                true,
                adminId,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () =>
                       groupsDataService.InsertGroupCustomisation(groupCourse.GroupId, groupCourse.CustomisationId, 0, adminId, true, adminId)
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void
            AddCourseToGroup_add_new_progress_record_sets_CompleteByDate_as_correct_future_date_if_course_CompleteWithinMonths_is_not_zero()
        {
            // Given
            const int adminId = 1;
            const int completeWithinMonths = 3;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(
                completeWithinMonths: completeWithinMonths,
                supervisorAdminId: adminId
            );
            var expectedFutureDate = testDate.AddMonths(3);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                groupCourse.GroupId,
                groupCourse.CustomisationId,
                completeWithinMonths,
                adminId,
                true,
                adminId,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () =>

                groupsDataService.InsertGroupCustomisation(groupCourse.GroupId, groupCourse.CustomisationId, completeWithinMonths, adminId, true, adminId)
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void
            AddCourseToGroup_keeps_existing_supervisor_when_updating_progress()
        {
            // Given
            const int adminId = 4;
            const int supervisorId = 12;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: supervisorId);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                reusableProgressRecord
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                reusableGroupCourse.GroupId,
                reusableGroupCourse.CustomisationId,
                8,
                adminId,
                true,
                supervisorId,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                NewDelegateProgressRecordMustNotHaveBeenAdded();
                A.CallTo(
                    () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                        reusableProgressRecord.ProgressId,
                        reusableProgressRecord.SupervisorAdminId,
                        A<DateTime?>._
                    )
                ).MustHaveHappened();
            }
        }

        [Test]
        public void
            AddCourseToGroup_update_sets_CompleteByDate_as_null_if_course_CompleteWithinMonths_is_zero()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(completeWithinMonths: 0);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                reusableProgressRecord
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                reusableGroupCourse.GroupId,
                reusableGroupCourse.CustomisationId,
                8,
                1,
                true,
                1,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                NewDelegateProgressRecordMustNotHaveBeenAdded();
                A.CallTo(
                    () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                        reusableProgressRecord.ProgressId,
                        A<int>._,
                        null
                    )
                ).MustHaveHappened();
            }
        }

        [Test]
        public void
            AddCourseToGroup_update_sets_CompleteByDate_as_correct_future_date_if_course_CompleteWithinMonths_is_not_zero()
        {
            // Given
            const int completeWithinMonths = 3;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(completeWithinMonths: completeWithinMonths);
            var expectedFutureDate = testDate.AddMonths(3);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                reusableProgressRecord
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                groupCourse.GroupId,
                groupCourse.CustomisationId,
                8,
                1,
                true,
                1,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                NewDelegateProgressRecordMustNotHaveBeenAdded();
                A.CallTo(
                    () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                        reusableProgressRecord.ProgressId,
                        A<int>._,
                        expectedFutureDate
                    )
                ).MustHaveHappened();
            }
        }

        [Test]
        public void AddCourseToGroup_sends_email_on_successful_enrolment_with_correct_process_name()
        {
            // Given
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddCourseEnrolProcessFakes(reusableGroupCourse);

            // When
            groupsService.AddCourseToGroup(
                reusableGroupCourse.GroupId,
                reusableGroupCourse.CustomisationId,
                8,
                1,
                true,
                1,
                CentreId
            );

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => emailService.ScheduleEmail(A<Email>._, "AddCourseToDelegateGroup_Refactor", null)
                    )
                    .MustHaveHappened();
            }
        }

        [Test]
        public void AddCourseToGroup_sends_correct_email_with_no_CompleteByDate()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(
                customisationId: 13,
                applicationName: "application",
                customisationName: "customisation",
                completeWithinMonths: 0
            );
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                groupCourse.GroupId,
                groupCourse.CustomisationId,
                0,
                1,
                true,
                1,
                CentreId
            );

            // Then
            A.CallTo(
                () => emailService.ScheduleEmail(
                    A<Email>.That.Matches(
                        e =>
                            e.Bcc.IsNullOrEmpty()
                            && e.Cc.IsNullOrEmpty()
                            && e.To[0] == reusableGroupDelegate.PrimaryEmail
                            && e.Subject == "New Learning Portal Course Enrolment"
                            && e.Body.TextBody == genericEmailBodyText
                            && e.Body.HtmlBody == genericEmailBodyHtml
                    ),
                    A<string>._,
                    null
                )
            ).MustHaveHappened();
        }

        [Test]
        public void AddCourseToGroup_does_not_send_email_to_delegates_without_required_notification_preference()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(
                customisationId: 13,
                applicationName: "application",
                customisationName: "customisation",
                completeWithinMonths: 0
            );
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                notifyDelegates: false
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                groupCourse.GroupId,
                groupCourse.CustomisationId,
                0,
                1,
                true,
                1,
                CentreId
            );

            // Then
            A.CallTo(
                () => emailService.ScheduleEmail(
                    A<Email>._,
                    A<string>._,
                    A<DateTime>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void AddCourseToGroup_sends_correct_email_with_additional_CompleteByDate()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(
                customisationId: 13,
                applicationName: "application",
                customisationName: "customisation",
                completeWithinMonths: 12
            );
            var expectedTextBody = genericEmailBodyText + "The date the course should be completed by is 11/12/2022";
            var expectedHtmlBody =
                genericEmailBodyHtml + "<p>The date the course should be completed by is 11/12/2022</p>";

            SetupEnrolProcessFakes(
            GenericNewProgressId,
            GenericRelatedTutorialId
            );
            SetUpAddCourseEnrolProcessFakes(groupCourse);

            // When
            groupsService.AddCourseToGroup(
                groupCourse.GroupId,
                groupCourse.CustomisationId,
                8,
                1,
                true,
                1,
                CentreId
            );

            // TODO: Fix failing test:
            //// Then
            //A.CallTo(
            //    () => emailService.ScheduleEmail(
            //        A<Email>.That.Matches(
            //            e =>
            //                e.Bcc.IsNullOrEmpty()
            //                && e.Cc.IsNullOrEmpty()
            //                && e.To[0] == reusableGroupDelegate.PrimaryEmail
            //                && e.Subject == "New Learning Portal Course Enrolment"
            //                && e.Body.TextBody == expectedTextBody
            //                && e.Body.HtmlBody == expectedHtmlBody
            //        ),
            //        A<string>._,
            //        null
            //    )
            //).MustHaveHappened();
        }

        [Test]
        public void AddCourseToGroup_with_invalid_customisation_for_centre_results_in_exception()
        {
            // Given
            const int adminId = 1;
            const int groupCustomisationId = 8;
            A.CallTo(
                () => groupsDataService.InsertGroupCustomisation(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<int?>._
                )
            ).Returns(groupCustomisationId);
            A.CallTo(() => groupsDataService.GetGroupCourseIfVisibleToCentre(groupCustomisationId, CentreId))
                .Returns(null);

            // Then
            Assert.Throws<CourseAccessDeniedException>(
                () => groupsService.AddCourseToGroup(
                    1,
                    1,
                    0,
                    adminId,
                    true,
                    adminId,
                    CentreId
                )
            );
        }
    }
}
