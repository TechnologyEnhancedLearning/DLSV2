﻿namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
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

        [Test]
        public void GetNonArchivedCcObjectivesBySectionAndCustomisationId_returns_objectives_correctly()
        {
            using (new TransactionScope())
            {
                // Given
                connection.Execute("UPDATE Tutorials SET OriginalTutorialID = 1 WHERE TutorialID = 1137");
                connection.Execute("UPDATE CustomisationTutorials SET DiagStatus = 0 WHERE CusTutID = 704039");

                // When
                var result = tutorialContentDataService
                    .GetNonArchivedCcObjectivesBySectionAndCustomisationId(248, 22062, false)
                    .ToList();

                // Then
                using (new AssertionScope())
                {
                    result.Count.Should().Be(3);
                    result.First().TutorialId.Should().Be(1);
                    result.First().TutorialName.Should().Be("Manufacturers, operating systems and applications");
                    result.First().Possible.Should().Be(4);
                    result.First().WrongCount.Should().Be(0);
                    result.First().MyScore.Should().Be(0);

                    result.Last().TutorialId.Should().Be(1257);
                    result.Last().TutorialName.Should().Be("Sharing information");
                    result.Last().Possible.Should().Be(3);
                    result.Last().WrongCount.Should().Be(0);
                    result.Last().MyScore.Should().Be(0);

                    result.Select(x => x.TutorialId).Should().NotContain(1138);
                }
            }
        }

        [Test]
        public void
            GetNonArchivedCcObjectivesBySectionAndCustomisationId_returns_objectives_correctly_with_postlearning()
        {
            using (new TransactionScope())
            {
                // Given
                connection.Execute("UPDATE Tutorials SET OriginalTutorialID = 1 WHERE TutorialID = 1137");
                connection.Execute("UPDATE CustomisationTutorials SET DiagStatus = 0 WHERE CusTutID = 704039");

                // When
                var result = tutorialContentDataService
                    .GetNonArchivedCcObjectivesBySectionAndCustomisationId(248, 22062, true)
                    .ToList();

                // Then
                using (new AssertionScope())
                {
                    result.Count.Should().Be(4);
                    result.First().TutorialId.Should().Be(1);

                    result.Last().TutorialId.Should().Be(1257);

                    var postLearnResult = result.Single(x => x.TutorialId == 1138);
                    postLearnResult.TutorialName.Should().Be("Managing files");
                    postLearnResult.Possible.Should().Be(1);
                    postLearnResult.WrongCount.Should().Be(0);
                    postLearnResult.MyScore.Should().Be(0);
                }
            }
        }
    }
}
