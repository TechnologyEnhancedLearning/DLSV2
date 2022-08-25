namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Models;

    public static class RegistrationMappingHelper
    {
        private static readonly IClockUtility ClockUtility = new ClockUtility();

        public static AdminRegistrationModel MapToCentreManagerAdminRegistrationModel(RegistrationData data)
        {
            return new AdminRegistrationModel(
                data.FirstName!,
                data.LastName!,
                data.PrimaryEmail!,
                data.CentreSpecificEmail,
                data.Centre!.Value,
                data.PasswordHash!,
                true,
                true,
                data.ProfessionalRegistrationNumber,
                data.JobGroup!.Value,
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
                data.PrimaryEmail!,
                data.CentreSpecificEmail,
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
                true,
                data.ProfessionalRegistrationNumber,
                notifyDate: ClockUtility.UtcNow
            );
        }

        public static InternalDelegateRegistrationModel
            MapInternalDelegateRegistrationDataToInternalDelegateRegistrationModel(
                InternalDelegateRegistrationData data
            )
        {
            return new InternalDelegateRegistrationModel(
                data.Centre!.Value,
                data.CentreSpecificEmail,
                data.Answer1,
                data.Answer2,
                data.Answer3,
                data.Answer4,
                data.Answer5,
                data.Answer6
            );
        }

        public static DelegateRegistrationModel MapCentreRegistrationToDelegateRegistrationModel(
            DelegateRegistrationByCentreData data
        )
        {
            return new DelegateRegistrationModel(
                data.FirstName!,
                data.LastName!,
                Guid.NewGuid().ToString(),
                data.CentreSpecificEmail,
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
                true,
                data.ProfessionalRegistrationNumber,
                true,
                data.WelcomeEmailDate
            );
        }

        public static DelegateRegistrationModel MapDelegateUploadTableRowToDelegateRegistrationModel(
            DelegateTableRow delegateTableRow,
            DateTime welcomeEmailDate,
            int centreId
        )
        {
            return new DelegateRegistrationModel(
                delegateTableRow.FirstName!,
                delegateTableRow.LastName!,
                Guid.NewGuid().ToString(),
                delegateTableRow.Email,
                centreId,
                delegateTableRow.JobGroupId!.Value,
                null,
                delegateTableRow.Answer1,
                delegateTableRow.Answer2,
                delegateTableRow.Answer3,
                delegateTableRow.Answer4,
                delegateTableRow.Answer5,
                delegateTableRow.Answer6,
                false,
                delegateTableRow.Active!.Value,
                true,
                delegateTableRow.Prn,
                true,
                welcomeEmailDate
            );
        }
    }
}
