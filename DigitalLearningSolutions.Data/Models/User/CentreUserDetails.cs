namespace DigitalLearningSolutions.Data.Models.User
{
    public class CentreUserDetails
    {
        public CentreUserDetails(int centreId, string centreName, bool isAdmin = false, bool isDelegate = false)
        {
            CentreId = centreId;
            CentreName = centreName;
            IsAdmin = isAdmin;
            IsDelegate = isDelegate;
        }

        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDelegate { get; set; }
    }
}
