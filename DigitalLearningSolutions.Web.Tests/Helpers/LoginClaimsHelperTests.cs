namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using FluentAssertions;
    using NUnit.Framework;

    public class LoginHelperTests
    {
        [Test]
        public void Delegate_user_forename_and_surname_set_correctly()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(firstName: "fname", lastName: "lname");
            var delegateLoginDetails = new DelegateLoginDetails(delegateUser);

            // When
            var claims = LoginClaimsHelper.GetClaimsForSignIn(null, delegateLoginDetails);

            // Then
            claims.Should().Contain((claim) => claim.Type == CustomClaimTypes.UserForename);
            claims.Should().Contain((claim) => claim.Type == CustomClaimTypes.UserSurname);
            var forenameClaim = claims.Find((claim) => claim.Type == CustomClaimTypes.UserForename);
            var surnameClaim = claims.Find((claim) => claim.Type == CustomClaimTypes.UserSurname);
            forenameClaim.Value.Should().Be("fname");
            surnameClaim.Value.Should().Be("lname");
        }
        
        [Test]
        public void User_without_email_has_empty_string_email_claim()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: string.Empty);
            var adminLoginDetails = new AdminLoginDetails(adminUser);

            // When
            var claims = LoginClaimsHelper.GetClaimsForSignIn(adminLoginDetails, null);

            // Then
            claims.Should().Contain((claim) => claim.Type == ClaimTypes.Email);
            var emailClaim = claims.Find((claim) => claim.Type == ClaimTypes.Email);
            emailClaim.Value.Should().Be(string.Empty);
        }

        [Test]
        public void Admin_only_user_does_not_have_learn_candidate_id_or_learn_candidate_number()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var adminLoginDetails = new AdminLoginDetails(adminUser);

            // When
            var claims = LoginClaimsHelper.GetClaimsForSignIn(adminLoginDetails, null);

            // Then
            claims.Should().NotContain((claim) => claim.Type == CustomClaimTypes.LearnCandidateId);
            claims.Should().NotContain((claim) => claim.Type == CustomClaimTypes.LearnCandidateNumber);
        }

        [Test]
        public void Delegate_only_user_does_not_have_user_admin_id_or_admin_category_id()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var delegateLoginDetails = new DelegateLoginDetails(delegateUser);

            // When
            var claims = LoginClaimsHelper.GetClaimsForSignIn(null, delegateLoginDetails);

            // Then
            claims.Should().NotContain((claim) => claim.Type == CustomClaimTypes.UserAdminId);
            claims.Should().NotContain((claim) => claim.Type == CustomClaimTypes.AdminCategoryId);
        }

    }
}
