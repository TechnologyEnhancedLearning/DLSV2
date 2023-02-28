using DigitalLearningSolutions.Data.Extensions;
using DigitalLearningSolutions.Data.Models.Certificates;
using DigitalLearningSolutions.Data.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.ApiClients
{
    public interface ILearningHubReportApiClient
    {
        Task<PdfReportResponse?> PdfReport(CertificateInformation certificates, string strHtml, int delegateId);
        Task<PdfReportStatusResponse?> PdfReportStatus(PdfReportResponse pdfReportResponse);
        Task<byte[]> GetPdfReportFile(PdfReportResponse pdfReportResponse);
    }
    public class LearningHubReportApiClient : ILearningHubReportApiClient
    {
        private readonly HttpClient client;
        private readonly ILogger<ILearningHubReportApiClient> logger;
        private readonly string leaningHubReportApiClientId;
        public LearningHubReportApiClient(HttpClient httpClient, ILogger<ILearningHubReportApiClient> logger, IConfiguration config)
        {
            string learningHubReportApiBaseUrl = config.GetLearningHubReportApiBaseUrl();
            leaningHubReportApiClientId = config.GetLearningHubReportApiClientId();
            string leaningHubReportApiClientIdentityKey = config.GetLearningHubReportApiClientIdentityKey();
            client = httpClient;
            client.BaseAddress = new Uri(learningHubReportApiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("ClientIdentityKey", leaningHubReportApiClientIdentityKey);
            this.logger = logger;
        }
        public async Task<PdfReportResponse?> PdfReport(CertificateInformation certificates, string strHtml, int delegateId)
        {
            ReportData reportData = new ReportData();
            ReportCreateModel reportCreateModel = new ReportCreateModel();
            Reportpage reportPage = new Reportpage();
            Reportpage[] reportPages = new Reportpage[1];
            PdfReportResponse pdfReportResponse = new PdfReportResponse();
            reportCreateModel.name = certificates.CourseName;
            reportPages[0] = reportPage;
            reportPage.html = strHtml;
            reportCreateModel.reportPages = reportPages;
            reportData.clientId = leaningHubReportApiClientId;
            reportData.userId = delegateId;
            reportData.reportCreateModel = reportCreateModel;
            var json = JsonConvert.SerializeObject(reportData);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json-patch+json");
            var response = await client.PostAsync($"/api/PdfReport/", stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                pdfReportResponse = JsonConvert.DeserializeObject<PdfReportResponse>(result);
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized
                ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            return pdfReportResponse;
        }
        public async Task<PdfReportStatusResponse?> PdfReportStatus(PdfReportResponse pdfReportResponse)
        {
            PdfReportStatusResponse pdfReportStatusResponse = new PdfReportStatusResponse();
            var response = await client.GetAsync($"/api/PdfReport/PdfReportStatus/{pdfReportResponse.FileName}/{pdfReportResponse.Hash}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                pdfReportStatusResponse = JsonConvert.DeserializeObject<PdfReportStatusResponse>(result);
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized
               ||
               response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            return pdfReportStatusResponse;
        }
        public async Task<byte[]> GetPdfReportFile(PdfReportResponse pdfReportResponse)
        {
            PdfReportStatusResponse pdfReportStatusResponse = new PdfReportStatusResponse();
            var response = await client.GetAsync($"api/PdfReport/PdfReportFile/{pdfReportResponse.FileName}/{pdfReportResponse.Hash}").ConfigureAwait(false);
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
