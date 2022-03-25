namespace DigitalLearningSolutions.Data.Tests.Helpers.GenericSearchHelperTests
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
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
                UserTestHelper.GetDefaultDelegateUser(
                    10,
                    firstName: "Mark",
                    lastName: "Jones",
                    candidateNumber: "MJ10",
                    emailAddress: "mark.jones@nhs.com"
                ),
                UserTestHelper.GetDefaultDelegateUser(
                    12,
                    firstName: "Ethan",
                    lastName: "Jones",
                    candidateNumber: "EJ12",
                    emailAddress: "ethanjones_77@nhs.gov.uk"
                ),
                UserTestHelper.GetDefaultDelegateUser(
                    14,
                    firstName: "Emily",
                    lastName: "Harper",
                    candidateNumber: "EH14",
                    emailAddress: "hypatia@hotmail.com"
                ),
                UserTestHelper.GetDefaultDelegateUser(
                    144,
                    firstName: "Andrew",
                    lastName: "Harper",
                    candidateNumber: "AH144",
                    emailAddress: "ah_moorfields@eyecare.co.uk"
                ),
                UserTestHelper.GetDefaultDelegateUser(
                    200,
                    firstName: "Mark",
                    lastName: "Alonso",
                    candidateNumber: "ML200",
                    emailAddress: null
                ),
                UserTestHelper.GetDefaultDelegateUser(
                    202,
                    firstName: "Sandra",
                    lastName: "Halondrus",
                    candidateNumber: "SH202",
                    emailAddress: "halondrussandra@gmail.com"
                ),
            };
        }

        [TestCase("Mark", new[] { 10, 200 })]
        [TestCase("MJ1", new[] { 10 })]
        [TestCase("J1", new[] { 10, 12 })]
        [TestCase("Harper", new[] { 14, 144 })]
        [TestCase("alon", new[] { 200, 202 })]
        [TestCase("hypatia", new[] { 14 })]
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
