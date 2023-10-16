namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Delegates
{
    public class ConfirmationViewModel
    {
        public int DelegateId { get; set; }

        public string DisplayName { get; set; }

        public bool IsChecked { get; set; }

        public bool Error { get; set; }

        public string SearchString { get; set; }

        public string ExistingFilterString { get; set; }

        public int Page { get; set; }
    }
}
