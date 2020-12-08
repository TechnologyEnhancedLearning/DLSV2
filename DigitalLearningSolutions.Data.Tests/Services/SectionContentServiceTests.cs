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
            const int progressId = 173218;
            const int sectionId = 382;
            var result = sectionContentService.GetSectionContent(customisationId, progressId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
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
            const int progressId = 173218;
            const int sectionId = 382;
            var result = sectionContentService.GetSectionContent(customisationId, progressId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_null_if_progress_id_is_invalid()
        {
            //When
            const int customisationId = 15853;
            const int progressId = 0;
            const int sectionId = 382;
            var result = sectionContentService.GetSectionContent(customisationId, progressId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_null_if_section_id_is_invalid()
        {
            //When
            const int customisationId = 15853;
            const int progressId = 173218;
            const int sectionId = 0;
            var result = sectionContentService.GetSectionContent(customisationId, progressId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_correct_section_when_percent_complete_is_not_zero()
        {
            // When
            const int customisationId = 19262;
            const int progressId = 201058;
            const int sectionId = 1011;
            var result = sectionContentService.GetSectionContent(customisationId, progressId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Entering data",
                4,
                28,
                true,
                25
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }
    }
}
