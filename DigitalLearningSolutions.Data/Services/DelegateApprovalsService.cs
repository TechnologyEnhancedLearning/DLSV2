namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public interface IDelegateApprovalsService
    {
        public List<(DelegateEntity delegateEntity, List<CentreRegistrationPromptWithAnswer> prompts)>
            GetUnapprovedDelegatesWithRegistrationPromptAnswersForCentre(int centreId);

        public void ApproveDelegate(int delegateId, int centreId);

        public void ApproveAllUnapprovedDelegatesForCentre(int centreId);

        public void RejectDelegate(int delegateId, int centreId);
    }

    public class DelegateApprovalsService : IDelegateApprovalsService
    {
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly ICentresDataService centresDataService;
        private readonly IConfiguration config;
        private readonly IEmailService emailService;
        private readonly ILogger<DelegateApprovalsService> logger;
        private readonly IUserDataService userDataService;

        public DelegateApprovalsService(
            IUserDataService userDataService,
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IEmailService emailService,
            ICentresDataService centresDataService,
            ILogger<DelegateApprovalsService> logger,
            IConfiguration config
        )
        {
            this.userDataService = userDataService;
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.emailService = emailService;
            this.centresDataService = centresDataService;
            this.logger = logger;
            this.config = config;
        }

        private string LoginUrl => config["AppRootPath"] + "/Login";
        private string FindCentreUrl => config["AppRootPath"] + "/FindYourCentre";

        public List<(DelegateEntity delegateEntity, List<CentreRegistrationPromptWithAnswer> prompts)>
            GetUnapprovedDelegatesWithRegistrationPromptAnswersForCentre(int centreId)
        {
            var users = userDataService.GetUnapprovedDelegatesByCentreId(centreId);
            var usersWithPrompts =
                centreRegistrationPromptsService.GetCentreRegistrationPromptsWithAnswersByCentreIdForDelegates(
                    centreId,
                    users
                );

            return usersWithPrompts;
        }

        public void ApproveDelegate(int delegateId, int centreId)
        {
            var delegateEntity = userDataService.GetDelegateById(delegateId);

            if (delegateEntity == null || delegateEntity.DelegateAccount.CentreId != centreId)
            {
                throw new UserAccountNotFoundException(
                    $"Delegate user id {delegateId} not found at centre id {centreId}."
                );
            }

            if (delegateEntity.DelegateAccount.Approved)
            {
                logger.LogWarning($"Delegate user id {delegateId} already approved.");
            }
            else
            {
                userDataService.ApproveDelegateUsers(delegateId);

                SendDelegateApprovalEmails(delegateEntity);
            }
        }

        public void ApproveAllUnapprovedDelegatesForCentre(int centreId)
        {
            var delegateEntities = userDataService.GetUnapprovedDelegatesByCentreId(centreId).ToArray();

            userDataService.ApproveDelegateUsers(delegateEntities.Select(d => d.DelegateAccount.Id).ToArray());

            SendDelegateApprovalEmails(delegateEntities);
        }

        public void RejectDelegate(int delegateId, int centreId)
        {
            var delegateEntity = userDataService.GetDelegateById(delegateId);

            if (delegateEntity == null || delegateEntity.DelegateAccount.CentreId != centreId)
            {
                throw new UserAccountNotFoundException(
                    $"Delegate user id {delegateId} not found at centre id {centreId}."
                );
            }

            if (delegateEntity.DelegateAccount.Approved)
            {
                logger.LogWarning($"Delegate user id {delegateId} cannot be rejected as they are already approved.");
                throw new UserAccountInvalidStateException(
                    $"Delegate user id {delegateId} cannot be rejected as they are already approved."
                );
            }

            userDataService.RemoveDelegateAccount(delegateId);
            SendRejectionEmail(delegateEntity);
        }

        private void SendDelegateApprovalEmails(params DelegateEntity[] delegateEntities)
        {
            var approvalEmails = new List<Email>();
            foreach (var delegateEntity in delegateEntities)
            {
                var centreId = delegateEntity.DelegateAccount.CentreId;
                var centreInformationUrl =
                    centresDataService.GetCentreDetailsById(centreId)?.ShowOnMap == true
                        ? FindCentreUrl + $"?centreId={centreId}"
                        : null;
                var delegateApprovalEmail = GenerateDelegateApprovalEmail(
                    delegateEntity.DelegateAccount.CandidateNumber,
                    delegateEntity.EmailForCentreNotifications,
                    LoginUrl,
                    centreInformationUrl
                );
                approvalEmails.Add(delegateApprovalEmail);
            }

            emailService.SendEmails(approvalEmails);
        }

        private void SendRejectionEmail(DelegateEntity delegateEntity)
        {
            var delegateRejectionEmail = GenerateDelegateRejectionEmail(
                delegateEntity.UserAccount.FullName,
                delegateEntity.DelegateAccount.CentreName,
                delegateEntity.EmailForCentreNotifications,
                FindCentreUrl
            );
            emailService.SendEmail(delegateRejectionEmail);
        }

        private static Email GenerateDelegateApprovalEmail(
            string candidateNumber,
            string emailAddress,
            string? loginUrl,
            string? centreInformationUrl
        )
        {
            const string emailSubject = "Digital Learning Solutions Registration Approved";

            var body = new BodyBuilder
            {
                TextBody =
                    $@"Your Digital Learning Solutions registration has been approved by your centre administrator.
                    You can now log in to Digital Learning Solutions using your e-mail address or your Delegate ID number <b>""{candidateNumber}""</b> and the password you chose during registration, using the URL: {loginUrl} .
                    For more assistance in accessing the materials, please contact your Digital Learning Solutions centre.
                    {(centreInformationUrl == null ? "" : $@"View centre contact information: {centreInformationUrl}")}",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Your Digital Learning Solutions registration has been approved by your centre administrator.</p>
                                <p>You can now <a href=""{loginUrl}"">log in to Digital Learning Solutions</a> using your e-mail address or your Delegate ID number <b>""{candidateNumber}""</b> and the password you chose during registration.</p>
                                <p>For more assistance in accessing the materials, please contact your Digital Learning Solutions centre.</p>
                                {(centreInformationUrl == null ? "" : $@"<p><a href=""{centreInformationUrl}"">View centre contact information</a></p>")}
                            </body >",
            };

            return new Email(emailSubject, body, emailAddress);
        }

        private static Email GenerateDelegateRejectionEmail(
            string? delegateName,
            string centreName,
            string emailAddress,
            string findCentreUrl
        )
        {
            var emailSubject = "Digital Learning Solutions Registration Rejected";

            var body = new BodyBuilder
            {
                TextBody = $@"Dear {delegateName},
                        Your Digital Learning Solutions (DLS) registration at the centre {centreName} has been rejected by an administrator.
                        There are several reasons that this may have happened including:
                        -You registered with a non-work email address which was not recognised by the administrator
                        -Your DLS centre chooses to manage delegate registration internally
                        -You have accidentally chosen the wrong centre during the registration process.
                        If you need access to the DLS platform, please use the Find Your Centre page to locate your local DLS centre and use the contact details provided there to ask for help with registration. The Find Your Centre page can be found at this URL: {findCentreUrl}",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear {delegateName},</p>
                                <p>Your Digital Learning Solutions (DLS) registration at the centre {centreName} has been rejected by an administrator.</p>
                                <p>There are several reasons that this may have happened including:</p>
                                <ul>
                                    <li>You registered with a non-work email address which was not recognised by the administrator</li>
                                    <li>Your DLS centre chooses to manage delegate registration internally</li>
                                    <li>You have accidentally chosen the wrong centre during the registration process.</li>
                                </ul>
                                <p>If you need access to the DLS platform, please use the <a href=""{findCentreUrl}"">Find Your Centre</a> page to locate your local DLS centre and use the contact details provided there to ask for help with registration.</p>
                            </body >",
            };

            return new Email(emailSubject, body, emailAddress);
        }
    }
}
