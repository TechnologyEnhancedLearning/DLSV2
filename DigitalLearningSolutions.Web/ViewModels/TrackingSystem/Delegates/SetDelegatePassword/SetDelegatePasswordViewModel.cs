namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.SetDelegatePassword
{
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class SetDelegatePasswordViewModel : PasswordViewModel
    {
        public SetDelegatePasswordViewModel() { }

        public SetDelegatePasswordViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
