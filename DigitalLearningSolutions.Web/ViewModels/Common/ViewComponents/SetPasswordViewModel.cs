namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class SetPasswordViewModel : ConfirmPasswordViewModel
    {
        public SetPasswordViewModel(
            string label = "Password"
        )
        {
            Label = label;
        }

        public string Label { get; set; }
    }
}
