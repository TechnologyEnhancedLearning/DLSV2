namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Web.ViewModels.Login;

    public static class LoginTestHelper
    {
        public static LoginViewModel GetDefaultLoginViewModel
        (
            string username = "testUsername",
            string password = "testPassword",
            bool rememberMe = false
        )
        {
            return new LoginViewModel
            {
                Username = username,
                Password = password,
                RememberMe = rememberMe
            };
        }
    }
}
