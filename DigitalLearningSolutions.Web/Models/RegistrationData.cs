namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;

    public class RegistrationData
    {
        public RegistrationData()
        {
            Id = new Guid();
        }

        public RegistrationData(int? centreId)
        {
            Id = new Guid();
            Centre = centreId;
        }

        public Guid Id { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? CentreSpecificEmail { get; set; }
        public int? Centre { get; set; }

        public int? JobGroup { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool? HasProfessionalRegistrationNumber { get; set; }

        public string? PasswordHash { get; set; }

        public void SetPersonalInformation(PersonalInformationViewModel model)
        {
            Centre = model.Centre;
            PrimaryEmail = model.PrimaryEmail;
            CentreSpecificEmail = model.CentreSpecificEmail;
            FirstName = model.FirstName;
            LastName = model.LastName;
        }

        public void SetPersonalInformation(DelegatePersonalInformationViewModel model)
        {
            Centre = model.Centre;
            CentreSpecificEmail = model.CentreSpecificEmail;
            FirstName = model.FirstName;
            LastName = model.LastName;
        }

        public virtual void SetLearnerInformation(LearnerInformationViewModel model)
        {
            JobGroup = model.JobGroup;
            ProfessionalRegistrationNumber = model.HasProfessionalRegistrationNumber == true
                ? model.ProfessionalRegistrationNumber
                : null;
            HasProfessionalRegistrationNumber = model.HasProfessionalRegistrationNumber;
        }
    }
}
