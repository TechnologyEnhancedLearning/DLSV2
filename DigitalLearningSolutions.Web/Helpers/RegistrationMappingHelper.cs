﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Web.Models;

    public static class RegistrationMappingHelper
    {
        public static RegistrationModel MapToRegistrationModel(RegistrationData data)
        {
            return new RegistrationModel(
                data.FirstName!,
                data.LastName!,
                data.Email!,
                data.Centre!.Value,
                data.JobGroup!.Value,
                data.PasswordHash!
            );
        }

        public static DelegateRegistrationModel MapToDelegateRegistrationModel(DelegateRegistrationData data)
        {
            return new DelegateRegistrationModel(
                data.FirstName!,
                data.LastName!,
                data.Email!,
                data.Centre!.Value,
                data.JobGroup!.Value,
                data.PasswordHash!,
                data.Answer1,
                data.Answer2,
                data.Answer3,
                data.Answer4,
                data.Answer5,
                data.Answer6
            );
        }

        public static DelegateRegistrationModel MapToDelegateRegistrationModel(DelegateRegistrationByCentreData data)
        {
            return new DelegateRegistrationModel(
                data.FirstName!,
                data.LastName!,
                data.Email!,
                data.Centre!.Value,
                data.JobGroup!.Value,
                data.IsPasswordSet ? data.PasswordHash! : null,
                data.Answer1,
                data.Answer2,
                data.Answer3,
                data.Answer4,
                data.Answer5,
                data.Answer6
            ) {AliasId = data.Alias, NotifyDate = data.WelcomeEmailDate};
        }
    }
}
