namespace DigitalLearningSolutions.Data.Models.Register
{
    public class RegistrationModel
    {
        public RegistrationModel(
            string firstName,
            string lastName,
            string email,
            string? secondaryEmail,
            int centre,
            string? passwordHash,
            bool active,
            bool approved,
            string? professionalRegistrationNumber
        )
        {
            FirstName = firstName;
            LastName = lastName;
            PrimaryEmail = email;
            SecondaryEmail = secondaryEmail;
            Centre = centre;
            PasswordHash = passwordHash;
            Active = active;
            Approved = approved;
            ProfessionalRegistrationNumber = professionalRegistrationNumber;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PrimaryEmail { get; set; }

        public string? SecondaryEmail { get; set; }

        public int Centre { get; set; }

        public string? PasswordHash { get; set; }

        public bool Approved { get; set; }

        public bool Active { get; set; }

        public string? ProfessionalRegistrationNumber { get; set; }
    }
}
