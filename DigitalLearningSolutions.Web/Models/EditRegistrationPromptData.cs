namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;

    public class EditRegistrationPromptData
    {
        public EditRegistrationPromptData(EditRegistrationPromptViewModel model)
        {
            Id = Guid.NewGuid();
            EditModel = model;
        }

        public Guid Id { get; set; }

        public EditRegistrationPromptViewModel EditModel { get; set; }
    }
}
