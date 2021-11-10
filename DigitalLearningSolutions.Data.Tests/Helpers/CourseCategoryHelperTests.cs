namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using DigitalLearningSolutions.Data.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    internal class CourseCategoryHelperTests
    {
        [Test]
        public void GetCourseCategoryFilter_returns_null_when_id_is_zero()
        {
            // When
            var result = CourseCategoryHelper.GetCourseCategoryFilter(0);

            // Then
            result.Should().BeNull();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void GetCourseCategoryFilter_returns_id_when_id_is_non_zero(int categoryId)
        {
            // When
            var result = CourseCategoryHelper.GetCourseCategoryFilter(categoryId);

            // Then
            result.Should().Be(categoryId);
        }
    }
}
