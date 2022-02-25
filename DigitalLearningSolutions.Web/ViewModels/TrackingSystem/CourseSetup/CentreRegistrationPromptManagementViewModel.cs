namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class CentreRegistrationPromptManagementViewModel : CustomPromptManagementViewModel
    {
        public CentreRegistrationPromptManagementViewModel(
            int fieldId,
            string promptName,
            bool mandatory,
            List<string> options
        ) : base(fieldId, promptName, options)
        {
            PromptNumber = fieldId;
            PromptName = promptName;
            Mandatory = mandatory;
        }

        public bool Mandatory { get; set; }
    }
}
