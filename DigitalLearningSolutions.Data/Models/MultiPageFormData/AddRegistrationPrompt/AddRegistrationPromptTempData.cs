namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddRegistrationPrompt
{
    public class AddRegistrationPromptTempData
    {
        public AddRegistrationPromptTempData()
        {
            SelectPromptData = new AddRegistrationPromptSelectPromptData();
            ConfigureAnswersTempData = new RegistrationPromptAnswersTempData();
        }

        public AddRegistrationPromptSelectPromptData SelectPromptData { get; set; }
        public RegistrationPromptAnswersTempData ConfigureAnswersTempData { get; set; }
    }
}
