namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using NUnit.Framework;

    public class LoginServiceTests
    {
        private ICryptoService cryptoService;
        private LoginService loginService;
        private IUserService userService;

        [SetUp]
        public void Setup()
        {
            userService = A.Fake<IUserService>();
            cryptoService = A.Fake<ICryptoService>();

            loginService = new LoginService(userService, cryptoService);
        }

        [Test]
        public void Return_no_delegate_users_when_delegate_stored_password_is_empty()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);

            var delegateUserWithoutPassword = UserTestHelper.GetDefaultDelegateUser(password: string.Empty);

            //When
            var (_, returnedDelegateList) = loginService.VerifyUsers(
                "password",
                UserTestHelper.GetDefaultAdminUser(),
                new List<DelegateUser> { delegateUserWithoutPassword });

            // Then
            Assert.IsEmpty(returnedDelegateList);
        }

        [Test]
        public void Return_no_associated_admin_users_when_delegate_email_is_empty()
        {
            var delegateUserWithoutEmail = UserTestHelper.GetDefaultDelegateUser(emailAddress: null);

            // When
            var returnedAdminUser = loginService.GetVerifiedAdminUserAssociatedWithApprovedDelegateUser(
                delegateUserWithoutEmail, "password");

            // Then
            Assert.IsNull(returnedAdminUser);
        }
    }
}
