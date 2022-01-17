namespace DigitalLearningSolutions.Data.Tests.Extensions
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Extensions;
    using FluentAssertions;
    using NUnit.Framework;

    public class EnumerableExtensionsTests
    {
        [Test]
        public void WhereNotNull_filters_items_correctly()
        {
            // Given
            var list = new List<string?> { "1", "2", null, "3", null, "4" };

            // When
            var result = list.WhereNotNull();

            // Then
            var expectedResult = new List<string> { "1", "2", "3", "4" };
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
