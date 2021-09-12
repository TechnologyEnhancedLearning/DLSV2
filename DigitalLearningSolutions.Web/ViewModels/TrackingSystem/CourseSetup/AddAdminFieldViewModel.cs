namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AddAdminFieldViewModel : AdminFieldAnswersViewModel
    {
        public AddAdminFieldViewModel()
        {
            IncludeAnswersTableCaption = true;
        }

        public AddAdminFieldViewModel(int customisationId)
        {
            CustomisationId = customisationId;
            IncludeAnswersTableCaption = true;
        }

        public AddAdminFieldViewModel(
            int customisationId,
            int customPromptId,
            string? options,
            string? answer = null
        )
        {
            CustomisationId = customisationId;
            CustomPromptId = customPromptId;
            OptionsString = options;
            IncludeAnswersTableCaption = true;
            Answer = answer;
        }

        [Required(ErrorMessage = "Select a prompt name")]
        public int? CustomPromptId { get; set; }

        public IEnumerable<SelectListItem>? CoursePromptNameOptions { get; set; }
    }
}
