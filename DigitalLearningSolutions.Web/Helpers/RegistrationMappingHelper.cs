namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Register;

    public static class RegistrationMappingHelper
    {
        public static DelegateRegistrationModel MapToDelegateRegistrationModel(DelegateRegistrationData data)
        {
            return new DelegateRegistrationModel(
                data.FirstName!,
                data.LastName!,
                data.Email!,
                (int)data.Centre!,
                (int)data.JobGroup!,
                data.PasswordHash!,
                data.Answer1,
                data.Answer2,
                data.Answer3,
                data.Answer4,
                data.Answer5,
                data.Answer6
            );
        }

        public static SummaryViewModel MapDataToSummary(DelegateRegistrationData data)
        {
            return new SummaryViewModel
            {
                FirstName = data.FirstName!,
                LastName = data.LastName!,
                Email = data.Email!,
                IsCentreSpecificRegistration = data.IsCentreSpecificRegistration
            };
        }

        public static PersonalInformationViewModel MapDataToPersonalInformation(DelegateRegistrationData data)
        {
            return new PersonalInformationViewModel
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                Centre = data.Centre,
                Email = data.Email,
                IsCentreSpecificRegistration = data.IsCentreSpecificRegistration
            };
        }

        public static LearnerInformationViewModel MapDataToLearnerInformation(
            DelegateRegistrationData data
        )
        {
            return new LearnerInformationViewModel
            {
                JobGroup = data.JobGroup,
                Answer1 = data.Answer1,
                Answer2 = data.Answer2,
                Answer3 = data.Answer3,
                Answer4 = data.Answer4,
                Answer5 = data.Answer5,
                Answer6 = data.Answer6
            };
        }
    }
}
