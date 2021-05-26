namespace DigitalLearningSolutions.Data.Services
{
    using System;
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

        public void ApproveDelegate(int delegateId);
        public void ApproveAllUnapprovedDelegatesForCentre(int centreId);
    }

    public class DelegateApprovalsService : IDelegateApprovalsService
    {
        private readonly IUserDataService userDataService;
        private readonly ICustomPromptsService customPromptsService;
        private readonly IEmailService emailService;
        private readonly ILogger<DelegateApprovalsService> logger;

        public DelegateApprovalsService(IUserDataService userDataService, ICustomPromptsService customPromptsService,
            IEmailService emailService, ILogger<DelegateApprovalsService> logger)
        {
            this.userDataService = userDataService;
            this.customPromptsService = customPromptsService;
            this.emailService = emailService;
            this.logger = logger;
        }

        public List<(DelegateUser delegateUser, List<CustomPromptWithAnswer> prompts)> GetUnapprovedDelegatesWithCustomPromptAnswersForCentre(int centreId)
        {
            var users = userDataService.GetUnapprovedDelegateUsersByCentreId(centreId);
            var usersWithPrompts =
                customPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers(centreId, users);

            return usersWithPrompts;
        }

        public async void ApproveDelegate(int delegateId)
        {
            var delegateUser = userDataService.GetDelegateUserById(delegateId);

            if (delegateUser == null)
            {
                throw new UserAccountNotFoundException($"Delegate user id {delegateId} not found");
            }
            if (delegateUser.Approved)
            {
                logger.LogWarning($"Approval request sent for already approved delegate user id {delegateId}.");
            }
            else
            {
                await userDataService.ApproveDelegateUsers(new[] { delegateId });
                var delegateApprovalEmail = GenerateDelegateApprovalEmail(delegateUser.CandidateNumber, delegateUser.EmailAddress);
                emailService.SendEmail(delegateApprovalEmail);
            }
        }

        public async void ApproveAllUnapprovedDelegatesForCentre(int centreId)
        {
            var delegateUsers = userDataService.GetUnapprovedDelegateUsersByCentreId(centreId);

            await userDataService.ApproveDelegateUsers(delegateUsers.Select(d => d.Id));
            foreach (var delegateUser in delegateUsers)
            {
                var delegateApprovalEmail = GenerateDelegateApprovalEmail(delegateUser.CandidateNumber, delegateUser.EmailAddress);
                emailService.SendEmail(delegateApprovalEmail);
            }
        }

        private static Email GenerateDelegateApprovalEmail(
            string candidateNumber,
            string emailAddress)
        {
            string emailSubject = "Digital Learning Solutions Registration Approved";

            // TODO HEEDLS-423 do we want a 'dear X' salutation here?
            var body = new BodyBuilder
                {
                    TextBody = $@"Your Digital Learning Solutions registration has been approved by your centre administrator.
                            You can now login to the Digital Learning Solutions learning materials using your e-mail address or your Delegate ID number <b>"" & {candidateNumber} & ""</b> and the password you chose during registration.
                            For more assistance in accessing the materials, please contact your Digital Learning Solutions centre.",
                    HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'>
                                    <p>Your Digital Learning Solutions registration has been approved by your centre administrator.</p>
                                    <p>You can now login to the Digital Learning Solutions learning materials using your e-mail address or your Delegate ID number <b>"" & {candidateNumber} & ""</b> and the password you chose during registration.</p>
                                    <p>For more assistance in accessing the materials, please contact your Digital Learning Solutions centre.</p>
                                </body >"
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
