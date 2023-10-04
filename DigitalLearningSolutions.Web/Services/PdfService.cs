namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public interface IPdfService
    {
        Task<PdfReportResponse?> PdfReport(string reportName, string strHTML, int userId);
        Task<PdfReportStatusResponse?> PdfReportStatus(PdfReportResponse pdfReportResponse);
        Task<byte[]> GetPdfReportFile(PdfReportResponse pdfReportResponse);
    }

    public class PdfService : IPdfService
    {
        private readonly ILearningHubReportApiClient learningHubReportApiClient;
        private readonly ILogger<ILearningHubReportApiClient> logger;
        public PdfService(
            ILearningHubReportApiClient learningHubReportApiClient,
            ILogger<ILearningHubReportApiClient> logger
            )
        {
            this.learningHubReportApiClient = learningHubReportApiClient;
            this.logger = logger;
        }
        public Task<PdfReportResponse?> PdfReport(string reportName, string strHtml, int userId)
        {
            return learningHubReportApiClient.PdfReport(reportName, strHtml, userId);
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
