namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        [Test]
        public void GetObjectivesBySectionId_returns_objectives_correctly()
        {
            // When
            var result = tutorialContentDataService.GetObjectivesBySectionId(SectionId, CustomisationId).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(4);
                result.First().TutorialId.Should().Be(49);
                result.First().Interactions.Should().BeEquivalentTo(new []{1,2,3,4});
                result.First().Possible.Should().Be(255);
                result.First().MyScore.Should().Be(120);
            }
        }
    }
}
