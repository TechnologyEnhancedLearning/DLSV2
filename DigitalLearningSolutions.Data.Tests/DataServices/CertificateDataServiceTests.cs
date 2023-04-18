namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CertificateDataServiceTests
    {
        private CertificateDataService certificateDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<CertificateDataService>>();
            certificateDataService = new CertificateDataService(connection, logger);
        }
        [Test]
        public void GetCertificates_should_return_null_when_the_centre_does_not_exist()
        {
            // When
            var result = certificateDataService.GetCertificateDetailsById(0);

            // Then
            result.Should().BeNull();
        }
        [Test]
        public void GetCertificates_should_return_notnull_when_the_centre_does_not_exist()
        {
            // When
            var result = certificateDataService.GetCertificateDetailsById(10);

            // Then
            result.Should().NotBeNull();
        }
        [Test]
        public void GetPreviewCertificates_should_return_null_when_the_centre_does_not_exist()
        {
            // When
            var result = certificateDataService.GetPreviewCertificateForCentre(0);

            // Then
            result.Should().BeNull();
        }
        [Test]
        public void GetPreviewCertificates_should_return_notnull_when_the_centre_does_not_exist()
        {
            // When
            var result = certificateDataService.GetCertificateDetailsById(3);

            // Then
            result.Should().NotBeNull();
        }
        [Test]
        public void GetCertificates_should_contain_an_active_centre()
        {
            // When
            var result = certificateDataService.GetCertificateDetailsById(3);

            // Then
            result.CentreName.Should().NotBeNull();
        }
        [Test]
        public void GetCertificates_should_contain_an_course_name()
        {
            // When
            var result = certificateDataService.GetCertificateDetailsById(3);

            // Then
            result.CourseName.Should().NotBeNull();
        }
    }
}
