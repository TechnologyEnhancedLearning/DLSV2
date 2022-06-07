namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Web.Models;

    public static class RegistrationMappingHelper
    {
        public static AdminRegistrationModel MapToCentreManagerAdminRegistrationModel(RegistrationData data)
        {
            return new AdminRegistrationModel(
                data.FirstName!,
                data.LastName!,
                data.Email!,
                data.Centre!.Value,
                data.PasswordHash!,
                true,
                true,
                data.ProfessionalRegistrationNumber,
                null,
                true,
                true,
                false,
                false,
                false,
                false,
                false,
                false
            );
        }

        public static DelegateRegistrationModel MapSelfRegistrationToDelegateRegistrationModel(
            DelegateRegistrationData data
        )
        {
            return new DelegateRegistrationModel(
                data.FirstName!,
                data.LastName!,
                data.Email!,
                data.SecondaryEmail,
                data.Centre!.Value,
                data.JobGroup!.Value,
                data.PasswordHash!,
                data.Answer1,
                data.Answer2,
                data.Answer3,
                data.Answer4,
                data.Answer5,
                data.Answer6,
                true,
                true,
                data.ProfessionalRegistrationNumber,
                notifyDate: DateTime.Now
            );
        }

        public static DelegateRegistrationModel MapCentreRegistrationToDelegateRegistrationModel(
            DelegateRegistrationByCentreData data
        )
        {
            return new DelegateRegistrationModel(
                data.FirstName!,
                data.LastName!,
                data.Email!,
                data.SecondaryEmail,
                data.Centre!.Value,
                data.JobGroup!.Value,
                data.PasswordHash,
                data.Answer1,
                data.Answer2,
                data.Answer3,
                data.Answer4,
                data.Answer5,
                data.Answer6,
                false,
                true,
                data.ProfessionalRegistrationNumber,
                true,
                data.Alias,
                data.WelcomeEmailDate
            );
        }
    }
}
