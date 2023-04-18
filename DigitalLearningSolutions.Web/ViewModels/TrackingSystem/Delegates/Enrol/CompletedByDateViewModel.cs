namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol
{
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CompletedByDateViewModel : IValidatableObject
    {
        public CompletedByDateViewModel()
        {
        }

        public CompletedByDateViewModel(int delegateId, int delegateUserId, string delegateName, int? day, int? month, int? year)
        {
            Day = day;
            Month = month;
            Year = year;
            DelegateId = delegateId;
            DelegateUserId = delegateUserId;
            DelegateName = delegateName;
        }

        public int DelegateId { get; set; }
        public int DelegateUserId { get; set; }
        public string DelegateName { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "complete by date")
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }
    }
}
