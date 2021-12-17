namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class MyAccountEditDetailsViewModel : MyAccountEditDetailsFormData, IValidatableObject
    {
        public MyAccountEditDetailsViewModel(
            AdminUser? adminUser,
            DelegateUser? delegateUser,
            List<(int id, string name)> jobGroups,
            List<EditCustomFieldViewModel> editCustomFieldViewModels,
            DlsSubApplication dlsSubApplication
        ) : base(adminUser, delegateUser, jobGroups)
        {
            DlsSubApplication = dlsSubApplication;
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(
                jobGroups,
                delegateUser?.JobGroupName
            );
            CustomFields = editCustomFieldViewModels;
        }

        public MyAccountEditDetailsViewModel(
            MyAccountEditDetailsFormData formData,
            IReadOnlyCollection<(int id, string name)> jobGroups,
            List<EditCustomFieldViewModel> editCustomFieldViewModels,
            DlsSubApplication dlsSubApplication
        ) : base(formData)
        {
            DlsSubApplication = dlsSubApplication;
            var jobGroupName = jobGroups.Where(jg => jg.id == formData.JobGroupId).Select(jg => jg.name)
                .SingleOrDefault();
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(jobGroups, jobGroupName);
            CustomFields = editCustomFieldViewModels;
        }

        public DlsSubApplication DlsSubApplication { get; set; }

        public IEnumerable<SelectListItem> JobGroups { get; }

        public List<EditCustomFieldViewModel> CustomFields { get; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (!IsDelegateUser ||
                HasProfessionalRegistrationNumber == YesNoSelectionEnum.No)
            {
                ProfessionalRegistrationNumber = null;
                return validationResults;
            }

            if (HasProfessionalRegistrationNumber == YesNoSelectionEnum.None)
            {
                validationResults.Add(
                    new ValidationResult(
                        "Select an option",
                        new[] { nameof(HasProfessionalRegistrationNumber) }
                    )
                );

                return validationResults;
            }

            if (string.IsNullOrEmpty(ProfessionalRegistrationNumber))
            {
                validationResults.Add(
                    new ValidationResult(
                        "Enter professional registration number",
                        new[] { nameof(ProfessionalRegistrationNumber) }
                    )
                );

                return validationResults;
            }

            if (ProfessionalRegistrationNumber.Trim().Length < 5 ||
                ProfessionalRegistrationNumber.Trim().Length > 20)
            {
                validationResults.Add(
                    new ValidationResult(
                        "Professional registration number must be between 5 and 20 characters",
                        new[] { nameof(ProfessionalRegistrationNumber) }
                    )
                );
            }

            const string pattern = @"^[a-z\d-]+$";
            var rg = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!rg.Match(ProfessionalRegistrationNumber).Success)
            {
                validationResults.Add(
                    new ValidationResult(
                        "Invalid professional registration number format (only alphanumeric and hyphens allowed)",
                        new[] { nameof(ProfessionalRegistrationNumber) }
                    )
                );
            }

            return validationResults;
        }
    }
}
