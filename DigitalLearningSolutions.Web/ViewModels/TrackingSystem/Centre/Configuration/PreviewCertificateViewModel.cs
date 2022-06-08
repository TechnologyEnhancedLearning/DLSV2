namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration
{
    using DigitalLearningSolutions.Data.Models.Certificates;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class PreviewCertificateViewModel : CertificateViewModel
    {
        public PreviewCertificateViewModel(
            CertificateInformation certificateInformation,
            int centreId
        ) : base(certificateInformation)
        {
            CentreId = centreId;
        }

        public int CentreId { get; set; }
    }
}
