namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    public class MyAccountViewModel
    {
        public MyAccountViewModel(string? centreName, string? userEmail, string? delegateId)
        {
            Centre = centreName;
            User = userEmail;
            DelegateId = delegateId;
        }

        public string? Centre { get; set; }

        public string? User { get; set; }

        public string? DelegateId { get; set; }
    }
}
