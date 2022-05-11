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
            PrimaryEmail = email;
            SecondaryEmail = null;
            Centre = centre;
            PasswordHash = passwordHash;
            Active = active;
            Approved = approved;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PrimaryEmail { get; set; }

        public string? SecondaryEmail { get; set; }

        public int Centre { get; set; }

        public string? PasswordHash { get; set; }

        public bool Approved { get; set; }

        public bool Active { get; set; }
    }
}
