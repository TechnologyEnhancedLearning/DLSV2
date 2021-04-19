namespace DigitalLearningSolutions.Web.Tests.ViewModels.MyAccount
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FluentAssertions;
    using NUnit.Framework;

    public class MyAccountViewModelTests
    {
        [Test]
        public void MyAccountViewModel_AdminUser_and_DelegateUser_populates_expected_values()
        {
            // Given
            var adminUser = MyAccountHelper.GetDefaultAdminUser();
            var delegateUser = MyAccountHelper.GetDefaultDelegateUser();
            var centreName = MyAccountHelper.GetDefaultCentreName();
            
            // When
            var returnedModel = new MyAccountViewModel(adminUser, delegateUser, centreName);

            // Then
            returnedModel.FirstName.Should().BeEquivalentTo(adminUser.FirstName);
            returnedModel.Centre.Should().BeEquivalentTo(centreName);
            returnedModel.Surname.Should().BeEquivalentTo(adminUser.LastName);
            returnedModel.ProfilePicture.Should().BeEquivalentTo(adminUser.ProfileImage);
            returnedModel.DelegateNumber.Should().BeEquivalentTo(delegateUser.CandidateNumber);
            returnedModel.User.Should().BeEquivalentTo(adminUser.EmailAddress);
        }

        [Test]
        public void MyAccountViewModel_AdminUser_no_DelegateUser_populates_expected_values()
        {
            // Given
            var adminUser = MyAccountHelper.GetDefaultAdminUser();
            var centreName = MyAccountHelper.GetDefaultCentreName();

            // When
            var returnedModel = new MyAccountViewModel(adminUser, null, centreName);

            // Then
            returnedModel.FirstName.Should().BeEquivalentTo(adminUser.FirstName);
            returnedModel.Centre.Should().BeEquivalentTo(centreName);
            returnedModel.Surname.Should().BeEquivalentTo(adminUser.LastName);
            returnedModel.ProfilePicture.Should().BeEquivalentTo(adminUser.ProfileImage);
            returnedModel.DelegateNumber.Should().BeNull();
            returnedModel.User.Should().BeEquivalentTo(adminUser.EmailAddress);
        }

        [Test]
        public void MyAccountViewModel_DelegateUser_no_AdminUser_populates_expected_values()
        {
            // Given
            var delegateUser = MyAccountHelper.GetDefaultDelegateUser();
            var centreName = MyAccountHelper.GetDefaultCentreName();

            // When
            var returnedModel = new MyAccountViewModel(null, delegateUser, centreName);

            // Then
            returnedModel.FirstName.Should().BeEquivalentTo(delegateUser.FirstName);
            returnedModel.Centre.Should().BeEquivalentTo(centreName);
            returnedModel.Surname.Should().BeEquivalentTo(delegateUser.LastName);
            returnedModel.ProfilePicture.Should().BeEquivalentTo(delegateUser.ProfileImage);
            returnedModel.DelegateNumber.Should().BeEquivalentTo(delegateUser.CandidateNumber);
            returnedModel.User.Should().BeEquivalentTo(delegateUser.EmailAddress);
        }
    }
}
