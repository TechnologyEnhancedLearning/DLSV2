namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class EditDelegateFormData : EditDetailsFormData, IValidatableObject
    {
        public EditDelegateFormData() {}

        public EditDelegateFormData(DelegateUser delegateUser, IEnumerable<(int id, string name)> jobGroups)
        {
            FirstName = delegateUser.FirstName;
            LastName = delegateUser.LastName;
            Email = delegateUser.EmailAddress;

            JobGroupId = jobGroups.Where(jg => jg.name == delegateUser.JobGroupName).Select(jg => jg.id)
                .SingleOrDefault();

            Answer1 = delegateUser.Answer1;
            Answer2 = delegateUser.Answer2;
            Answer3 = delegateUser.Answer3;
            Answer4 = delegateUser.Answer4;
            Answer5 = delegateUser.Answer5;
            Answer6 = delegateUser.Answer6;

            AliasId = delegateUser.AliasId;
        }

        public EditDelegateFormData(EditDelegateFormData formData)
        {
            FirstName = formData.FirstName;
            LastName = formData.LastName;
            Email = formData.Email;
            JobGroupId = formData.JobGroupId;
            Answer1 = formData.Answer1;
            Answer2 = formData.Answer2;
            Answer3 = formData.Answer3;
            Answer4 = formData.Answer4;
            Answer5 = formData.Answer5;
            Answer6 = formData.Answer6;
            AliasId = formData.AliasId;
        }

        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongLastName)]
        public string? AliasId { get; set; }
    }
}
