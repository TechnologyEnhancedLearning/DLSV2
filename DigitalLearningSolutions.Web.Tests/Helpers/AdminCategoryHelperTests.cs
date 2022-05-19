namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class AdminCategoryHelperTests
    {
        [Test]
        [TestCase(0, null)]
        [TestCase(1, 1)]
        [TestCase(100, 100)]
        public void AdminCategoryToCategoryId_returns_expected_value(int adminCategory, int? expectedValue)
        {
            // When
            var result = AdminCategoryHelper.AdminCategoryToCategoryId(adminCategory);

            // Then
            result.Should().Be(expectedValue);
        }

        [Test]
        [TestCase(null, 0)]
        [TestCase(1, 1)]
        [TestCase(100, 100)]
        public void CategoryIdToAdminCategory_returns_expected_value(int? categoryId, int expectedValue)
        {
            // When
            var result = AdminCategoryHelper.CategoryIdToAdminCategory(categoryId);

            // Then
            result.Should().Be(expectedValue);
        }
    }
}
