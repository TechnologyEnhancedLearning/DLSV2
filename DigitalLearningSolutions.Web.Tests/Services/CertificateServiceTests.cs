namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using Castle.Core.Logging;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Certificates;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Services;
    using DocumentFormat.OpenXml.Wordprocessing;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CertificateServiceTests
    {
        private ICentresDataService centresDataService = null!;
        private ICertificateService certificateService = null!;
        private ICertificateDataService certificateDataService = null!;
        private ILearningHubReportApiClient learningHubReportApiClient = null!;
        private ILogger<ILearningHubReportApiClient> logger = null!;
        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            certificateDataService = A.Fake<ICertificateDataService>();
            learningHubReportApiClient = A.Fake<ILearningHubReportApiClient>();
            logger = A.Fake<ILogger<ILearningHubReportApiClient>>();
            certificateService = new CertificateService(certificateDataService, learningHubReportApiClient, logger);
        }

        [Test]
        public void GetPreviewCertificateForCentre_returns_null_when_data_service_returns_null()
        {
            // Given
            const int centreId = 0;
            A.CallTo(() => centresDataService.GetCentreDetailsById(centreId)).Returns(null);

            // When
            var result = certificateDataService.GetPreviewCertificateForCentre(centreId);

            // Then
            result.Should().NotBeNull();
        }

        [Test]
        public void
            GetPreviewCertificateForCentre_returns_expected_certificate_information_when_data_service_returns_centre()
        {
            // Given
            var centre = CentreTestHelper.GetDefaultCentre();
            var certificateInfo = CertificateTestHelper.GetDefaultCertificate();
            A.CallTo(() => centresDataService.GetCentreDetailsById(centre.CentreId)).Returns(centre);
            A.CallTo(() => certificateDataService.GetPreviewCertificateForCentre(centre.CentreId)).Returns(certificateInfo);

            // When
            var result = certificateService.GetPreviewCertificateForCentre(centre.CentreId);

            result.Should().BeEquivalentTo(certificateInfo);
        }
    }
}

