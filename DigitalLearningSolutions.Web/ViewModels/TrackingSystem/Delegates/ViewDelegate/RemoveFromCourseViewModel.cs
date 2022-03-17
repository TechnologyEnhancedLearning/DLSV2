namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class RemoveFromCourseViewModel : IValidatableObject
    {
        public int DelegateId { get; set; }
        public string Name { get; set; }
        public int CustomisationId { get; set; }
        public string CourseName { get; set; }
        public bool Confirm { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (!Confirm)
            {
                validationResults.Add(
                    new ValidationResult(
                        "Confirm you wish to remove this delegate from this course",
                        new[]
                        {
                            nameof(Confirm)
                        }
                    )
                );
            }

            return validationResults;
        }
    }
}
