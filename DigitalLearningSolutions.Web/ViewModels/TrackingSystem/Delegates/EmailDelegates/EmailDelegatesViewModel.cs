namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;

    public class EmailDelegatesViewModel : IValidatableObject
    {
        public EmailDelegatesViewModel() { }

        public EmailDelegatesViewModel(IEnumerable<DelegateUserCard> delegateUsers)
        {
            SetDelegates(delegateUsers);
            Day = DateTime.Today.Day;
            Month = DateTime.Today.Month;
            Year = DateTime.Today.Year;
        }

        public void SetDelegates(IEnumerable<DelegateUserCard> delegateUsers)
        {
            Delegates = delegateUsers.Select(delegateUser =>
                {
                    var preChecked = SelectedDelegateIds != null && SelectedDelegateIds.Contains(delegateUser.Id);
                    return new EmailDelegatesItemViewModel(delegateUser, preChecked);
                }
            );
        }

        public IEnumerable<EmailDelegatesItemViewModel>? Delegates { get; set; }

        [Required(ErrorMessage = "You must select at least one delegate")]
        public IEnumerable<int>? SelectedDelegateIds { get; set; }

        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "Email delivery date", true)
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }
    }
}
