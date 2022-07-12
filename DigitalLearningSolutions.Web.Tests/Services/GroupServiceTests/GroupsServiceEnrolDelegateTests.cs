namespace DigitalLearningSolutions.Web.Tests.Services.GroupServiceTests
{
    using System;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public partial class GroupsServiceTests
    {
        private readonly string genericEmailBodyHtml = @"
                <p>Dear newFirst newLast</p>
                <p>This is an automated message to notify you that you have been enrolled on the course
                <b>application - customisation</b>
                by the system because a previous course completion has expired.</p>
                <p>To login to the course directly <a href=""baseUrl/LearningMenu/13"">click here</a>.</p>
                <p>To login to the Learning Portal to access and complete your course
                <a href=""baseUrl/LearningPortal/Current"">click here</a>.</p>";

        private readonly string genericEmailBodyText = @"
                Dear newFirst newLast
                This is an automated message to notify you that you have been enrolled on the course
                application - customisation
                by the system because a previous course completion has expired.
                To login to the course directly click here:baseUrl/LearningMenu/13.
                To login to the Learning Portal to access and complete your course click here:
                baseUrl/LearningPortal/Current.";

        [Test]
        public void EnrolDelegateOnGroupCourses_adds_new_progress_record_when_no_existing_progress_found()
        {
            // Given
            SetupEnrolProcessFakes(GenericNewProgressId, GenericRelatedTutorialId);
            SetUpAddDelegateEnrolProcessFakes(reusableGroupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () => progressDataService.CreateNewDelegateProgress(
                        reusableDelegateDetails.Id,
                        reusableGroupCourse.CustomisationId,
                        reusableGroupCourse.CurrentVersion,
                        testDate,
                        3,
                        null,
                        A<DateTime?>._,
                        A<int>._
                    )
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void EnrolDelegateOnGroupCourses_adds_new_progress_record_when_existing_progress_record_is_removed()
        {
            // Given
            var existingProgressRecord = ProgressTestHelper.GetDefaultProgress(removedDate: testDate);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                existingProgressRecord
            );
            SetUpAddDelegateEnrolProcessFakes(reusableGroupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () => progressDataService.CreateNewDelegateProgress(
                        reusableDelegateDetails.Id,
                        reusableGroupCourse.CustomisationId,
                        reusableGroupCourse.CurrentVersion,
                        testDate,
                        3,
                        null,
                        A<DateTime?>._,
                        A<int>._
                    )
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void EnrolDelegateOnGroupCourses_adds_new_progress_record_when_existing_progress_record_is_completed()
        {
            // Given
            var existingProgressRecord = ProgressTestHelper.GetDefaultProgress(completed: testDate);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                existingProgressRecord
            );
            SetUpAddDelegateEnrolProcessFakes(reusableGroupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () => progressDataService.CreateNewDelegateProgress(
                        reusableDelegateDetails.Id,
                        reusableGroupCourse.CustomisationId,
                        reusableGroupCourse.CurrentVersion,
                        testDate,
                        3,
                        null,
                        A<DateTime?>._,
                        A<int>._
                    )
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void
            EnrolDelegateOnGroupCourses_add_new_progress_record_uses_zero_for_supervisor_id_if_course_supervisor_is_null()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: null);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddDelegateEnrolProcessFakes(groupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () => progressDataService.CreateNewDelegateProgress(
                        reusableDelegateDetails.Id,
                        reusableGroupCourse.CustomisationId,
                        reusableGroupCourse.CurrentVersion,
                        testDate,
                        3,
                        null,
                        A<DateTime?>._,
                        0
                    )
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void
            EnrolDelegateOnGroupCourses_add_new_progress_record_uses_course_supervisor_id_if_course_supervisor_is_not_null()
        {
            // Given
            const int supervisorId = 14;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: supervisorId);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddDelegateEnrolProcessFakes(groupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () => progressDataService.CreateNewDelegateProgress(
                        reusableDelegateDetails.Id,
                        reusableGroupCourse.CustomisationId,
                        reusableGroupCourse.CurrentVersion,
                        testDate,
                        3,
                        null,
                        testDate.AddMonths(12),
                        supervisorId
                    )
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void
            EnrolDelegateOnGroupCourses_add_new_progress_record_sets_CompleteByDate_as_null_if_course_CompleteWithinMonths_is_zero()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(completeWithinMonths: 0);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddDelegateEnrolProcessFakes(groupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () => progressDataService.CreateNewDelegateProgress(
                        reusableDelegateDetails.Id,
                        reusableGroupCourse.CustomisationId,
                        reusableGroupCourse.CurrentVersion,
                        testDate,
                        3,
                        null,
                        null,
                        A<int>._
                    )
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void
            EnrolDelegateOnGroupCourses_add_new_progress_record_sets_CompleteByDate_as_correct_future_date_if_course_CompleteWithinMonths_is_not_zero()
        {
            // Given
            const int completeWithinMonths = 3;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(completeWithinMonths: completeWithinMonths);
            var expectedFutureDate = testDate.AddMonths(3);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddDelegateEnrolProcessFakes(groupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
            );

            // Then
            using (new AssertionScope())
            {
                DelegateProgressRecordMustNotHaveBeenUpdated();
                A.CallTo(
                    () => progressDataService.CreateNewDelegateProgress(
                        reusableDelegateDetails.Id,
                        reusableGroupCourse.CustomisationId,
                        reusableGroupCourse.CurrentVersion,
                        testDate,
                        3,
                        null,
                        expectedFutureDate,
                        A<int>._
                    )
                ).MustHaveHappened();
                A.CallTo(() => progressDataService.CreateNewAspProgress(GenericRelatedTutorialId, GenericNewProgressId))
                    .MustHaveHappened();
            }
        }

        [Test]
        public void EnrolDelegateOnGroupCourses_updates_existing_progress_record()
        {
            // Given
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                reusableProgressRecord
            );
            SetUpAddDelegateEnrolProcessFakes(reusableGroupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
            );

            // Then
            using (new AssertionScope())
            {
                NewDelegateProgressRecordMustNotHaveBeenAdded();
                A.CallTo(
                    () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                        reusableProgressRecord.ProgressId,
                        A<int>._,
                        A<DateTime?>._
                    )
                ).MustHaveHappened();
            }
        }

        [Test]
        public void
            EnrolDelegateOnGroupCourses_update_uses_existing_record_supervisor_id_if_course_supervisor_id_is_null()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: null);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                reusableProgressRecord
            );
            SetUpAddDelegateEnrolProcessFakes(groupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
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
        public void EnrolDelegateOnGroupCourses_update_uses_course_supervisor_id_if_course_supervisor_id_is_not_null()
        {
            // Given
            const int supervisorId = 12;
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(supervisorAdminId: supervisorId);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                reusableProgressRecord
            );
            SetUpAddDelegateEnrolProcessFakes(groupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
            );

            // Then
            using (new AssertionScope())
            {
                NewDelegateProgressRecordMustNotHaveBeenAdded();
                A.CallTo(
                    () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                        reusableProgressRecord.ProgressId,
                        supervisorId,
                        A<DateTime?>._
                    )
                ).MustHaveHappened();
            }
        }

        [Test]
        public void
            EnrolDelegateOnGroupCourses_update_sets_CompleteByDate_as_null_if_course_CompleteWithinMonths_is_zero()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(completeWithinMonths: 0);
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId,
                reusableProgressRecord
            );
            SetUpAddDelegateEnrolProcessFakes(groupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
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
            EnrolDelegateOnGroupCourses_update_sets_CompleteByDate_as_correct_future_date_if_course_CompleteWithinMonths_is_not_zero()
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
            SetUpAddDelegateEnrolProcessFakes(groupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
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
        public void EnrolDelegateOnGroupCourses_sends_email_on_successful_enrolment_with_correct_process_name()
        {
            // Given
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddDelegateEnrolProcessFakes(reusableGroupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                reusableDelegateDetails,
                reusableMyAccountDetailsData,
                8
            );

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => emailService.ScheduleEmail(A<Email>._, "AddDelegateToGroup_Refactor", null)
                    )
                    .MustHaveHappened();
            }
        }

        [Test]
        public void EnrolDelegateOnGroupCourses_sends_correct_email_with_no_CompleteByDate()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(
                customisationId: 13,
                applicationName: "application",
                customisationName: "customisation",
                completeWithinMonths: 0
            );
            var oldDelegateDetails = UserTestHelper.GetDefaultDelegateUser(
                firstName: "oldFirst",
                lastName: "oldLast",
                emailAddress: "oldEmail"
            );
            var newAccountDetails = UserTestHelper.GetDefaultAccountDetailsData(
                firstName: "newFirst",
                surname: "newLast",
                email: "newEmail"
            );
            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddDelegateEnrolProcessFakes(groupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                oldDelegateDetails,
                newAccountDetails,
                8
            );

            // Then
            A.CallTo(
                () => emailService.ScheduleEmail(
                    A<Email>.That.Matches(
                        e =>
                            e.Bcc.IsNullOrEmpty()
                            && e.Cc.IsNullOrEmpty()
                            && e.To[0] == newAccountDetails.Email
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
        public void EnrolDelegateOnGroupCourses_sends_correct_email_with_additional_CompleteByDate()
        {
            // Given
            var groupCourse = GroupTestHelper.GetDefaultGroupCourse(
                customisationId: 13,
                applicationName: "application",
                customisationName: "customisation",
                completeWithinMonths: 12
            );
            var oldDelegateDetails = UserTestHelper.GetDefaultDelegateUser(
                firstName: "oldFirst",
                lastName: "oldLast",
                emailAddress: "oldEmail"
            );
            var newAccountDetails = UserTestHelper.GetDefaultAccountDetailsData(
                firstName: "newFirst",
                surname: "newLast",
                email: "newEmail"
            );
            var expectedTextBody = genericEmailBodyText + "The date the course should be completed by is 11/12/2022";
            var expectedHtmlBody =
                genericEmailBodyHtml + "<p>The date the course should be completed by is 11/12/2022</p>";

            SetupEnrolProcessFakes(
                GenericNewProgressId,
                GenericRelatedTutorialId
            );
            SetUpAddDelegateEnrolProcessFakes(groupCourse);

            // When
            groupsService.EnrolDelegateOnGroupCourses(
                oldDelegateDetails,
                newAccountDetails,
                8
            );

            // Then
            A.CallTo(
                () => emailService.ScheduleEmail(
                    A<Email>.That.Matches(
                        e =>
                            e.Bcc.IsNullOrEmpty()
                            && e.Cc.IsNullOrEmpty()
                            && e.To[0] == newAccountDetails.Email
                            && e.Subject == "New Learning Portal Course Enrolment"
                            && e.Body.TextBody == expectedTextBody
                            && e.Body.HtmlBody == expectedHtmlBody
                    ),
                    A<string>._,
                    null
                )
            ).MustHaveHappened();
        }
    }
}
