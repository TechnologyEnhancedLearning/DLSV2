namespace DigitalLearningSolutions.Data.Models.Centres
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
        public bool ShowOnMap { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string? OpeningHours { get; set; }
        public string? CentreWebAddress { get; set; }
        public string? OrganisationsCovered { get; set; }
        public string? TrainingVenues { get; set; }
        public string? OtherInformation { get; set; }
        public int CmsAdministratorSpots { get; set; }
        public int CmsManagerSpots { get; set; }
        public int CcLicenceSpots { get; set; }
        public int TrainerSpots { get; set; }
        public string? IpPrefix { get; set; }
        public string? ContractType { get; set; }
        public int CentreTypeId { get; set; }
        public string CentreType { get; set; }
        public int CustomCourses { get; set; }
        public long ServerSpaceUsed { get; set; }
        public long ServerSpaceBytes { get; set; }
        public bool Active { get; set; }
    }
}
