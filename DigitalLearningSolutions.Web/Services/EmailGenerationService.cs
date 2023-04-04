namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.Models.Email;
    using MimeKit;

    public interface IEmailGenerationService
    {
        Email GenerateDelegateAdminRolesNotificationEmail(
            string firstName,
            string supervisorFirstName,
            string supervisorLastName,
            string supervisorEmail,
            bool isCentreAdmin,
            bool isCentreManager,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool isTrainer,
            bool isContentCreator,
            bool isCmsAdmin,
            bool isCmsManager,
            string primaryEmail,
            string centreName
        );
    }

    public class EmailGenerationService : IEmailGenerationService
    {
        public Email GenerateDelegateAdminRolesNotificationEmail(
            string firstName,
            string supervisorFirstName,
            string supervisorLastName,
            string supervisorEmail,
            bool isCentreAdmin,
            bool isCentreManager,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool isTrainer,
            bool isContentCreator,
            bool isCmsAdmin,
            bool isCmsManager,
            string primaryEmail,
            string centreName
        )
        {
            const string emailSubjectLine = "New Digital Learning Solutions permissions granted";

            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {firstName},
                                The user {supervisorFirstName} {supervisorLastName} has granted you new access permissions for the centre {centreName} in the Digital Learning Solutions system.
                                You have been granted the following permissions:",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear {firstName},</p>
                                <p>The user <a href = 'mailto:{supervisorEmail}'>{supervisorFirstName} {supervisorLastName}</a> has granted you new access permissions for the centre {centreName} in the Digital Learning Solutions system.</p>
                                <p>You have been granted the following permissions:</p>",
            };

            builder.HtmlBody += "<ul>";

            if (isCentreManager)
            {
                builder.TextBody += " Centre manager";
                builder.HtmlBody += "<li>Centre manager</li>";
            }
            if (isCentreAdmin)
            {
                builder.TextBody += " Centre administrator";
                builder.HtmlBody += "<li>Centre administrator</li>";
            }
            if (isSupervisor)
            {
                builder.TextBody += " Supervisor";
                builder.HtmlBody += "<li>Supervisor</li>";
            }
            if (isNominatedSupervisor)
            {
                builder.TextBody += " Nominated supervisor";
                builder.HtmlBody += "<li>Nominated supervisor</li>";
            }
            if (isTrainer)
            {
                builder.TextBody += " Trainer";
                builder.HtmlBody += "<li>Trainer</li>";
            }
            if (isContentCreator)
            {
                builder.TextBody += " Content Creator licence";
                builder.HtmlBody += "<li>Content Creator licence</li>";
            }
            if (isCmsAdmin)
            {
                builder.TextBody += " CMS administrator";
                builder.HtmlBody += "<li>CMS administrator</li>";
            }

            if (isCmsManager)
            {
                builder.TextBody += " CMS manager";
                builder.HtmlBody += "<li>CMS manager</li>";
            }

            builder.HtmlBody += "</ul>";

            builder.TextBody += $@"You will be able to access the Digital Learning Solutions platform with these new access permissions the next time you log in to {centreName}.";
            builder.HtmlBody += $@"You will be able to access the Digital Learning Solutions platform with these new access permissions the next time you log in to {centreName}.</body>";

            return new Email(emailSubjectLine, builder, primaryEmail, supervisorEmail);
        }
    }
}
