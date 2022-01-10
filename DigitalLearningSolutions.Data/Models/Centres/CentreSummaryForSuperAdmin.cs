namespace DigitalLearningSolutions.Data.Models.Centres
{
    public class CentreSummaryForSuperAdmin
    {
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string? ContactForename { get; set; }
        public string? ContactSurname { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactTelephone { get; set; }
        public int CentreTypeId { get; set; }
        public string CentreType { get; set; }
        public bool Active { get; set; }
    }
}
