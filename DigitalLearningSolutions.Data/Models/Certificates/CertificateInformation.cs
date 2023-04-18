namespace DigitalLearningSolutions.Data.Models.Certificates
{
    using System;
    using DigitalLearningSolutions.Data.Models.Centres;

    public class CertificateInformation
    {
        public int ProgressID { get; set; }
        public string? DelegateFirstName { get; set; }
        public string? DelegateLastName { get; set; }
        public string? ContactForename { get; set; }
        public string? ContactSurname { get; set; }
        public string? CentreName { get; set; }
        public int CentreID { get; set; }
        public byte[]? SignatureImage { get; set; }
        public int SignatureWidth { get; set; }
        public int SignatureHeight { get; set; }
        public byte[]? CentreLogo { get; set; }
        public int LogoWidth { get; set; }
        public int LogoHeight { get; set; }
        public string? LogoMimeType { get; set; }
        public string? CourseName { get; set; }
        public DateTime CompletionDate { get; set; }
        public int AppGroupID { get; set; }
        public int CreatedByCentreID { get; set; }
    }
}
