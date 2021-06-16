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

        public static SummaryViewModel MapDataToSummaryViewModel(DelegateRegistrationData data)
        {
            return new SummaryViewModel
            {
                FirstName = data.FirstName!,
                LastName = data.LastName!,
                Email = data.Email!,
                IsCentreSpecificRegistration = data.IsCentreSpecificRegistration
            };
        }

        public static PersonalInformationViewModel MapDataToRegisterViewModel(DelegateRegistrationData data)
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

        public static LearnerInformationViewModel MapDataToLearnerInformationViewModel(
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

        public static DelegateRegistrationData MapRegisterViewModelToData(
            PersonalInformationViewModel model,
            DelegateRegistrationData data
        )
        {
            data.Centre = model.Centre;
            data.Email = model.Email;
            data.FirstName = model.FirstName;
            data.LastName = model.LastName;
            return data;
        }

        public static DelegateRegistrationData MapLearnerInformationViewModelToData(
            LearnerInformationViewModel model,
            DelegateRegistrationData data
        )
        {
            data.JobGroup = model.JobGroup;
            data.Answer1 = model.Answer1;
            data.Answer2 = model.Answer2;
            data.Answer3 = model.Answer3;
            data.Answer4 = model.Answer4;
            data.Answer5 = model.Answer5;
            data.Answer6 = model.Answer6;
            return data;
        }
    }
}
