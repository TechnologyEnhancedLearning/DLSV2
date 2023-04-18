namespace DigitalLearningSolutions.Data.Models.Register
{
    public class RegistrationModel
    {
        public RegistrationModel(
            string firstName,
            string lastName,
            string primaryEmail,
            string? centreSpecificEmail,
            int centre,
            string? passwordHash,
            bool centreAccountIsActive,
            bool approved,
            string? professionalRegistrationNumber,
            int jobGroupId
        )
        {
            FirstName = firstName;
            LastName = lastName;
            PrimaryEmail = primaryEmail;
            CentreSpecificEmail = centreSpecificEmail;
            Centre = centre;
            PasswordHash = passwordHash;
            CentreAccountIsActive = centreAccountIsActive;
            Approved = approved;
            ProfessionalRegistrationNumber = professionalRegistrationNumber;
            JobGroup = jobGroupId;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PrimaryEmail { get; set; }

        public string? CentreSpecificEmail { get; set; }

        public int Centre { get; set; }

        public string? PasswordHash { get; set; }

        public bool Approved { get; set; }

        public bool CentreAccountIsActive { get; set; }

        public string? ProfessionalRegistrationNumber { get; set; }

        public int JobGroup { get; set; }
    }
}
