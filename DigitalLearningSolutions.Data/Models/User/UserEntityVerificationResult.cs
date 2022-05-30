namespace DigitalLearningSolutions.Data.Models.User
{
    using System.Collections.Generic;
    using System.Linq;

    public class UserEntityVerificationResult
    {
        public UserEntityVerificationResult(
            bool userAccountPassed,
            IEnumerable<int> nullPasswordDelegateIds,
            IEnumerable<int> passedDelegateIds,
            IEnumerable<int> failedDelegateIds
        )
        {
            UserAccountPassedVerification = userAccountPassed;
            PassedVerificationDelegateAccountIds = passedDelegateIds;
            FailedVerificationDelegateAccountIds = failedDelegateIds;
            DelegateAccountsWithNoPassword = nullPasswordDelegateIds;
        }

        public bool UserAccountPassedVerification { get; set; }
        public IEnumerable<int> DelegateAccountsWithNoPassword { get; set; }
        public IEnumerable<int> PassedVerificationDelegateAccountIds { get; set; }
        public IEnumerable<int> FailedVerificationDelegateAccountIds { get; set; }

        public bool PasswordMatchesAllAccountPasswords => UserAccountPassedVerification && !FailedVerificationDelegateAccountIds.Any();

        public bool PasswordMatchesAtLeastOneAccountPassword =>
            UserAccountPassedVerification || PassedVerificationDelegateAccountIds.Any();
    }
}
