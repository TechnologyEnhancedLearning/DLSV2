namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models.Centres;

    public class CentreSummaryViewModel
    {
        public CentreSummaryViewModel(CentreSummaryForSuperAdmin model)
        {
            CentreId = model.CentreId;
            CentreName = model.CentreName;
            RegionName = model.RegionName;
            ContactForename = model.ContactForename;
            ContactSurname = model.ContactSurname;
            ContactEmail = model.ContactEmail;
            ContactTelephone = model.ContactTelephone;
            CentreType = model.CentreType;
            Active = model.Active;
        }

        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public string RegionName { get; set; }
        public string? ContactForename { get; set; }
        public string? ContactSurname { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactTelephone { get; set; }
        public string CentreType { get; set; }
        public bool Active { get; set; }
    }
}
