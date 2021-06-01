namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public interface IDelegateApprovalsService
    {
        public List<(DelegateUser delegateUser, List<CustomPromptWithAnswer> prompts)>
            GetUnapprovedDelegatesWithCustomPromptAnswersForCentre(int centreId);

        public void ApproveDelegate(int delegateId, int centreId);
        public void ApproveAllUnapprovedDelegatesForCentre(int centreId);
    }

    public class DelegateApprovalsService : IDelegateApprovalsService
    {
        private readonly ICustomPromptsService customPromptsService;
        private readonly IEmailService emailService;
        private readonly ILogger<DelegateApprovalsService> logger;
        private readonly IUserDataService userDataService;

        public DelegateApprovalsService
        (
            IUserDataService userDataService,
            ICustomPromptsService customPromptsService,
            IEmailService emailService,
            ILogger<DelegateApprovalsService> logger
        )
        {
            this.userDataService = userDataService;
            this.customPromptsService = customPromptsService;
            this.emailService = emailService;
            this.logger = logger;
        }

        public List<(DelegateUser delegateUser, List<CustomPromptWithAnswer> prompts)>
            GetUnapprovedDelegatesWithCustomPromptAnswersForCentre(int centreId)
        {
            var users = userDataService.GetUnapprovedDelegateUsersByCentreId(centreId);
            var usersWithPrompts =
                customPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers(centreId, users);

            return usersWithPrompts;
        }

        public void ApproveDelegate(int delegateId, int centreId)
        {
            var delegateUser = userDataService.GetDelegateUserByIdFromCentre(delegateId, centreId);

            if (delegateUser == null)
            {
                throw new UserAccountNotFoundException($"Delegate user id {delegateId} not found at centre id {centreId}.");
            }

            if (delegateUser.Approved)
            {
                logger.LogWarning($"Delegate user id {delegateId} already approved.");
            }
            else
            {
                userDataService.ApproveDelegateUsers(new[] { delegateId });

                if (string.IsNullOrWhiteSpace(delegateUser.EmailAddress))
                {
                    LogNoEmailWarning(delegateUser.Id);
                }
                else
                {
                    var delegateApprovalEmail = GenerateDelegateApprovalEmail
                        (delegateUser.CandidateNumber, delegateUser.EmailAddress);
                    emailService.SendEmail(delegateApprovalEmail);
                }
            }
        }

        public void ApproveAllUnapprovedDelegatesForCentre(int centreId)
        {
            var delegateUsers = userDataService.GetUnapprovedDelegateUsersByCentreId(centreId);

            userDataService.ApproveDelegateUsers(delegateUsers.Select(d => d.Id));

            var approvalEmails = new List<Email>();
            foreach (var delegateUser in delegateUsers)
            {
                if (string.IsNullOrWhiteSpace(delegateUser.EmailAddress))
                {
                    LogNoEmailWarning(delegateUser.Id);
                }
                else
                {
                    var delegateApprovalEmail = GenerateDelegateApprovalEmail
                        (delegateUser.CandidateNumber, delegateUser.EmailAddress);
                    approvalEmails.Add(delegateApprovalEmail);
                }
            }
            emailService.SendEmails(approvalEmails);
        }

        private void LogNoEmailWarning(int id)
        {
            logger.LogWarning($"Delegate user id {id} has no email associated with their account.");
        }

        private static Email GenerateDelegateApprovalEmail
        (
            string candidateNumber,
            string emailAddress
        )
        {
            const string emailSubject = "Digital Learning Solutions Registration Approved";

            var body = new BodyBuilder
            {
                TextBody =
                    $@"Your Digital Learning Solutions registration has been approved by your centre administrator.
                            You can now login to the Digital Learning Solutions learning materials using your e-mail address or your Delegate ID number <b>""{candidateNumber}""</b> and the password you chose during registration.
                            For more assistance in accessing the materials, please contact your Digital Learning Solutions centre.",
                HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'>
                                    <p>Your Digital Learning Solutions registration has been approved by your centre administrator.</p>
                                    <p>You can now login to the Digital Learning Solutions learning materials using your e-mail address or your Delegate ID number <b>""{candidateNumber}""</b> and the password you chose during registration.</p>
                                    <p>For more assistance in accessing the materials, please contact your Digital Learning Solutions centre.</p>
                                </body >"
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
