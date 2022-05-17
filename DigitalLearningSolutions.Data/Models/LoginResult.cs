namespace DigitalLearningSolutions.Data.Models
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;

    public class LoginResult
    {
        public LoginResult(
            LoginAttemptResult result,
            UserEntity? userEntity = null,
            int? centreToLogInto = null
        )
        {
            LoginAttemptResult = result;
            UserEntity = userEntity;
            CentreToLogInto = centreToLogInto;
        }

        public LoginAttemptResult LoginAttemptResult { get; set; }

        public int? CentreToLogInto { get; set; }

        public UserEntity? UserEntity { get; set; }
    }
}
