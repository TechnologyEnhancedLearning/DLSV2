namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    public class NotificationPreferencesViewModel
    {
        public NotificationPreferencesViewModel(int? adminId, int? delegateId)
        {
            AdminId = adminId;
            DelegateId = delegateId;
        }

        public int? AdminId { get; set; }

        public int? DelegateId { get; set; }
    }
}
