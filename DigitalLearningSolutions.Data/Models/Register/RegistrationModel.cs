﻿namespace DigitalLearningSolutions.Data.Models.Register
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
            bool active,
            bool approved,
            string? professionalRegistrationNumber
        )
        {
            FirstName = firstName;
            LastName = lastName;
            PrimaryEmail = primaryEmail;
            CentreSpecificEmail = centreSpecificEmail;
            Centre = centre;
            PasswordHash = passwordHash;
            Active = active;
            Approved = approved;
            ProfessionalRegistrationNumber = professionalRegistrationNumber;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PrimaryEmail { get; set; }

        public string? CentreSpecificEmail { get; set; }

        public int Centre { get; set; }

        public string? PasswordHash { get; set; }

        public bool Approved { get; set; }

        public bool Active { get; set; }

        public string? ProfessionalRegistrationNumber { get; set; }
    }
}
