namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Http;

    public class MyAccountEditDetailsFormData : EditDetailsFormData, IEditProfessionalRegistrationNumbers,
        IValidatableObject
    {
        public MyAccountEditDetailsFormData() { }

        protected MyAccountEditDetailsFormData(
            UserAccount userAccount,
            DelegateAccount? delegateAccount,
            List<(int id, string name)> jobGroups,
            string? centreEmail,
            string? returnUrl,
            bool isCheckDetailRedirect
        )
        {
            FirstName = userAccount.FirstName;
            LastName = userAccount.LastName;
            Email = userAccount.PrimaryEmail;
            ProfileImage = userAccount.ProfileImage;
            ProfessionalRegistrationNumber = userAccount.ProfessionalRegistrationNumber;
            HasProfessionalRegistrationNumber =
                ProfessionalRegistrationNumberHelper.GetHasProfessionalRegistrationNumberForView(
                    userAccount.HasBeenPromptedForPrn,
                    userAccount.ProfessionalRegistrationNumber
                );
            JobGroupId = jobGroups.Where(jg => jg.name == userAccount.JobGroupName).Select(jg => jg.id)
                .SingleOrDefault();

            IsDelegateUser = delegateAccount != null;
            Answer1 = delegateAccount?.Answer1;
            Answer2 = delegateAccount?.Answer2;
            Answer3 = delegateAccount?.Answer3;
            Answer4 = delegateAccount?.Answer4;
            Answer5 = delegateAccount?.Answer5;
            Answer6 = delegateAccount?.Answer6;

            CentreEmail = centreEmail;
            ReturnUrl = returnUrl;
            IsSelfRegistrationOrEdit = true;
            IsCheckDetailRedirect = isCheckDetailRedirect;
        }

        protected MyAccountEditDetailsFormData(MyAccountEditDetailsFormData formData)
        {
            FirstName = formData.FirstName;
            LastName = formData.LastName;
            Email = formData.Email;
            CentreEmail = formData.CentreEmail;
            ProfileImageFile = formData.ProfileImageFile;
            ProfileImage = formData.ProfileImage;
            IsDelegateUser = formData.IsDelegateUser;
            JobGroupId = formData.JobGroupId;
            Answer1 = formData.Answer1;
            Answer2 = formData.Answer2;
            Answer3 = formData.Answer3;
            Answer4 = formData.Answer4;
            Answer5 = formData.Answer5;
            Answer6 = formData.Answer6;
            HasProfessionalRegistrationNumber = formData.HasProfessionalRegistrationNumber;
            ProfessionalRegistrationNumber = formData.ProfessionalRegistrationNumber;
            ReturnUrl = formData.ReturnUrl;
            IsSelfRegistrationOrEdit = true;
            IsCheckDetailRedirect = formData.IsCheckDetailRedirect;
        }

        public byte[]? ProfileImage { get; set; }

        [AllowedExtensions(new[] { ".png", ".tiff", ".jpg", ".jpeg", ".bmp", ".gif" })]
        public IFormFile? ProfileImageFile { get; set; }

        public bool IsDelegateUser { get; set; }

        public string? ReturnUrl { get; set; }

        public bool IsCheckDetailRedirect { get; set; }
    }
}
