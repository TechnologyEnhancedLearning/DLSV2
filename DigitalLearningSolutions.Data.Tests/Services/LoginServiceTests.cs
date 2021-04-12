namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using NUnit.Framework;

    public class LoginServiceTests
    {
        private ICryptoService cryptoService;
        private ILoginService loginService;
        private IUserService userService;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();
            cryptoService = A.Fake<ICryptoService>();
            loginService = new LoginService(userService, cryptoService);
        }

        [Test]
        public void Exception_thrown_when_no_users_returned_from_user_service()
        {
            // Given
            A.CallTo(() => userService.GetAdminUsersByUsername(A<string>._)).Returns(new List<AdminUser>());
            A.CallTo(() => userService.GetDelegateUsersByUsername(A<string>._)).Returns(new List<DelegateUser>());

            // Then
            Assert.Throws<UserAccountNotFoundException>(() => loginService.VerifyUserDetailsAndGetClaims("username", "password"));
        }

        [Test]
        public void Exception_thrown_when_password_mismatches()
        {
            // Given
            A.CallTo(() => userService.GetAdminUsersByUsername(A<string>._)).Returns(new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() });
            A.CallTo(() => userService.GetDelegateUsersByUsername(A<string>._)).Returns(new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() });
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);

            // Then
            Assert.Throws<IncorrectPasswordLoginException>(() => loginService.VerifyUserDetailsAndGetClaims("username", "password"));
        }

        [Test]
        public void Exception_thrown_when_user_is_unapproved_delegate_and_not_admin()
        {
            // Given
            A.CallTo(() => userService.GetAdminUsersByUsername(A<string>._)).Returns(new List<AdminUser>());
            A.CallTo(() => userService.GetDelegateUsersByUsername(A<string>._)).Returns(new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() });
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);
            // Then
            Assert.Throws<DelegateUserNotApprovedException>(() => loginService.VerifyUserDetailsAndGetClaims("username", "password"));
        }
    }
}
