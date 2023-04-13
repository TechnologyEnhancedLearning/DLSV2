using DigitalLearningSolutions.Data.Models.User;
using DigitalLearningSolutions.Data.Models;
using DigitalLearningSolutions.Data.ViewModels;
using System.Collections.Generic;
using DigitalLearningSolutions.Data.ViewModels.UserCentreAccount;
using System.Linq;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IUserCentreAccountsService
    {

        IEnumerable<UserCentreAccountsRoleViewModel> GetUserCentreAccountsRoleViewModel(
            UserEntity? userEntity,
            List<int> idsOfCentresWithUnverifiedEmails
        );

    }
    public class UserCentreAccountsService : IUserCentreAccountsService
    {
        public IEnumerable<UserCentreAccountsRoleViewModel> GetUserCentreAccountsRoleViewModel(
               UserEntity? userEntity,
               List<int> idsOfCentresWithUnverifiedEmails
           )
        {
            return userEntity!.CentreAccountSetsByCentreId.Values.Where(
                centreAccountSet => centreAccountSet.AdminAccount?.Active == true ||
                                    centreAccountSet.DelegateAccount != null
            ).Select(
                centreAccountSet => new UserCentreAccountsRoleViewModel(
                    centreAccountSet.CentreId,
                    centreAccountSet.CentreName,
                    centreAccountSet.IsCentreActive,
                    centreAccountSet.AdminAccount?.Active == true,
                    centreAccountSet.DelegateAccount != null,
                    centreAccountSet.DelegateAccount?.Approved ?? false,
                    centreAccountSet.DelegateAccount?.Active ?? false,
                    idsOfCentresWithUnverifiedEmails.Contains(centreAccountSet.CentreId)
                )
            );
        }
    }
}
