namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class NotificationServiceTests
    {
        private const int ProgressId = 1;
        private const int DelegateId = 1;
        private const int CustomisationId = 1;
        private IConfiguration configuration = null!;
        private IEmailService emailService = null!;
        private IFeatureManager featureManager = null!;
        private INotificationDataService notificationDataService = null!;
        private INotificationService notificationService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            configuration = A.Fake<IConfiguration>();
            emailService = A.Fake<IEmailService>();
            featureManager = A.Fake<IFeatureManager>();
            notificationDataService = A.Fake<INotificationDataService>();
            userService = A.Fake<IUserService>();

            notificationService = new NotificationService(
                configuration,
                notificationDataService,
                emailService,
                featureManager,
                userService
            );
        }

        [Test]
        public void SendProgressCompletionNotification_calls_data_service_and_sends_email_to_correct_delegate_email()
        {
            // Given
            const string delegateEmail = "delegate@email.com";
            var progress = ProgressTestHelper.GetDefaultDetailedCourseProgress();

            SetUpSendProgressCompletionNotificationEmailFakes(delegateEmail: delegateEmail);

            // When
            notificationService.SendProgressCompletionNotificationEmail(progress, 2, 3);

            // Then
            A.CallTo(() => notificationDataService.GetProgressCompletionData(ProgressId, DelegateId, CustomisationId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(
                () => emailService.SendEmail(
                    A<Email>.That.Matches(
                        e => e.To.SequenceEqual(new[] { delegateEmail })
                    )
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void SendProgressCompletionNotification_does_not_send_email_if_progress_completion_data_is_null()
        {
            // Given
            var progress = ProgressTestHelper.GetDefaultDetailedCourseProgress();

            A.CallTo(() => notificationDataService.GetProgressCompletionData(ProgressId, DelegateId, CustomisationId))
                .Returns(null);

            // When
            notificationService.SendProgressCompletionNotificationEmail(progress, 2, 3);

            // Then
            A.CallTo(() => emailService.SendEmail(A<Email>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void SendProgressCompletionNotification_ccs_admin_if_email_found()
        {
            // Given
            const string adminEmail = "admin@email.com";

            var progress = ProgressTestHelper.GetDefaultDetailedCourseProgress();
            SetUpSendProgressCompletionNotificationEmailFakes(adminId: 1, adminEmail: adminEmail);

            // When
            notificationService.SendProgressCompletionNotificationEmail(progress, 2, 3);

            // Then
            A.CallTo(
                () => emailService.SendEmail(
                    A<Email>.That.Matches(
                        e => e.Cc.SequenceEqual(new[] { adminEmail }) && e.Body.TextBody.Contains(
                            "Note: This message has been copied to the administrator(s) managing this activity, for their information."
                        )
                    )
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void SendProgressCompletionNotification_does_not_add_line_of_text_about_admin_if_admin_email_is_null()
        {
            // Given
            var progress = ProgressTestHelper.GetDefaultDetailedCourseProgress();
            SetUpSendProgressCompletionNotificationEmailFakes(adminEmail: null);

            // When
            notificationService.SendProgressCompletionNotificationEmail(progress, 2, 3);

            // Then
            A.CallTo(
                () => emailService.SendEmail(
                    A<Email>.That.Matches(
                        e => e.Body.TextBody.Contains(
                            "Note: This message has been copied to the administrator(s) managing this activity, for their information."
                        )
                    )
                )
            ).MustNotHaveHappened();
        }

        [TestCase(
            2,
            "To evaluate the activity and access your certificate of completion, visit this URL:"
        )]
        [TestCase(
            1,
            "If you haven't already done so, please evaluate the activity to help us to improve provision " +
            "for future delegates by visiting this URL:"
        )]
        public void SendProgressCompletionNotification_shows_correct_text_for_completion_status(
            int completionStatus,
            string lineOfText
        )
        {
            // Given
            var progress = ProgressTestHelper.GetDefaultDetailedCourseProgress();
            SetUpSendProgressCompletionNotificationEmailFakes();

            // When
            notificationService.SendProgressCompletionNotificationEmail(progress, completionStatus, 3);

            // Then
            A.CallTo(
                () => emailService.SendEmail(
                    A<Email>.That.Matches(
                        e => e.Body.TextBody.Contains(
                            lineOfText
                        )
                    )
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void SendProgressCompletionNotification_forms_url_correctly()
        {
            // Given
            const string finaliseUrl = "/tracking/finalise?SessionID=123&ProgressID=1&UserCentreID=101";

            var progress = ProgressTestHelper.GetDefaultDetailedCourseProgress();
            SetUpSendProgressCompletionNotificationEmailFakes();

            // When
            notificationService.SendProgressCompletionNotificationEmail(progress, 2, 3);

            // Then
            A.CallTo(
                () => emailService.SendEmail(
                    A<Email>.That.Matches(
                        e => e.Body.HtmlBody.Contains(
                            finaliseUrl
                        )
                    )
                )
            ).MustHaveHappenedOnceExactly();
        }

        [TestCase(
            1,
            "This activity is related to 1 planned development log action in another activity " +
            "in your Learning Portal. This has automatically been marked as complete."
        )]
        [TestCase(
            2,
            "This activity is related to 2 planned development log actions in other activities " +
            "in your Learning Portal. These have automatically been marked as complete."
        )]
        public void SendProgressCompletionNotification_shows_correct_text_for_number_of_learning_log_items_affected(
            int numLearningLogItemsAffected,
            string lineOfText
        )
        {
            // Given
            var progress = ProgressTestHelper.GetDefaultDetailedCourseProgress();
            SetUpSendProgressCompletionNotificationEmailFakes();

            // When
            notificationService.SendProgressCompletionNotificationEmail(progress, 2, numLearningLogItemsAffected);

            // Then
            A.CallTo(
                () => emailService.SendEmail(
                    A<Email>.That.Matches(
                        e => e.Body.TextBody.Contains(
                            lineOfText
                        )
                    )
                )
            ).MustHaveHappenedOnceExactly();
        }

        [TestCase(
            "This activity is related to 1 planned development log action in another activity " +
            "in your Learning Portal. This has automatically been marked as complete."
        )]
        [TestCase(
            "This activity is related to 2 planned development log actions in other activities " +
            "in your Learning Portal. These have automatically been marked as complete."
        )]
        public void
            SendProgressCompletionNotification_does_not_add_the_extra_learning_log_item_line_of_text_when_number_affected_is_zero(
                string lineOfText
            )
        {
            // Given
            var progress = ProgressTestHelper.GetDefaultDetailedCourseProgress();
            SetUpSendProgressCompletionNotificationEmailFakes();

            // When
            notificationService.SendProgressCompletionNotificationEmail(progress, 2, 0);

            // Then
            A.CallTo(
                () => emailService.SendEmail(
                    A<Email>.That.Matches(
                        e => e.Body.TextBody.Contains(
                            lineOfText
                        )
                    )
                )
            ).MustNotHaveHappened();
        }

        private void SetUpSendProgressCompletionNotificationEmailFakes(
            int centreId = 101,
            string courseName = "Example application - course name",
            int? adminId = null,
            string? adminEmail = null,
            string delegateEmail = "",
            int sessionId = 123
        )
        {
            var progressCompletionData = new ProgressCompletionData
            {
                CentreId = centreId,
                CourseName = courseName,
                AdminId = adminId,
                SessionId = sessionId,
            };

            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(
                UserTestHelper.GetDefaultDelegateEntity(
                    DelegateId,
                    primaryEmail: delegateEmail
                )
            );
            if (adminId != null && adminEmail != null)
            {
                A.CallTo(() => userService.GetAdminById(adminId.Value)).Returns(
                    UserTestHelper.GetDefaultAdminEntity(
                        adminId.Value,
                        primaryEmail: adminEmail
                    )
                );
            }

            A.CallTo(() => notificationDataService.GetProgressCompletionData(ProgressId, DelegateId, CustomisationId))
                .Returns(progressCompletionData);
            A.CallTo(() => emailService.SendEmail(A<Email>._))
                .DoesNothing();
        }
    }
}
