namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class CentreRegistrationPromptManagementViewModel : PromptManagementViewModel
    {
        public CentreRegistrationPromptManagementViewModel(
            int fieldId,
            string promptName,
            bool mandatory,
            List<string> options
        ) : base(fieldId, promptName, options)
        {
            Mandatory = mandatory;
        }

        public bool Mandatory { get; set; }
    }
}
