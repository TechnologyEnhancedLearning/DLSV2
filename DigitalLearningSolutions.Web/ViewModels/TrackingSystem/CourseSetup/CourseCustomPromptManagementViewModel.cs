namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class CourseCustomPromptManagementViewModel : CustomPromptManagementViewModel
    {
        public CourseCustomPromptManagementViewModel(
            int customisationId,
            int fieldId,
            string promptName,
            bool mandatory,
            List<string> options
        ) : base(fieldId, promptName, mandatory, options)
        {
            CustomisationId = customisationId;
            PromptNumber = fieldId;
        }

        public int CustomisationId { get; set; }

        public int PromptNumber { get; set; }
    }
}
