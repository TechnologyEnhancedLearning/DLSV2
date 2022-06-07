namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate;

    public static class AccountDetailsDataHelper
    {
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

            var delegateDetailsData = userDelegateId != null ? new DelegateDetailsData(
                    userDelegateId.Value,
                    formData.Answer1,
                    formData.Answer2,
                    formData.Answer3,
                    formData.Answer4,
                    formData.Answer5,
                    formData.Answer6
                ) : null;
            return (accountDetailsData, delegateDetailsData);
        }

        public static (EditDelegateDetailsData, RegistrationFieldAnswers) MapToUpdateAccountData(
            EditDelegateFormData formData,
            int userDelegateId,
            int centreId
        )
        {
            var accountDetailsData = new EditDelegateDetailsData(
                userDelegateId,
                formData.FirstName!,
                formData.LastName!,
                formData.Email!,
                formData.AliasId,
                formData.HasProfessionalRegistrationNumber == true
                    ? formData.ProfessionalRegistrationNumber
                    : null,
                formData.HasProfessionalRegistrationNumber.HasValue
            );

            var centreAnswersData = new RegistrationFieldAnswers(
                centreId,
                formData.JobGroupId!.Value,
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6
            );
            return (accountDetailsData, centreAnswersData);
        }
    }
}
