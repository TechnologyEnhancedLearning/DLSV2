namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddRegistrationPrompt
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;

    public class RegistrationPromptAnswersData
    {
        public RegistrationPromptAnswersData() { }

        public RegistrationPromptAnswersData(
            string? optionsString,
            string? answer = null,
            bool includeAnswersTableCaption = false
        )
        {
            OptionsString = optionsString;
            Answer = answer;
            IncludeAnswersTableCaption = includeAnswersTableCaption;
        }

        public string? OptionsString { get; set; }
        public string? Answer { get; set; }
        public bool IncludeAnswersTableCaption { get; set; }

        public bool OptionsStringContainsDuplicates()
        {
            var optionsList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(OptionsString)
                .Select(o => o.Trim().ToLower()).ToList();

            return optionsList.Distinct().Count() != optionsList.Count;
        }
    }
}
