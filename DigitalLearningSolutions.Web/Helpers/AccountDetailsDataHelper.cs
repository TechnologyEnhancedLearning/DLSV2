﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate;

    public static class AccountDetailsDataHelper
    {
        public static (EditAccountDetailsData, DelegateDetailsData?) MapToEditAccountDetailsData(
            MyAccountEditDetailsFormData formData,
            int userId,
            int? userDelegateId
        )
        {
            return MapToEditAccountDetailsData(
                userId,
                formData.FirstName!,
                formData.LastName!,
                formData.Email!,
                formData.JobGroupId!.Value,
                formData.HasProfessionalRegistrationNumber.HasValue,
                formData.ProfessionalRegistrationNumber,
                userDelegateId,
                formData.ProfileImage,
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6
            );
        }

        public static (EditAccountDetailsData, DelegateDetailsData) MapToEditAccountDetailsData(
            EditDelegateFormData formData,
            int userId,
            int userDelegateId,
            byte[]? profileImage
        )
        {
            return MapToEditAccountDetailsData(
                userId,
                formData.FirstName!,
                formData.LastName!,
                formData.Email!,
                formData.JobGroupId!.Value,
                formData.HasProfessionalRegistrationNumber.HasValue,
                formData.ProfessionalRegistrationNumber,
                userDelegateId,
                profileImage,
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6
            );
        }

        private static (EditAccountDetailsData, DelegateDetailsData) MapToEditAccountDetailsData(
            int userId,
            string? firstName,
            string? lastName,
            string? email,
            int? jobGroupId,
            bool hasProfessionalRegistrationNumber,
            string? professionalRegistrationNumber,
            int? userDelegateId,
            byte[]? profileImage,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6
        )
        {
            var accountDetailsData = new EditAccountDetailsData(
                userId,
                firstName!,
                lastName!,
                email!,
                jobGroupId!.Value,
                hasProfessionalRegistrationNumber
                    ? professionalRegistrationNumber
                    : null,
                hasProfessionalRegistrationNumber,
                profileImage
            );

            var delegateDetailsData = new DelegateDetailsData(
                userDelegateId.Value,
                answer1,
                answer2,
                answer3,
                answer4,
                answer5,
                answer6
            );
            return (accountDetailsData, delegateDetailsData);
        }
    }
}
