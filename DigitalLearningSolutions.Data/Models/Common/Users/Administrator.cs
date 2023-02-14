namespace DigitalLearningSolutions.Data.Models.Common.Users
{
    public class Administrator
    {
        public int AdminID {get;set;}
        public int CentreID { get; set; }
        public string? Email { get; set; }
        public string? Forename { get; set; }
        public string? Surname { get; set; }
        public bool Active { get; set; }
        public bool IsFrameworkDeveloper { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string? CentreName { get; set; }
    }
}
