namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration
{
    using DigitalLearningSolutions.Data.Models.Centres;

    public class CentreConfigurationViewModel
    {
        public CentreConfigurationViewModel(Centre centre)
        {
            CentreId = centre.CentreId;
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
            CentreTelephone = centre.CentreTelephone;
            CentreEmail = centre.CentreEmail;
            CentrePostcode = centre.CentrePostcode;
            ShowCentreOnMap = centre.ShowOnMap;
            OpeningHours = centre.OpeningHours;
            CentreWebAddress = centre.CentreWebAddress;
            OrganisationsCovered = centre.OrganisationsCovered;
            TrainingVenues = centre.TrainingVenues;
            OtherInformation = centre.OtherInformation;
        }

        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public string RegionName { get; set; }
        public string? NotifyEmail { get; set; }
        public string? BannerText { get; set; }
        public byte[]? SignatureImage { get; set; }
        public byte[]? CentreLogo { get; set; }
        public string? ContactForename { get; set; }
        public string? ContactSurname { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactTelephone { get; set; }
        public string? CentreTelephone { get; set; }
        public string? CentreEmail { get; set; }
        public string? CentrePostcode { get; set; }
        public bool ShowCentreOnMap { get; set; }
        public string? OpeningHours { get; set; }
        public string? CentreWebAddress { get; set; }
        public string? OrganisationsCovered { get; set; }
        public string? TrainingVenues { get; set; }
        public string? OtherInformation { get; set; }
    }
}
