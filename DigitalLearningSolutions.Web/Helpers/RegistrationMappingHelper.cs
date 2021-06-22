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

        public static SummaryViewModel MapDataToSummary(RegistrationData data)
        {
            return new SummaryViewModel
            {
                FirstName = data.FirstName!,
                LastName = data.LastName!,
                Email = data.Email!
            };
        }

        public static SummaryViewModel MapDataToSummary(DelegateRegistrationData data)
        {
            var model = MapDataToSummary((RegistrationData)data);
            model.IsCentreSpecificRegistration = data.IsCentreSpecificRegistration;
            return model;
        }

        public static PersonalInformationViewModel MapDataToPersonalInformation(RegistrationData data)
        {
            return new PersonalInformationViewModel
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                Centre = data.Centre,
                Email = data.Email
            };
        }

        public static PersonalInformationViewModel MapDataToPersonalInformation(DelegateRegistrationData data)
        {
            var model = MapDataToPersonalInformation((RegistrationData)data);
            model.IsCentreSpecificRegistration = data.IsCentreSpecificRegistration;
            return model;
        }

        public static LearnerInformationViewModel MapDataToLearnerInformation(RegistrationData data)
        {
            return new LearnerInformationViewModel
            {
                JobGroup = data.JobGroup
            };
        }

        public static LearnerInformationViewModel MapDataToLearnerInformation(DelegateRegistrationData data)
        {
            var model = MapDataToLearnerInformation((RegistrationData)data);
            model.Answer1 = data.Answer1;
            model.Answer2 = data.Answer2;
            model.Answer3 = data.Answer3;
            model.Answer4 = data.Answer4;
            model.Answer5 = data.Answer5;
            model.Answer6 = data.Answer6;
            return model;
        }
    }
}
