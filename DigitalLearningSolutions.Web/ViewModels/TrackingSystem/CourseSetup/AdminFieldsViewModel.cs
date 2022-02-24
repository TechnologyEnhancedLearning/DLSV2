namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class AdminFieldsViewModel
    {
        public AdminFieldsViewModel(IEnumerable<CoursePrompt> coursePrompts, int customisationId)
        {
            CustomFields = coursePrompts.Select(
                    cp =>
                        new CourseAdminFieldManagementViewModel(
                            customisationId,
                            cp.CoursePromptNumber,
                            cp.CustomPromptText,
                            cp.Mandatory,
                            cp.Options
                        )
                )
                .ToList();

            CustomisationId = customisationId;
        }

        public int CustomisationId { get; set; }

        public List<CourseAdminFieldManagementViewModel> CustomFields { get; set; }
    }
}
