namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem
{
    using DigitalLearningSolutions.Data.Models;

    public class CentreConfigurationViewModel
    {
        public CentreConfigurationViewModel(Centre centre)
        {
            CentreName = centre.CentreName;
            RegionName = centre.RegionName;
            NotifyEmail = centre.NotifyEmail;
            BannerText = centre.BannerText;
            SignatureImage = centre.SignatureImage;
            CentreLogo = centre.CentreLogo;
            ContactForename = centre.ContactForename;
            ContactSurname = centre.ContactSurname;
            ContactEmail = centre.ContactEmail;
            ContactTelephone = centre.ContactTelephone;
        }

        public string CentreName { get; set; }
        public string RegionName { get; set; }
        public string NotifyEmail { get; set; }
        public string BannerText { get; set; }
        public byte[]? SignatureImage { get; set; }
        public byte[]? CentreLogo { get; set; }
        public string? ContactForename { get; set; }
        public string? ContactSurname { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactTelephone { get; set; }
    }
}
