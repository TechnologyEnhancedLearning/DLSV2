namespace DigitalLearningSolutions.Web.Tests.Helpers.GenericSearchHelperTests
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class AllDelegatesSearchTests
    {
        private DelegateUser[] delegateUsers = null!;

        [SetUp]
        public void SetUp()
        {
            delegateUsers = new[]
            {
                UserTestHelper.GetDefaultDelegateUser(10, firstName: "Mark", lastName: "Jones", candidateNumber: "MJ10"),
                UserTestHelper.GetDefaultDelegateUser(12, firstName: "Ethan", lastName: "Jones", candidateNumber: "EJ12"),
                UserTestHelper.GetDefaultDelegateUser(14, firstName: "Emily", lastName: "Harper", candidateNumber: "EH14"),
                UserTestHelper.GetDefaultDelegateUser(144, firstName: "Andrew", lastName: "Harper", candidateNumber: "AH144"),
                UserTestHelper.GetDefaultDelegateUser(200, firstName: "Mark", lastName: "Lowe", candidateNumber: "ML200"),
                UserTestHelper.GetDefaultDelegateUser(202, firstName: "Sandra", lastName: "Halos", candidateNumber: "SH202"),
            };
        }

        [TestCase("Mark", new[] { 10, 200 })]
        [TestCase("MJ1", new[] { 10 })]
        [TestCase("J1", new[] { 10, 12 })]
        [TestCase("Harper H14", new[] { 14, 144 })]
        [TestCase("lo", new[] { 200, 202 })]
        public void DelegateUsers_should_be_filtered_on_name_and_candidateNumber_correctly(
            string searchString,
            int[] expectedIds
        )
        {
            // When
            var result = GenericSearchHelper.SearchItems(delegateUsers, searchString);
            var filteredIds = result.Select(delegateUser => delegateUser.Id);

            // Then
            filteredIds.Should().Equal(expectedIds);
        }
    }
}
