namespace DigitalLearningSolutions.Data.Models.User
{
    public abstract class User
    {
        public int Id { get; set; }

        public int CentreId { get; set; }

        public string CentreName { get; set; }

        public string? FirstName { get; set; }

        public string Surname { get; set; }

        public string? EmailAddress { get; set; }

        public string? Password { get; set; }

        public int? ResetPasswordId { get; set; }
    }
}
