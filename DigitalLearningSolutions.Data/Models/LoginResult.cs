namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;

    public class LoginResult
    {
        public LoginResult(
            LoginAttemptResult result,
            UserEntity? userEntity = null,
            List<CentreUserDetails>? availableCentres = null
            )
        {
            LoginAttemptResult = result;
            UserEntity = userEntity;
            AvailableCentres = availableCentres ?? new List<CentreUserDetails>();
        }

        public LoginAttemptResult LoginAttemptResult { get; set; }

        public List<CentreUserDetails> AvailableCentres { get; set; }

        public UserEntity? UserEntity { get; set; }
    }
}
