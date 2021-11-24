namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        [Test]
        public void GetNonArchivedObjectivesBySectionAndCustomisationId_returns_objectives_correctly()
        {
            using (new TransactionScope())
            {
                connection.Execute("UPDATE Tutorials SET OriginalTutorialID = 1 WHERE TutorialID = 1137");

                // When
                var result = tutorialContentDataService.GetNonArchivedObjectivesBySectionAndCustomisationId(248, 22062)
                    .ToList();

                // Then
                using (new AssertionScope())
                {
                    result.Count.Should().Be(4);
                    result.First().TutorialId.Should().Be(1);
                    result.First().Interactions.Should().BeEquivalentTo(new[] { 0, 1, 2, 3 });
                    result.First().Possible.Should().Be(4);
                    result.First().MyScore.Should().Be(0);

                    result.Last().TutorialId.Should().Be(1257);
                    result.Last().Interactions.Should().BeEquivalentTo(new[] { 8, 9, 10 });
                    result.Last().Possible.Should().Be(3);
                    result.Last().MyScore.Should().Be(0);
                }
            }
        }
    }
}
