namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;

    public class LoginResult
    {
        public LoginResult(
            LoginAttemptResult result,
            AdminUser? adminUser = null,
            List<DelegateUser>? delegateUsers = null,
            List<CentreUserDetails>? availableCentres = null
        )
        {
            LoginAttemptResult = result;
            LogInAdmin = adminUser;
            LogInDelegates = delegateUsers ?? new List<DelegateUser>();
            AvailableCentres = availableCentres;
        }

        public LoginAttemptResult LoginAttemptResult { get; set; }

        public AdminUser? LogInAdmin { get; set; }

        public List<DelegateUser> LogInDelegates { get; set; }

        public List<CentreUserDetails>? AvailableCentres { get; set; }
    }
}
