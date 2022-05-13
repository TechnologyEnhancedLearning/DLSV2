namespace DigitalLearningSolutions.Data.Models.User
{
    public class CentreUserDetails
    {
        public CentreUserDetails(DelegateAccount delegateAtCentre, AdminAccount? adminAtCentre)
        {
            CentreId = delegateAtCentre.CentreId;
            CentreName = delegateAtCentre.CentreName;
            IsDelegate = true;
            IsApproved = delegateAtCentre.Approved;
            IsAdmin = adminAtCentre != null;
            AdminIsActive = adminAtCentre?.Active;
            DelegateIsActive = delegateAtCentre.Active; 
        }

        public CentreUserDetails(AdminAccount adminAccount)
        {
            CentreId = adminAccount.CentreId;
            CentreName = adminAccount.CentreName;
            IsAdmin = true;
            IsDelegate = false;
            IsApproved = null;
            AdminIsActive = adminAccount.Active; 
            DelegateIsActive = null;
        }

        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDelegate { get; set; }
        public bool? DelegateIsActive { get; set; }
        public bool? AdminIsActive { get; set; }
        public bool? IsApproved { get; set; }
    }
}
