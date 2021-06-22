namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.Register;

    public class RegistrationData
    {
        public RegistrationData()
        {
            Id = new Guid();
        }

        public Guid Id { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int? Centre { get; set; }

        public int? JobGroup { get; set; }

        public string? PasswordHash { get; set; }

        public void SetPersonalInformation(PersonalInformationViewModel model)
        {
            Centre = model.Centre;
            Email = model.Email;
            FirstName = model.FirstName;
            LastName = model.LastName;
        }

        public void SetLearnerInformation(LearnerInformationViewModel model)
        {
            JobGroup = model.JobGroup;
        }
    }
}
