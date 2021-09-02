namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Email;

    public interface IEmailDataService
    {
        public void ScheduleEmails(
            IEnumerable<Email> emails,
            string senderAddress,
            string addedByProcess,
            bool urgent,
            DateTime deliverAfter
        );
    }

    public class EmailDataService : IEmailDataService
    {
        private readonly IDbConnection connection;

        public EmailDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public void ScheduleEmails(
            IEnumerable<Email> emails,
            string senderAddress,
            string addedByProcess,
            bool urgent,
            DateTime deliverAfter
        )
        {
            foreach (var email in emails)
            {
                connection.Execute(
                    @"INSERT INTO
                        EmailOut (EmailTo, EmailFrom, Subject, BodyHTML, AddedByProcess, Urgent, DeliverAfter)
                        VALUES (@emailTo, @emailFrom, @emailSubject, @emailBody, @addedByProcess, @urgent, @deliverAfter)",
                    new
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
            }
        }
    }
}
