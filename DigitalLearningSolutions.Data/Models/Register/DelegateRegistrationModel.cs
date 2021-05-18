namespace DigitalLearningSolutions.Data.Models.Register
{
    public class DelegateRegistrationModel
    {
        public DelegateRegistrationModel(
            string firstName,
            string lastName,
            string email,
            int centre,
            int jobGroup,
            string passwordHash,
            bool approved)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Centre = centre;
            JobGroup = jobGroup;
            PasswordHash = passwordHash;
            Approved = approved;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int Centre { get; set; }

        public int JobGroup { get; set; }

        public string PasswordHash { get; set; }

        public bool Approved { get; set; }
    }
}
