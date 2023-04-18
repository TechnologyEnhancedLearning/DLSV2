using DigitalLearningSolutions.Data.Models.User;
using DigitalLearningSolutions.Data.ViewModels;
using DigitalLearningSolutions.Data.ViewModels.UserCentreAccount;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.UserCentreAccounts
{
    public class UserCentreAccountRoleViewModel
    {
        public UserCentreAccountRoleViewModel(List<UserCentreAccountsRoleViewModel> centreUserDetails,
                 UserEntity userEntity)
        {
            CentreUserDetails = centreUserDetails;
            UserEntity = userEntity;
        }
        public List<UserCentreAccountsRoleViewModel> CentreUserDetails { get; set; }
        public UserEntity UserEntity { get; set; }
        public string? SearchString { get; set; }

        public string? ExistingFilterString { get; set; }

        public int Page { get; set; }
    }
}
