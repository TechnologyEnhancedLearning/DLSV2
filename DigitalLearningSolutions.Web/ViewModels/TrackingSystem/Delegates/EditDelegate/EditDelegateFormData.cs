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

        public EditDelegateFormData(Delegate delegateUser, IEnumerable<(int id, string name)> jobGroups)
        {
            FirstName = delegateUser.UserAccount.FirstName;
            LastName = delegateUser.UserAccount.LastName;
            Email = delegateUser.UserAccount.PrimaryEmail;
            CentreEmail = delegateUser.UserCentreDetails?.Email ?? delegateUser.UserAccount.PrimaryEmail;

            JobGroupId = jobGroups.Where(jg => jg.name == delegateUser.UserAccount.JobGroupName).Select(jg => jg.id)
                .SingleOrDefault();

            Answer1 = delegateUser.DelegateAccount.Answer1;
            Answer2 = delegateUser.DelegateAccount.Answer2;
            Answer3 = delegateUser.DelegateAccount.Answer3;
            Answer4 = delegateUser.DelegateAccount.Answer4;
            Answer5 = delegateUser.DelegateAccount.Answer5;
            Answer6 = delegateUser.DelegateAccount.Answer6;

            ProfessionalRegistrationNumber = delegateUser.UserAccount.ProfessionalRegistrationNumber;
            HasProfessionalRegistrationNumber =
                ProfessionalRegistrationNumberHelper.GetHasProfessionalRegistrationNumberForView(
                    delegateUser.UserAccount.HasBeenPromptedForPrn,
                    delegateUser.UserAccount.ProfessionalRegistrationNumber
                );
            IsSelfRegistrationOrEdit = false;
        }

        public EditDelegateFormData(EditDelegateFormData formData)
        {
            FirstName = formData.FirstName;
            LastName = formData.LastName;
            Email = formData.Email;
            CentreEmail = formData.CentreEmail;
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
