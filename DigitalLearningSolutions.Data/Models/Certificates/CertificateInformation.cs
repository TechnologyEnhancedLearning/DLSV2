namespace DigitalLearningSolutions.Data.Models.Certificates
{
    using System;
    using DigitalLearningSolutions.Data.Models.Centres;

    public class CertificateInformation
    {
        public CertificateInformation(
            Centre centreDetails,
            string? delegateFirstName,
            string delegateLastName,
            string courseName,
            DateTime completionDate,
            string certificateModifier
        )
        {
            SignatureImage = centreDetails.SignatureImage;
            CentreLogo = centreDetails.CentreLogo;
            ContactForename = centreDetails.ContactForename;
            ContactSurname = centreDetails.ContactSurname;
            CentreName = centreDetails.CentreName;

            DelegateFirstName = delegateFirstName;
            DelegateLastName = delegateLastName;
            CourseName = courseName;
            CompletionDate = completionDate;
            CertificateModifier = certificateModifier;
        }

        public string? DelegateFirstName { get; set; }
        public string DelegateLastName { get; set; }
        public string CourseName { get; set; }
        public byte[]? SignatureImage { get; set; }
        public byte[]? CentreLogo { get; set; }
        public string? ContactForename { get; set; }
        public string? ContactSurname { get; set; }
        public DateTime CompletionDate { get; set; }
        public string CentreName { get; set; }
        public string CertificateModifier { get; set; }
    }
}
