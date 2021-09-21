namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class ConfirmDeleteGroupViewModel : IValidatableObject
    {
        public readonly CheckboxListItemViewModel[] Checkboxes =
        {
            new CheckboxListItemViewModel(
                nameof(Confirm),
                "I am sure that I wish to delete this group and remove all delegates and courses from it.",
                ""
            ),
            new CheckboxListItemViewModel(
                nameof(DeleteEnrolments),
                "Remove all related enrolments where course has been started but is not yet complete",
                "Optionally all enrolments on courses that have been started but are incomplete and are associated with the group memebership can also be removed."
            )
        };

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
                    new[] { "Confirm" }
                );
            }
        }
    }
}
