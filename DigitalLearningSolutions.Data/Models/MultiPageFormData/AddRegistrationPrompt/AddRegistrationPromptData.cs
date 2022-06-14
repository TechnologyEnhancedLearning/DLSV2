namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddRegistrationPrompt
{
    public class AddRegistrationPromptData
    {
        public AddRegistrationPromptData()
        {
            SelectPromptData = new AddRegistrationPromptSelectPromptData();
            ConfigureAnswersData = new RegistrationPromptAnswersData();
        }

        public AddRegistrationPromptSelectPromptData SelectPromptData { get; set; }
        public RegistrationPromptAnswersData ConfigureAnswersData { get; set; }
    }
}
