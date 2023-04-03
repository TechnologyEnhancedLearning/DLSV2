using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Helpers;
using FluentMigrator.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    public class AddMultipleSupervisorDelegatesViewModel
    {
        public AddMultipleSupervisorDelegatesViewModel() { }
        public AddMultipleSupervisorDelegatesViewModel(string? delegateEmails)
        {
            DelegateEmails = delegateEmails;
        }

        [Required(ErrorMessage = "Enter an email")]
        [RegularExpression(CommonValidationErrorMessages.EmailsRegexWithNewLineSeparator, ErrorMessage = CommonValidationErrorMessages.InvalidMultiLineEmail)]
        public string? DelegateEmails { get; set; }
    }
}
