namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;

    public class AdminFieldAnswersViewModel
    {
        public AdminFieldAnswersViewModel() { }

        public AdminFieldAnswersViewModel(
            int customisationId,
            string optionsString,
            string? answer = null,
            bool includeAnswersTableCaption = false
        )
        {
            CustomisationId = customisationId;
            OptionsString = optionsString;
            Answer = answer;
            IncludeAnswersTableCaption = includeAnswersTableCaption;
        }

        public int CustomisationId { get; set; }

        public string? OptionsString { get; set; }

        public List<string> Options => NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(OptionsString);

        [Required(ErrorMessage = "Enter an answer")]
        [MaxLength(100, ErrorMessage = "Answer must be 100 characters or fewer")]
        public string? Answer { get; set; }

        public bool IncludeAnswersTableCaption { get; set; }
    }
}
