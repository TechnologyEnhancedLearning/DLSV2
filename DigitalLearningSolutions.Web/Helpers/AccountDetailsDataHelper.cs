namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate;

    public static class AccountDetailsDataHelper
    {
        public static (MyAccountDetailsData, CentreAnswersData?) MapToUpdateAccountData(
            MyAccountEditDetailsFormData formData,
            int? userAdminId,
            int? userDelegateId,
            int centreId
        )
        {
            var accountDetailsData = new MyAccountDetailsData(
                userAdminId,
                userDelegateId,
                formData.Password!,
                formData.FirstName!,
                formData.LastName!,
                formData.Email!,
                formData.ProfileImage
            );

            var centreAnswersData = userDelegateId == null
                ? null
                : new CentreAnswersData(
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

        public static (EditDelegateDetailsData, CentreAnswersData) MapToUpdateAccountData(
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
                formData.AliasId
            );

            var centreAnswersData = new CentreAnswersData(
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
