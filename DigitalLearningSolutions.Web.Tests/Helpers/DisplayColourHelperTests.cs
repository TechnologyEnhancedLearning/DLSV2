namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class DisplayColourHelperTests
    {
        [Test]
        public void GetDisplayColourForPercentage_returns_grey_for_zero_of_zero()
        {
            // When
            var result = DisplayColourHelper.GetDisplayColourForPercentage(0, 0);

            // Then
            result.Should().BeEquivalentTo("grey");
        }

        [Test]
        public void GetDisplayColourForPercentage_returns_blue_for_no_limit()
        {
            // When
            var result = DisplayColourHelper.GetDisplayColourForPercentage(10, -1);

            // Then
            result.Should().BeEquivalentTo("blue");
        }

        [Test]
        public void GetDisplayColourForPercentage_returns_blue_for_no_limit_with_no_value()
        {
            // When
            var result = DisplayColourHelper.GetDisplayColourForPercentage(0, -1);

            // Then
            result.Should().BeEquivalentTo("blue");
        }

        [Test]
        public void GetDisplayColourForPercentage_returns_green_for_under_sixty_percent()
        {
            // When
            var result = DisplayColourHelper.GetDisplayColourForPercentage(59, 100);

            // Then
            result.Should().BeEquivalentTo("green");
        }

        [Test]
        public void GetDisplayColourForPercentage_returns_yellow_for_sixty_percent()
        {
            // When
            var result = DisplayColourHelper.GetDisplayColourForPercentage(60, 100);

            // Then
            result.Should().BeEquivalentTo("yellow");
        }

        [Test]
        public void GetDisplayColourForPercentage_returns_red_for_one_hundred_percent()
        {
            // When
            var result = DisplayColourHelper.GetDisplayColourForPercentage(100, 100);

            // Then
            result.Should().BeEquivalentTo("red");
        }

        [Test]
        public void GetDisplayColourForPercentage_returns_red_for_zero_limit_with_value()
        {
            // When
            var result = DisplayColourHelper.GetDisplayColourForPercentage(100, 0);

            // Then
            result.Should().BeEquivalentTo("red");
        }
    }
}
