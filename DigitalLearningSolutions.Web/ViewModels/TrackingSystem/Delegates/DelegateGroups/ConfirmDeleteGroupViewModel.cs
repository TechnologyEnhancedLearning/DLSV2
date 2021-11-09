namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ConfirmDeleteGroupViewModel : IValidatableObject
    {
        public string GroupLabel { get; set; }
        public int DelegateCount { get; set; }
        public int CourseCount { get; set; }
        public bool Confirm { get; set; }
        public bool DeleteEnrolments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Confirm == false)
            {
                yield return new ValidationResult(
                    "Confirm you wish to delete this group",
                    new[] { nameof(Confirm) }
                );
            }
        }
    }
}
