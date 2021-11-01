namespace DigitalLearningSolutions.Data.Models.Register
{
    public class RegistrationModel
    {
        public RegistrationModel(
            string firstName,
            string lastName,
            string email,
            int centre,
            string? passwordHash,
            bool active,
            bool approved
        )
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Centre = centre;
            PasswordHash = passwordHash;
            Active = active;
            Approved = approved;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int Centre { get; set; }

        public string? PasswordHash { get; set; }

        public bool Approved { get; set; }

        public bool Active { get; set; }
    }
}
