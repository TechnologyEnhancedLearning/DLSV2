using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Models.SectionContent;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class SectionContentServiceTests
    {
        private SectionContentService sectionContentService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<SectionContentService>>();
            sectionContentService = new SectionContentService(connection, logger);
        }

        [Test]
        public void Get_section_content_should_return_section_content()
        {
            // When
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 382;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Erin Test 01",
                "Office 2013 Essentials for the Workplace",
                "Working with Microsoft Office applications",
                0,
                28,
                true,
                0
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_return_null_if_customisation_id_is_invalid()
        {
            //When
            const int customisationId = 0;
            const int candidateId = 1;
            const int sectionId = 382;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_null_if_section_id_is_invalid()
        {
            //When
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 0;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_correct_section_when_percent_complete_is_not_zero()
        {
            // When
            const int customisationId = 19262;
            const int candidateId = 1;
            const int sectionId = 1011;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Testing",
                "Excel 2013 for the Workplace",
                "Entering data",
                4,
                28,
                true,
                25
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_still_return_content_if_candidate_is_not_enrolled()
        {
            // When
            const int customisationId = 19262;
            const int candidateId = 0;
            const int sectionId = 1011;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Testing",
                "Excel 2013 for the Workplace",
                "Entering data",
                null,
                28,
                true,
                0
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }
    }
}
