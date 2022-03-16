namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts;

    public class AddRegistrationPromptData
    {
        public AddRegistrationPromptData()
        {
            Id = Guid.NewGuid();
            SelectPromptViewModel = new AddRegistrationPromptSelectPromptViewModel();
            ConfigureAnswersViewModel = new RegistrationPromptAnswersViewModel();
        }

        public AddRegistrationPromptData(IEnumerable<int> existingPromptIds)
        {
            Id = Guid.NewGuid();
            SelectPromptViewModel = new AddRegistrationPromptSelectPromptViewModel(existingPromptIds);
            ConfigureAnswersViewModel = new RegistrationPromptAnswersViewModel();
        }

        public Guid Id { get; set; }
        public AddRegistrationPromptSelectPromptViewModel SelectPromptViewModel { get; set; }
        public RegistrationPromptAnswersViewModel ConfigureAnswersViewModel { get; set; }
    }
}
