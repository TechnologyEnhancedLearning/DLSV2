namespace DigitalLearningSolutions.Data.Models.User
{
    public class AccountDetailsData
    {
        public AccountDetailsData(
            string firstName,
            string surname,
            string email
        )
        {
            FirstName = firstName;
            Surname = surname;
            Email = email;
        }

        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}
