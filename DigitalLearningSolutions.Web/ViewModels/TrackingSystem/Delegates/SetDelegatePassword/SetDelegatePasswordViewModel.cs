namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.SetDelegatePassword
{
    using System.ComponentModel.DataAnnotations;

    public class SetDelegatePasswordViewModel
    {
        public SetDelegatePasswordViewModel() { }

        public SetDelegatePasswordViewModel(string name, int delegateId, bool isFromViewDelegatePage = false)
        {
            Name = name;
            DelegateId = delegateId;
            IsFromViewDelegatePage = isFromViewDelegatePage;
        }

        public string Name { get; set; }

        public int DelegateId { get; set; }

        public bool IsFromViewDelegatePage { get; set; }

        [Required(ErrorMessage = "Enter a password")]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or more")]
        [MaxLength(100, ErrorMessage = "Password must be 100 characters or fewer")]
        [RegularExpression(
            @"(?=.*?[^\w\s])(?=.*?[0-9])(?=.*?[A-Za-z]).*",
            ErrorMessage = "Password must contain at least 1 letter, 1 number and 1 symbol"
        )]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
