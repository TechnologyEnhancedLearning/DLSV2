namespace DigitalLearningSolutions.Data.Models
{
    public class Centre
    {
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public int RegionId { get; set; }
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
        public string? OpeningHours { get; set; }
        public string? CentreWebAddress { get; set; }
        public string? OrganisationsCovered { get; set; }
        public string? TrainingVenues { get; set; }
        public string? OtherInformation { get; set; }
    }
}
