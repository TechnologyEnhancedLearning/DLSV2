namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol
{
    using DigitalLearningSolutions.Web.Helpers;
    using System;

    public class CompletedByDateViewModel
    {
        public int DelegateId { get; set; }
        public string DelegateName { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public OldDateValidator.ValidationResult? CompleteByValidationResult { get; set; }
    }
}
