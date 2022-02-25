namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class CourseAdminFieldManagementViewModel : PromptManagementViewModel
    {
        public CourseAdminFieldManagementViewModel(
            int customisationId,
            int promptNumber,
            string promptName,
            List<string> options
        ) : base(promptNumber, promptName, options)
        {
            CustomisationId = customisationId;
            PromptNumber = promptNumber;
        }

        public int CustomisationId { get; set; }
    }
}
