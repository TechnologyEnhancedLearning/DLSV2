namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;

    public class EditRegistrationPromptData
    {
        public EditRegistrationPromptData()
        {
            Id = Guid.NewGuid();
            EditModel = new EditRegistrationPromptViewModel();
        }

        public Guid Id { get; set; }

        public EditRegistrationPromptViewModel EditModel { get; set; }
    }
}
