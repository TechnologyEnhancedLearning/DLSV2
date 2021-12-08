namespace DigitalLearningSolutions.Data.Tests.Models.Tracker
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using FluentAssertions;
    using NUnit.Framework;

    public class ObjectiveTests
    {
        public static IEnumerable<object> InteractionsTestData()
        {
            yield return new object[] { "450,3,14", new[] { 450, 3, 14 } };
            yield return new object[] { "450,,14", new[] { 450, 14 } };
            yield return new object[] { "", new int[] { } };
            yield return new object[] { null, new int[] { } };
        }

        [Theory]
        [TestCaseSource(nameof(InteractionsTestData))]
        public void Objective_constructor_parses_comma_separated_interactions_string(
            string interactionString,
            IEnumerable<int> parsedInteractions
        )
        {
            // When
            var objective = new Objective(1, interactionString, 100);

            // Then
            objective.Interactions.Should().BeEquivalentTo(parsedInteractions);
        }
    }
}
