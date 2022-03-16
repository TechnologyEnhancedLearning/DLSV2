namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class AddRegistrationPromptSelectPromptViewModel : IValidatableObject
    {
        public AddRegistrationPromptSelectPromptViewModel()
        {
            ExistingPromptIds = new List<int>();
        }

        public AddRegistrationPromptSelectPromptViewModel(IEnumerable<int> existingPromptIds)
        {
            ExistingPromptIds = existingPromptIds;
        }

        public AddRegistrationPromptSelectPromptViewModel(int customPromptId, bool mandatory)
        {
            CustomPromptId = customPromptId;
            Mandatory = mandatory;
            ExistingPromptIds = new List<int>();
        }

        [Required(ErrorMessage = "Select a prompt name")]
        public int? CustomPromptId { get; set; }

        public bool Mandatory { get; set; }

        public IEnumerable<int> ExistingPromptIds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (CustomPromptId.HasValue && ExistingPromptIds.Contains(CustomPromptId.Value))
            {
                validationResults.Add(
                    new ValidationResult(
                        "That custom prompt already exists at this centre",
                        new[]
                        {
                            nameof(CustomPromptId),
                        }
                    )
                );
            }

            return validationResults;
        }
    }
}
