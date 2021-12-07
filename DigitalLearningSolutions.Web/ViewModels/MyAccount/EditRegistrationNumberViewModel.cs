namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    public class EditProfessionalRegNumberViewModel
    {
        public string? OptionSelected { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
        public string? ProfessionalRegNumber { get; set; }

        public EditProfessionalRegNumberViewModel(bool hasBeenPromptedForPrn, string professionalRegNumber)
        {
            HasBeenPromptedForPrn = hasBeenPromptedForPrn;
            ProfessionalRegNumber = professionalRegNumber;
        }

        public EditProfessionalRegNumberViewModel()
        {
                
        }
    }
}
