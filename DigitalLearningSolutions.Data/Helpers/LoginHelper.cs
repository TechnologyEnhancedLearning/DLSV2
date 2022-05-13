namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

    public static class LoginHelper
    {
        public static UserEntity FilterUserEntityForLoggingIntoSingleCentre(UserEntity userEntity, int centreId)
        {
            var delegateAccountAtCentre = userEntity.DelegateAccounts.SingleOrDefault(da => da.CentreId == centreId);
            var adminAccountAtCentre = userEntity.AdminAccounts.SingleOrDefault(aa => aa.CentreId == centreId);
            var delegateAccountList = delegateAccountAtCentre != null
                ? new List<DelegateAccount> { delegateAccountAtCentre }
                : new List<DelegateAccount>();
            var adminAccountList= adminAccountAtCentre != null
                ? new List<AdminAccount> { adminAccountAtCentre }
                : new List<AdminAccount>();

            return new UserEntity(userEntity.UserAccount, adminAccountList, delegateAccountList);
        }
    }
}
