namespace DigitalLearningSolutions.Data.Models.Certificates
{
    using System;
    using DigitalLearningSolutions.Data.Models.Centres;

    public class CertificateInformation
    {
        public CertificateInformation(
           int progressID,
           string? delegateFirstName,
           string delegateLastName,
           string? contactForename,
           string? contactSurname,
           string centreName,
           int centreID,
           byte[]? signatureImage,
           int signatureWidth,
           int signatureHeight,
           byte[]? centreLogo,
           int logoWidth,
           int logoHeight,
           string? logoMimeType,
           string courseName,
           DateTime completionDate,
           int appGroupID,
           int createdByCentreID
       )
        {
            ProgressID = progressID;
            DelegateFirstName = delegateFirstName;
            DelegateLastName = delegateLastName;
            ContactForename = contactForename;
            ContactSurname = contactSurname;
            CentreName = centreName;
            CentreID = centreID;
            SignatureImage = signatureImage;
            SignatureWidth = signatureWidth;
            SignatureHeight = signatureHeight;
            CentreLogo = centreLogo;
            LogoWidth = logoWidth;
            LogoHeight = logoHeight;
            LogoMimeType = logoMimeType;
            CourseName = courseName;
            CompletionDate = completionDate;
            AppGroupID = appGroupID;
            CreatedByCentreID = createdByCentreID;
        }
        public int ProgressID { get; set; }
        public string? DelegateFirstName { get; set; }
        public string DelegateLastName { get; set; }
        public string? ContactForename { get; set; }
        public string? ContactSurname { get; set; }
        public string CentreName { get; set; }
        public int CentreID { get; set; }
        public byte[]? SignatureImage { get; set; }
        public int SignatureWidth { get; set; }
        public int SignatureHeight { get; set; }
        public byte[]? CentreLogo { get; set; }
        public int LogoWidth { get; set; }
        public int LogoHeight { get; set; }
        public string? LogoMimeType { get; set; }
        public string CourseName { get; set; }
        public DateTime CompletionDate { get; set; }
        public int AppGroupID { get; set; }
        public int CreatedByCentreID { get; set; }
    }
}
