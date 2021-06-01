namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;

    public class RegistrationPromptAnswersViewModel
    {
        public RegistrationPromptAnswersViewModel() { }

        public RegistrationPromptAnswersViewModel(string optionsString, string? answer = null)
        {
            OptionsString = optionsString;
            Answer = answer;
        }

        public string? OptionsString { get; set; }

        public List<string> Options => NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(OptionsString);

        [Required(ErrorMessage = "Enter an answer.")]
        [MaxLength(100, ErrorMessage = "Answer must be at most 100 characters")]
        public string? Answer { get; set; }
    }
}
