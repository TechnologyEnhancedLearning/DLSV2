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
            Accounts = new UserAccountSet(adminUser, delegateUsers);
            AvailableCentres = availableCentres ?? new List<CentreUserDetails>();
        }

        public LoginAttemptResult LoginAttemptResult { get; set; }

        public UserAccountSet Accounts { get; set; }

        public List<CentreUserDetails> AvailableCentres { get; set; }
    }
}
