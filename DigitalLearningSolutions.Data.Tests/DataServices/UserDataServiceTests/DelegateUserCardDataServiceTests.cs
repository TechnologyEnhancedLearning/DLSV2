namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        [Test]
        public void GetDelegateUserCardById_populates_DelegateUser_fields_correctly()
        {
            // Given
            var expected = UserTestHelper.GetDefaultDelegateUser();

            // When
            var userCard = userDataService.GetDelegateUserCardById(2);

            // Then
            userCard.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GetDelegateUserCardById_populates_DelegateUserCard_fields_correctly()
        {
            // When
            var userCard = userDataService.GetDelegateUserCardById(3)!;
            
            // Then
            using (new AssertionScope())
            {
                userCard.Active.Should().BeTrue();
                userCard.SelfReg.Should().BeFalse();
                userCard.ExternalReg.Should().BeFalse();
                userCard.AdminId.Should().Be(1);
                userCard.EmailAddress.Should().Be("kevin.whittaker1@nhs.net");
                userCard.JobGroupId.Should().Be(10);
            }
        }

        [Test]
        public void GetDelegateUserCardById_does_not_match_admin_if_not_admin_in_this_centre()
        {
            // When
            var userCard = userDataService.GetDelegateUserCardById(268530)!;

            // Then
            userCard.AdminId.Should().BeNull();
        }

        [Test]
        public void GetDelegateUserCardById_does_not_match_admin_if_admin_email_address_is_blank()
        {
            // When
            var userCard = userDataService.GetDelegateUserCardById(41300)!;

            // Then
            userCard.AdminId.Should().BeNull();
        }

        [Test]
        public void GetDelegateUserCardsByCentreId_populates_DelegateUser_fields_correctly()
        {
            // Given
            var expected = UserTestHelper.GetDefaultDelegateUser(
                dateRegistered: DateTime.Parse("2010-09-22 06:52:09.080"),
                jobGroupName: "Nursing / midwifery"
            );

            // When
            var userCards = userDataService.GetDelegateUserCardsByCentreId(2);

            // Then
            var userCard = userCards.Single(user => user.Id == 2);
            userCard.Should().BeEquivalentTo(expected);
            userCard.Active.Should().BeTrue();
            userCard.SelfReg.Should().BeFalse();
            userCard.ExternalReg.Should().BeFalse();
            userCard.AdminId.Should().BeNull();
            userCard.JobGroupId.Should().Be(1);
        }

        [Test]
        public void GetDelegateUserCardsByCentreId_populates_DelegateUserCard_fields_correctly()
        {
            // When
            var userCards = userDataService.GetDelegateUserCardsByCentreId(101);

            // Then
            var userCard = userCards.Single(user => user.Id == 3);
            using (new AssertionScope())
            {
                userCard.Active.Should().BeTrue();
                userCard.SelfReg.Should().BeFalse();
                userCard.ExternalReg.Should().BeFalse();
                userCard.AdminId.Should().Be(1);
                userCard.EmailAddress.Should().Be("kevin.whittaker1@nhs.net");
                userCard.JobGroupId.Should().Be(10);
            }
        }

        [Test]
        public void GetDelegateUserCardsByCentreId_does_not_match_admin_if_not_admin_in_this_centre()
        {
            // When
            var userCards = userDataService.GetDelegateUserCardsByCentreId(409);

            // Then
            var userCard = userCards.Single(user => user.Id == 268530);
            userCard.AdminId.Should().BeNull();
        }
    }
}
