namespace DigitalLearningSolutions.Data.Models.User
{
    using System.Collections.Generic;
    using System.Linq;

    public class UserEntityVerificationResult
    {
        public UserEntityVerificationResult(
            bool userAccountPassed,
            IEnumerable<int> passedDelegateIds,
            IEnumerable<int> failedDelegateIds
        )
        {
            UserAccountPassedVerification = userAccountPassed;
            PassedVerificationDelegateAccountIds = passedDelegateIds;
            FailedVerificationDelegateAccountIds = failedDelegateIds;
        }

        public bool UserAccountPassedVerification { get; set; }
        public IEnumerable<int> PassedVerificationDelegateAccountIds { get; set; }
        public IEnumerable<int> FailedVerificationDelegateAccountIds { get; set; }

        public bool PasswordMatchesAllAccountPasswords => UserAccountPassedVerification && !FailedVerificationDelegateAccountIds.Any();

        public bool PasswordMatchesAtLeastOneAccountPassword =>
            UserAccountPassedVerification || PassedVerificationDelegateAccountIds.Any();
    }
}
