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
        UserEntity? userEntity
    );

    bool CentreEmailIsVerified(int userId, int centreIdIfLoggingIntoSingleCentre);
  }
  public class UserCentreAccountsService:IUserCentreAccountsService
  {
    private readonly IUserService userService;
    public IEnumerable<UserCentreAccountsRoleViewModel> GetUserCentreAccountsRoleViewModel(
           UserEntity? userEntity
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
              centreAccountSet.DelegateAccount?.Active ?? false
          )
      );
    }

    public bool CentreEmailIsVerified(int userId, int centreIdIfLoggingIntoSingleCentre)
    {
      var (_, unverifiedCentreEmails) = userService.GetUnverifiedEmailsForUser(userId);
      return unverifiedCentreEmails.Select(uce => uce.centreId)
          .Contains(centreIdIfLoggingIntoSingleCentre) == false;
    }
  }
}
