namespace DigitalLearningSolutions.Data.Models.Register
{
    public class RegistrationModel
    {
        public RegistrationModel(
            string firstName,
            string lastName,
            string email,
            int centre,
            int jobGroup,
            string? passwordHash
        )
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Centre = centre;
            JobGroup = jobGroup;
            PasswordHash = passwordHash;
            Approved = false;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int Centre { get; set; }

        public int JobGroup { get; set; }

        public string? PasswordHash { get; set; }

        public bool Approved { get; set; }
    }
}
