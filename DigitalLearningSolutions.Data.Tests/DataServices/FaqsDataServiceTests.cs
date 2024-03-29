﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class FaqsDataServiceTests
    {
        private SqlConnection connection = null!;
        private FaqsDataService faqsDataService = null!;
        private readonly Faq expectedFaq = new Faq
        {
            FaqId = 112,
            TargetGroup = 0,
            Published = true,
            AHtml =
                "No, existing learners will access the Learning Portal using their existing <strong>Delegate ID</strong>.&nbsp;",
            QText = "Do our existing learners need to register to use the Learning Portal? ",
            QAnchor = "LearningPortalRegister",
            Weighting = 20,
            CreatedDate = new DateTime(2017, 5, 9),
        };

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            faqsDataService = new FaqsDataService(connection);
        }

        [Test]
        public void GetFaqById_returns_expected_faq()
        {
            // Given
            const int faqId = 112;


            // When
            var result = faqsDataService.GetFaqById(faqId);

            // Then
            result.Should().BeEquivalentTo(expectedFaq);
        }

        [Test]
        public void GetFaqById_returns_null_when_faq_does_not_exist()
        {
            // Given
            const int faqId = -9999;

            // When
            var result = faqsDataService.GetFaqById(faqId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetAllFaqs_returns_expected_information()
        {
            // When
            var result = faqsDataService.GetAllFaqs();

            // Then
            var resultList = result.ToList();
            resultList.Count().Should().Be(121);
            var returnedFaq = resultList.FirstOrDefault(x => x.FaqId == 112);
            returnedFaq.Should().BeEquivalentTo(expectedFaq);
        }
    }
}
