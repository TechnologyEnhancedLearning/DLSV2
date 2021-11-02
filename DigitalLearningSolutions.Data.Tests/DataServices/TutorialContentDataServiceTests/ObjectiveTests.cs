namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
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
            var result = tutorialContentDataService.GetObjectivesBySectionId(248, 22062).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(4);
                result.First().TutorialId.Should().Be(1137);
                result.First().Interactions.Should().BeEquivalentTo(new[] { 0, 1, 2, 3 });
                result.First().Possible.Should().Be(4);
                result.First().MyScore.Should().Be(0);
            }
        }
    }
}
