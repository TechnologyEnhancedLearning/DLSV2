namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class EmailVerificationTransactionData
    {
        public EmailVerificationTransactionData() { } // Constructor for Builder to use in tests

        public EmailVerificationTransactionData(
            string email,
            DateTime hashCreationDate,
            int? centreIdIfEmailIsForUnapprovedDelegate,
            int userId
        )
        {
            Email = email;
            HashCreationDate = hashCreationDate;
            CentreIdIfEmailIsForUnapprovedDelegate = centreIdIfEmailIsForUnapprovedDelegate;
            UserId = userId;
        }

        public string Email { get; set; }

        public int UserId { get; set; }

        public DateTime? HashCreationDate { get; set; }

        public int? CentreIdIfEmailIsForUnapprovedDelegate { get; set; }
    }
}
