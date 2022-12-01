namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;

    public interface IEmailDataService
    {
        public void ScheduleEmails(
            IEnumerable<Email> emails,
            string senderAddress,
            string addedByProcess,
            bool urgent,
            DateTime? deliverAfter = null
        );

        public EmailOutDetails? GetEmailOutDetails(string email);
    }

    public class EmailDataService : IEmailDataService
    {
        private readonly IDbConnection connection;

        public EmailDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public EmailOutDetails? GetEmailOutDetails(string email)
        {
            return connection.Query<EmailOutDetails>(
                @"SELECT TOP 1
                    eo.EmailID as EmailID,
                    eo.EmailTo as EmailTo,
                    eo.Added as Added,
                    eo.Sent as Sent,
                    eo.DeliverAfter as DeliverAfter
                    FROM EmailOut eo
                    WHERE eo.EmailTo = @email Order by 1 ASC",
                new { email }
            ).SingleOrDefault();
        }

        public void ScheduleEmails(
            IEnumerable<Email> emails,
            string senderAddress,
            string addedByProcess,
            bool urgent,
            DateTime? deliverAfter = null
        )
        {
            var emailParams = emails.Select(
                email => new
                {
                    emailTo = email.To,
                    emailFrom = senderAddress,
                    emailSubject = email.Subject,
                    emailBody = email.Body.HtmlBody,
                    addedByProcess,
                    urgent,
                    deliverAfter
                }
            );

            connection.Execute(
                @"INSERT INTO
                        EmailOut (EmailTo, EmailFrom, Subject, BodyHTML, AddedByProcess, Urgent, DeliverAfter)
                        VALUES (@emailTo, @emailFrom, @emailSubject, @emailBody, @addedByProcess, @urgent, @deliverAfter)",
                emailParams
            );
        }
    }
}
