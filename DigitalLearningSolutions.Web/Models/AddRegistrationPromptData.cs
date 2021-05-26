namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;

    public class AddRegistrationPromptData
    {
        public AddRegistrationPromptData()
        {
            Id = Guid.NewGuid();
            SelectPromptViewModel = new AddRegistrationPromptSelectPromptViewModel();
            ConfigureAnswersViewModel = new AddRegistrationPromptConfigureAnswersViewModel();
        }

        public Guid Id { get; set; }
        public AddRegistrationPromptSelectPromptViewModel SelectPromptViewModel { get; set; }
        public AddRegistrationPromptConfigureAnswersViewModel ConfigureAnswersViewModel { get; set; }
    }
}
