namespace DigitalLearningSolutions.Web.Models
{
    using DigitalLearningSolutions.Web.ViewModels.Register;

    public class DelegateRegistrationData : RegistrationData
    {
        public DelegateRegistrationData() { }

        public DelegateRegistrationData(
            int? centreId,
            int? supervisorDelegateId = null,
            string? primaryEmail = null
        ) : base(centreId)
        {
            IsCentreSpecificRegistration = centreId.HasValue;
            SupervisorDelegateId = supervisorDelegateId;
            PrimaryEmail = primaryEmail;
        }

        public bool IsCentreSpecificRegistration { get; set; }
        public int? SupervisorDelegateId { get; set; }

        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }

        public override void SetLearnerInformation(LearnerInformationViewModel model)
        {
            JobGroup = model.JobGroup;
            ProfessionalRegistrationNumber = model.HasProfessionalRegistrationNumber == true
                ? model.ProfessionalRegistrationNumber
                : null;
            HasProfessionalRegistrationNumber = model.HasProfessionalRegistrationNumber;
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
