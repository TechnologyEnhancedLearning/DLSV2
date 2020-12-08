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
        public void Get_section_content_should_return_null_on_invalid_input()
        {
            //When
            const int customisationId = 15853;
            const int progressId = 173218;
            const int sectionId = 0;
            var result = sectionContentService.GetSectionContent(customisationId, progressId, sectionId);

            // Then
            result.Should().BeNull();
        }
    }
}
