namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    public class MyAccountViewModel
    {
        public MyAccountViewModel(
            string? centreName,
            string? userEmail,
            string? delegateId,
            string? firstName,
            string? surname)
        {
            Centre = centreName;
            User = userEmail;
            DelegateId = delegateId;
            FirstName = firstName;
            Surname = surname;
        }

        public string? Centre { get; set; }

        public string? User { get; set; }

        public string? DelegateId { get; set; }

        public string? FirstName { get; set; }

        public string? Surname { get; set; }
    }
}
