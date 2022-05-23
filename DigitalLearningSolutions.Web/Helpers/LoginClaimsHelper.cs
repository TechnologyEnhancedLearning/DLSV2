namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Models.User;

    public static class LoginClaimsHelper
    {
        public static List<Claim> GetClaimsForSignIntoCentre(
            UserEntity userEntity,
            int centreIdToLogInto
        )
        {
            var adminAccount = userEntity.AdminAccounts.SingleOrDefault(aa => aa.CentreId == centreIdToLogInto);
            var delegateAccount = userEntity.DelegateAccounts.SingleOrDefault(da => da.CentreId == centreIdToLogInto);

            var basicClaims = GetClaimsForCentrelessSignIn(userEntity.UserAccount);

            var adminClaims = GetAdminSpecificClaimsForSignIn(adminAccount);
            var delegateClaims = GetDelegateSpecificClaimsForSignIn(delegateAccount);

            var claims = new List<Claim>
            {
                new Claim(
                    CustomClaimTypes.UserCentreId,
                    adminAccount?.CentreId.ToString() ?? delegateAccount?.CentreId.ToString()
                ),
                new Claim(CustomClaimTypes.UserCentreManager, adminAccount?.IsCentreManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreAdmin, adminAccount?.IsCentreAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserUserAdmin, adminAccount?.IsSuperAdmin.ToString() ?? "False"),
                new Claim(
                    CustomClaimTypes.UserContentCreator,
                    adminAccount?.IsContentCreator.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.UserAuthenticatedCm,
                    adminAccount?.IsContentManager.ToString() ?? "False"
                ),
                new Claim(CustomClaimTypes.UserPublishToAll, adminAccount?.PublishToAll.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreReports, adminAccount?.IsReportsViewer.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnUserAuthenticated, (delegateAccount != null).ToString()),
                new Claim(CustomClaimTypes.IsSupervisor, adminAccount?.IsSupervisor.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsTrainer, adminAccount?.IsTrainer.ToString() ?? "False"),
                new Claim(
                    CustomClaimTypes.IsFrameworkDeveloper,
                    adminAccount?.IsFrameworkDeveloper.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.UserCentreName,
                    adminAccount?.CentreName ?? delegateAccount?.CentreName
                ),
                new Claim(
                    CustomClaimTypes.IsFrameworkContributor,
                    adminAccount?.IsFrameworkContributor.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.IsWorkforceManager,
                    adminAccount?.IsWorkforceManager.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.IsWorkforceContributor,
                    adminAccount?.IsWorkforceContributor.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.IsLocalWorkforceManager,
                    adminAccount?.IsLocalWorkforceManager.ToString() ?? "False"
                ),
            };
            
            return basicClaims.Concat(adminClaims).Concat(delegateClaims).Concat(claims).ToList();
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
