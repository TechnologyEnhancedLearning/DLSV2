namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;

    public class RegistrationPromptAnswersViewModel : IValidatableObject
    {
        public RegistrationPromptAnswersViewModel() { }

        public RegistrationPromptAnswersViewModel(
            string optionsString,
            string? answer = null,
            bool includeAnswersTableCaption = false
        )
        {
            OptionsString = optionsString;
            Answer = answer;
            IncludeAnswersTableCaption = includeAnswersTableCaption;
        }

        public string? OptionsString { get; set; }

        [Required(ErrorMessage = "Enter a response")]
        [MaxLength(100, ErrorMessage = "Response must be 100 characters or fewer")]
        public string? Answer { get; set; }

        public bool IncludeAnswersTableCaption { get; set; }

        private IEnumerable<string> ComparableOptions => NewlineSeparatedStringListHelper
            .SplitNewlineSeparatedList(OptionsString)
            .Select(o => o.Trim().ToLower());

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (AnswerInOptionsString())
            {
                validationResults.Add(
                    new ValidationResult(
                        "That response is already in the list of options",
                        new[]
                        {
                            nameof(Answer),
                        }
                    )
                );
            }

            if (OptionsStringContainsDuplicates())
            {
                validationResults.Add(
                    new ValidationResult(
                        "The list of responses contains duplicate options",
                        new string[] { }
                    )
                );
            }

            return validationResults;
        }

        private bool AnswerInOptionsString()
        {
            return ComparableOptions.Contains(Answer?.Trim().ToLower());
        }

        public bool OptionsStringContainsDuplicates()
        {
            var optionsList = ComparableOptions.ToList();

            return optionsList.Distinct().Count() != optionsList.Count;
        }
    }
}
