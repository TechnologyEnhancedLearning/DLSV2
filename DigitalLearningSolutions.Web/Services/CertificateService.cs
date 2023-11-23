namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Certificates;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public interface ICertificateService
    {
        CertificateInformation? GetPreviewCertificateForCentre(int centreId);
        CertificateInformation? GetCertificateDetailsById(int progressId);
        Task<PdfReportResponse?> PdfReport(CertificateInformation certificates, string strHTML, int delegateId);
        Task<PdfReportStatusResponse?> PdfReportStatus(PdfReportResponse pdfReportResponse);
        Task<byte[]> GetPdfReportFile(PdfReportResponse pdfReportResponse);
    }

    public class CertificateService : ICertificateService
    {
        private readonly ICertificateDataService certificateDataService;
        private readonly ILearningHubReportApiClient learningHubReportApiClient;
        private readonly ILogger<ILearningHubReportApiClient> logger;
        public CertificateService(
            ICertificateDataService certificateDataService,
            ILearningHubReportApiClient learningHubReportApiClient,
            ILogger<ILearningHubReportApiClient> logger
            )
        {
            this.certificateDataService = certificateDataService;
            this.learningHubReportApiClient = learningHubReportApiClient;
            this.logger = logger;
        }
        public CertificateInformation? GetPreviewCertificateForCentre(int centreId)
        {
            return certificateDataService.GetPreviewCertificateForCentre(centreId);
        }
        public CertificateInformation? GetCertificateDetailsById(int progressId)
        {
            return certificateDataService.GetCertificateDetailsById(progressId);
        }
        public Task<PdfReportResponse?> PdfReport(CertificateInformation certificates, string strHtml, int delegateId)
        {
            return learningHubReportApiClient.PdfReport(certificates, strHtml, delegateId);
        }
        public Task<PdfReportStatusResponse?> PdfReportStatus(PdfReportResponse pdfReportResponse)
        {
            return learningHubReportApiClient.PdfReportStatus(pdfReportResponse);
        }
        public Task<byte[]> GetPdfReportFile(PdfReportResponse pdfReportResponse)
        {
            return learningHubReportApiClient.GetPdfReportFile(pdfReportResponse);
        }
    }
}
