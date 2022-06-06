namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class LoginHelperTests
    {
        [Test]
        public void User_forename_and_surname_set_correctly()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount(firstName: "fname", lastName: "lname");
            var userEntity = new UserEntity(
                userAccount,
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount() }
            );

            // When
            var claims = LoginClaimsHelper.GetClaimsForSignIntoCentre(userEntity, 2);

            // Then
            claims.Should().Contain(claim => claim.Type == CustomClaimTypes.UserForename);
            claims.Should().Contain(claim => claim.Type == CustomClaimTypes.UserSurname);
            var forenameClaim = claims.Find(claim => claim.Type == CustomClaimTypes.UserForename);
            var surnameClaim = claims.Find(claim => claim.Type == CustomClaimTypes.UserSurname);
            forenameClaim.Value.Should().Be("fname");
            surnameClaim.Value.Should().Be("lname");
        }

        [Test]
        public void Admin_only_user_does_not_have_learn_candidate_id_or_learn_candidate_number()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount() },
                new List<DelegateAccount>()
            );

            // When
            var claims = LoginClaimsHelper.GetClaimsForSignIntoCentre(userEntity, 2);

            // Then
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.LearnCandidateId);
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.LearnCandidateNumber);
        }

        [Test]
        public void Delegate_only_user_does_not_have_user_admin_id_or_admin_category_id()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount() }
            );

            // When
            var claims = LoginClaimsHelper.GetClaimsForSignIntoCentre(userEntity, 2);

            // Then
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.UserAdminId);
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.AdminCategoryId);
        }

        [Test]
        public void Inactive_admin_user_does_not_have_user_admin_id_or_user_centre_id_or_user_centre_name()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount(active: false) },
                new List<DelegateAccount>()
            );

            // When
            var claims = LoginClaimsHelper.GetClaimsForSignIntoCentre(userEntity, 2);

            // Then
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.UserAdminId);
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.UserCentreId);
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.UserCentreName);
        }

        [Test]
        public void Inactive_delegate_user_does_not_have_learn_candidate_id_or_user_centre_id_or_user_centre_name()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount(active: false) }
            );

            // When
            var claims = LoginClaimsHelper.GetClaimsForSignIntoCentre(userEntity, 2);

            // Then
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.LearnCandidateId);
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.UserCentreId);
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.UserCentreName);
        }

        [Test]
        public void Unapproved_delegate_user_does_not_have_learn_candidate_id_or_user_centre_id_or_user_centre_name()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount(approved: false) }
            );

            // When
            var claims = LoginClaimsHelper.GetClaimsForSignIntoCentre(userEntity, 2);

            // Then
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.LearnCandidateId);
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.UserCentreId);
            claims.Should().NotContain(claim => claim.Type == CustomClaimTypes.UserCentreName);
        }
    }
}
