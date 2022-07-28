namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class SetPasswordViewModel : ConfirmPasswordViewModel
    {
        public SetPasswordViewModel(
            ConfirmPasswordViewModel model,
            string label = "Password"
        )
        {
            Password = model.Password;
            ConfirmPassword = model.ConfirmPassword;
            Label = label;
        }

        public string Label { get; set; }
    }
}
