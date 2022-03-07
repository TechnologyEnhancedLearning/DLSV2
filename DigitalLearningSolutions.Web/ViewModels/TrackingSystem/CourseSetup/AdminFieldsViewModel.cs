namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class AdminFieldsViewModel
    {
        public AdminFieldsViewModel(IEnumerable<CourseAdminField> coursePrompts, int customisationId)
        {
            CustomFields = coursePrompts.Select(
                    cp =>
                        new CourseAdminFieldManagementViewModel(
                            customisationId,
                            cp.PromptNumber,
                            cp.PromptText,
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
