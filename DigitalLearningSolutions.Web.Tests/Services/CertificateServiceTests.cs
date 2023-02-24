namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using Castle.Core.Logging;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Certificates;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CertificateServiceTests
    {
        private ICentresDataService centresDataService = null!;
        private ICertificateService certificateService = null!;
        private ICertificateDataService certificatedataService = null!;
        private ILearningHubReportApiClient learningHubReportApiClient = null!;
        private ILogger<ILearningHubReportApiClient> logger = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            certificateService = new CertificateService(
                certificatedataService, learningHubReportApiClient, logger);
        }

        [Test]
        public void GetPreviewCertificateForCentre_returns_null_when_data_service_returns_null()
        {
            // Given
            const int centreId = 2;
            A.CallTo(() => centresDataService.GetCentreDetailsById(centreId)).Returns(null);

            // When
            var result = certificateService.GetPreviewCertificateForCentre(centreId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void
            GetPreviewCertificateForCentre_returns_expected_certificate_information_when_data_service_returns_centre()
        {
            // Given
            var centre = CentreTestHelper.GetDefaultCentre();
            A.CallTo(() => centresDataService.GetCentreDetailsById(centre.CentreId)).Returns(centre);

            // When
            var result = certificateService.GetPreviewCertificateForCentre(centre.CentreId);

            // Then
            var expectedCertificateInformation = new CertificateInformation(
                0,
                "Joseph",
                "Bloggs",
                centre.ContactForename,
                centre.ContactSurname,
                centre.CentreName,
                centre.CentreId,
                centre.SignatureImage,
                250,
                250,
                centre.CentreLogo,
                250,
                250,
                "",
                "Level 2 - ITSP Course Name",
                new DateTime(2014, 4, 1),

                3,
                101
            );
            result.Should().BeEquivalentTo(expectedCertificateInformation);
        }
    }
}

