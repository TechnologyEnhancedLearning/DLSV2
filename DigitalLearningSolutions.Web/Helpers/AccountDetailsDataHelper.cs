namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate;

    public static class AccountDetailsDataHelper
    {
        // TODO: 951 - make these into wrapper methods around a private method
        public static (MyAccountDetailsData, DelegateDetailsData?) MapToUpdateAccountData(
            MyAccountEditDetailsFormData formData,
            int userId,
            int? userDelegateId
        )
        {
            var accountDetailsData = new MyAccountDetailsData(
                userId,
                formData.FirstName!,
                formData.LastName!,
                formData.Email!,
                formData.JobGroupId!.Value,
                formData.HasProfessionalRegistrationNumber == true
                    ? formData.ProfessionalRegistrationNumber
                    : null,
                formData.HasProfessionalRegistrationNumber.HasValue,
                formData.ProfileImage
            );

            var delegateDetailsData = userDelegateId != null
                ? new DelegateDetailsData(
                    userDelegateId.Value,
                    formData.Answer1,
                    formData.Answer2,
                    formData.Answer3,
                    formData.Answer4,
                    formData.Answer5,
                    formData.Answer6
                )
                : null;
            return (accountDetailsData, delegateDetailsData);
        }

        public static (MyAccountDetailsData, DelegateDetailsData) MapToUpdateAccountData(
            EditDelegateFormData formData,
            int userId,
            int userDelegateId,
            byte[]? profileImage
        )
        {
            var accountDetailsData = new MyAccountDetailsData(
                userId,
                formData.FirstName!,
                formData.LastName!,
                formData.Email!,
                formData.JobGroupId!.Value,
                formData.HasProfessionalRegistrationNumber == true
                    ? formData.ProfessionalRegistrationNumber
                    : null,
                formData.HasProfessionalRegistrationNumber.HasValue,
                profileImage
            );

            var delegateDetailsData = new DelegateDetailsData(
                userDelegateId,
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6
            );
            return (accountDetailsData, delegateDetailsData);
        }
    }
}
