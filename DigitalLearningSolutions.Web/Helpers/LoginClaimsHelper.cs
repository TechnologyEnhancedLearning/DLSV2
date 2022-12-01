namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.User;

    public static class LoginClaimsHelper
    {
        public static List<Claim> GetClaimsForSignIntoCentre(
            UserEntity userEntity,
            int centreIdToLogInto
        )
        {
            var centreAccountSet = userEntity.GetCentreAccountSet(centreIdToLogInto);

            if (!(centreAccountSet is { CanLogInToCentre: true }))
            {
                throw new LoginWithNoValidAccountException(
                    $"No active admin account or active, approved delegate account at centre {centreIdToLogInto} for user {userEntity.UserAccount.Id}"
                );
            }

            var userCentreClaims = GetClaimsForUserCentre(centreAccountSet);

            var adminAccount = centreAccountSet.CanLogIntoAdminAccount ? centreAccountSet.AdminAccount : null;
            var delegateAccount = centreAccountSet.CanLogIntoDelegateAccount ? centreAccountSet.DelegateAccount : null;

            var basicClaims = GetClaimsForCentrelessSignIn(userEntity.UserAccount);
            var adminClaims = GetAdminSpecificClaimsForSignIn(adminAccount);
            var delegateClaims = GetDelegateSpecificClaimsForSignIn(delegateAccount);
            var permissionClaims = GetPermissionClaimsForSignIn(adminAccount, delegateAccount);

            return basicClaims
                .Concat(userCentreClaims)
                .Concat(adminClaims)
                .Concat(delegateClaims)
                .Concat(permissionClaims)
                .ToList();
        }

        public static List<Claim> GetClaimsForCentrelessSignIn(
            UserAccount userAccount
        )
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userAccount.PrimaryEmail),
                new Claim(CustomClaimTypes.UserId, userAccount.Id.ToString()),
                new Claim(CustomClaimTypes.UserForename, userAccount.FirstName),
                new Claim(CustomClaimTypes.UserSurname, userAccount.LastName),
            };

            return claims;
        }

        private static List<Claim> GetPermissionClaimsForSignIn(
            AdminAccount? adminAccount,
            DelegateAccount? delegateAccount
        )
        {
            var claims = new List<Claim>
            {
                new Claim(CustomClaimTypes.UserCentreManager, adminAccount?.IsCentreManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreAdmin, adminAccount?.IsCentreAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserUserAdmin, adminAccount?.IsSuperAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserContentCreator, adminAccount?.IsContentCreator.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserAuthenticatedCm, adminAccount?.IsContentManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserPublishToAll, adminAccount?.PublishToAll.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreReports, adminAccount?.IsReportsViewer.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnUserAuthenticated, (delegateAccount != null).ToString()),
                new Claim(CustomClaimTypes.IsSupervisor, adminAccount?.IsSupervisor.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsTrainer, adminAccount?.IsTrainer.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsNominatedSupervisor, adminAccount?.IsNominatedSupervisor.ToString() ?? "False"), 
                new Claim(
                    CustomClaimTypes.IsFrameworkDeveloper,
                    adminAccount?.IsFrameworkDeveloper.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.IsFrameworkContributor,
                    adminAccount?.IsFrameworkContributor.ToString() ?? "False"
                ),
                new Claim(CustomClaimTypes.IsWorkforceManager, adminAccount?.IsWorkforceManager.ToString() ?? "False"),
                new Claim(
                    CustomClaimTypes.IsWorkforceContributor,
                    adminAccount?.IsWorkforceContributor.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.IsLocalWorkforceManager,
                    adminAccount?.IsLocalWorkforceManager.ToString() ?? "False"
                ),
            };
            return claims;
        }

        private static IEnumerable<Claim> GetClaimsForUserCentre(CentreAccountSet centreAccountSet)
        {
            return new List<Claim>
            {
                new Claim(CustomClaimTypes.UserCentreId, centreAccountSet.CentreId.ToString()),
                new Claim(CustomClaimTypes.UserCentreName, centreAccountSet.CentreName),
            };
        }

        private static IEnumerable<Claim> GetDelegateSpecificClaimsForSignIn(
            DelegateAccount? delegateAccount
        )
        {
            return delegateAccount != null
                ? new List<Claim>
                {
                    new Claim(CustomClaimTypes.LearnCandidateNumber, delegateAccount.CandidateNumber),
                    new Claim(CustomClaimTypes.LearnCandidateId, delegateAccount.Id.ToString()),
                }
                : new List<Claim>();
        }

        private static IEnumerable<Claim> GetAdminSpecificClaimsForSignIn(
            AdminAccount? adminAccount
        )
        {
            return adminAccount != null
                ? new List<Claim>
                {
                    new Claim(CustomClaimTypes.UserAdminId, adminAccount.Id.ToString()),
                    new Claim(CustomClaimTypes.AdminCategoryId, (adminAccount.CategoryId ?? 0).ToString()),
                }
                : new List<Claim>();
        }
    }
}
