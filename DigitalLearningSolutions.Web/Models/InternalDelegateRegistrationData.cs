namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.Register;

    public class InternalDelegateRegistrationData
    {
        public InternalDelegateRegistrationData()
        {
            Id = Guid.NewGuid();
        }

        public InternalDelegateRegistrationData(int? centreId, int? supervisorDelegateId = null, string? email = null)
        {
            Id = Guid.NewGuid();
            Centre = centreId;
            IsCentreSpecificRegistration = centreId.HasValue;
            SupervisorDelegateId = supervisorDelegateId;
            Email = email;
        }

        public Guid Id { get; set; }
        public bool IsCentreSpecificRegistration { get; set; }
        public int? SupervisorDelegateId { get; set; }
        public int? Centre { get; set; }
        public string? CentreSpecificEmail { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }

        public string? Email { get; set; }

        public virtual void SetPersonalInformation(InternalPersonalInformationViewModel model)
        {
            Centre = model.Centre;
            CentreSpecificEmail = model.CentreSpecificEmail;
        }

        public void SetLearnerInformation(InternalLearnerInformationViewModel model)
        {
            Answer1 = model.Answer1;
            Answer2 = model.Answer2;
            Answer3 = model.Answer3;
            Answer4 = model.Answer4;
            Answer5 = model.Answer5;
            Answer6 = model.Answer6;
        }

        public void ClearCustomPromptAnswers()
        {
            Answer1 = null;
            Answer2 = null;
            Answer3 = null;
            Answer4 = null;
            Answer5 = null;
            Answer6 = null;
        }
    }
}
