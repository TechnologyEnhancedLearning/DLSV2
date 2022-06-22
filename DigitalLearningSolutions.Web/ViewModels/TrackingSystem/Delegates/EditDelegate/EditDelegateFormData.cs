namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class EditDelegateFormData : EditDetailsFormData, IEditProfessionalRegistrationNumbers, IValidatableObject
    {
        public EditDelegateFormData() { }

        public EditDelegateFormData(DelegateEntity delegateEntity, IEnumerable<(int id, string name)> jobGroups)
        {
            FirstName = delegateEntity.UserAccount.FirstName;
            LastName = delegateEntity.UserAccount.LastName;
            Email = delegateEntity.UserAccount.PrimaryEmail;
            CentreSpecificEmail = delegateEntity.UserCentreDetails?.Email ?? delegateEntity.UserAccount.PrimaryEmail;

            JobGroupId = jobGroups.Where(jg => jg.name == delegateEntity.UserAccount.JobGroupName).Select(jg => jg.id)
                .SingleOrDefault();

            Answer1 = delegateEntity.DelegateAccount.Answer1;
            Answer2 = delegateEntity.DelegateAccount.Answer2;
            Answer3 = delegateEntity.DelegateAccount.Answer3;
            Answer4 = delegateEntity.DelegateAccount.Answer4;
            Answer5 = delegateEntity.DelegateAccount.Answer5;
            Answer6 = delegateEntity.DelegateAccount.Answer6;

            ProfessionalRegistrationNumber = delegateEntity.UserAccount.ProfessionalRegistrationNumber;
            HasProfessionalRegistrationNumber =
                ProfessionalRegistrationNumberHelper.GetHasProfessionalRegistrationNumberForView(
                    delegateEntity.UserAccount.HasBeenPromptedForPrn,
                    delegateEntity.UserAccount.ProfessionalRegistrationNumber
                );
            IsSelfRegistrationOrEdit = false;
        }

        public EditDelegateFormData(EditDelegateFormData formData)
        {
            FirstName = formData.FirstName;
            LastName = formData.LastName;
            Email = formData.Email;
            CentreSpecificEmail = formData.CentreSpecificEmail;
            JobGroupId = formData.JobGroupId;
            Answer1 = formData.Answer1;
            Answer2 = formData.Answer2;
            Answer3 = formData.Answer3;
            Answer4 = formData.Answer4;
            Answer5 = formData.Answer5;
            Answer6 = formData.Answer6;
            ProfessionalRegistrationNumber = formData.ProfessionalRegistrationNumber;
            HasProfessionalRegistrationNumber = formData.HasProfessionalRegistrationNumber;
            IsSelfRegistrationOrEdit = false;
        }
    }
}
