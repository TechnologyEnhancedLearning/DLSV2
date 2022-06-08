namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System;
    using DigitalLearningSolutions.Data.Models.Certificates;
    using DigitalLearningSolutions.Web.Helpers;

    public class CertificateViewModel
    {
        public CertificateViewModel(CertificateInformation certificateInformation)
        {
            CentreContactName = certificateInformation.ContactSurname != null
                ? DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                    certificateInformation.ContactForename,
                    certificateInformation.ContactSurname
                )
                : null;

            DelegateName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                certificateInformation.DelegateFirstName,
                certificateInformation.DelegateLastName
            );

            CourseName = certificateInformation.CourseName;

            SignatureImage = certificateInformation.SignatureImage;
            CentreLogo = certificateInformation.CentreLogo;
            CompletionDate = certificateInformation.CompletionDate;
            CentreName = certificateInformation.CentreName;
            CertificateModifier = certificateInformation.CertificateModifier;
        }

        public string DelegateName { get; set; }
        public string CourseName { get; set; }
        public string? CentreContactName { get; set; }
        public byte[]? SignatureImage { get; set; }
        public byte[]? CentreLogo { get; set; }
        public DateTime CompletionDate { get; set; }
        public string CentreName { get; set; }
        public string CertificateModifier { get; set; }
    }
}
