namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class RegisterViewModel
    {
        public RegisterViewModel(int? centreId, string? centreName, string? inviteId)
        {
            CentreId = centreId;
            CentreName = centreName;
            InviteId = inviteId;
        }

        public int? CentreId { get; set; }
        public string? CentreName { get; set; }
        public string? InviteId { get; set; }
    }
}
