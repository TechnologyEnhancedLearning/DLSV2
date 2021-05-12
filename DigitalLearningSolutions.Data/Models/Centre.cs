namespace DigitalLearningSolutions.Data.Models
{
    public class Centre
    {
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string NotifyEmail { get; set; }
        public string BannerText { get; set; }
        public byte[]? SignatureImage { get; set; }
        public byte[]? CentreLogo { get; set; }
    }
}
