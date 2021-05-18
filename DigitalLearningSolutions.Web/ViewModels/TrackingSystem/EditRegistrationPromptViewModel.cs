namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class EditRegistrationPromptViewModel
    {
        public EditRegistrationPromptViewModel(CustomPrompt customPrompt)
        {
            PromptNumber = customPrompt.CustomPromptNumber;
            Prompt = customPrompt.CustomPromptText;
            Mandatory = customPrompt.Mandatory;
            Options = customPrompt.Options;
        }

        public int PromptNumber { get; set; }

        public string Prompt { get; set; }

        public bool Mandatory { get; set; }

        public List<string> Options { get; set; }

        [Required(ErrorMessage = "Enter an answer.")]
        [MaxLength(100, ErrorMessage = "Answer must be at most 100 characters")]
        public string? Answer { get; set; }
    }
}
