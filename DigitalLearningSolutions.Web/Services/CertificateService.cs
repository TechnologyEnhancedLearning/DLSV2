namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Certificates;

    public interface ICertificateService
    {
        CertificateInformation? GetPreviewCertificateForCentre(int centreId);
    }

    public class CertificateService : ICertificateService
    {
        private readonly ICentresDataService centresDataService;

        public CertificateService(ICentresDataService centresDataService)
        {
            this.centresDataService = centresDataService;
        }

        public CertificateInformation? GetPreviewCertificateForCentre(int centreId)
        {
            var centreDetails = centresDataService.GetCentreDetailsById(centreId);

            if (centreDetails == null)
            {
                return null;
            }

            return new CertificateInformation(
                centreDetails,
                "Joseph",
                "Bloggs",
                "Level 2 - ITSP Course Name",
                new DateTime(2014, 04, 01),
                "Passing online Digital Learning Solutions post learning assessments"
            );
        }
    }
}
