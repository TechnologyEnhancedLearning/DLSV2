namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    public class MyAccountViewModel
    {
        public MyAccountViewModel(
            string? centreName,
            string? userEmail,
            string? delegateNumber,
            string? firstName,
            string? surname)
        {
            Centre = centreName;
            User = userEmail;
            DelegateNumber = delegateNumber;
            FirstName = firstName;
            Surname = surname;
        }

        public string? Centre { get; set; }

        public string? User { get; set; }

        public string? DelegateNumber { get; set; }

        public string? FirstName { get; set; }

        public string? Surname { get; set; }
    }
}
