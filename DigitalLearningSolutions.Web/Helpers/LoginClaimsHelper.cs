namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Models.User;

    public static class LoginClaimsHelper
    {
        public static List<Claim> GetClaimsForSignIn(
            UserEntity userEntity
        )
        {
            var adminLoginDetails = userEntity.AdminAccounts.SingleOrDefault();
            var delegateLoginDetails = userEntity.DelegateAccounts.SingleOrDefault();

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Email,
                    userEntity.UserAccount.PrimaryEmail // TODO: do we want the centre specific email here instead?
                ),
                new Claim(CustomClaimTypes.UserId, userEntity.UserAccount.Id.ToString()),
                new Claim(
                    CustomClaimTypes.UserCentreId,
                    adminLoginDetails?.CentreId.ToString() ?? delegateLoginDetails?.CentreId.ToString()
                ),
                new Claim(CustomClaimTypes.UserCentreManager, adminLoginDetails?.IsCentreManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreAdmin, adminLoginDetails?.IsCentreAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserUserAdmin, adminLoginDetails?.IsSuperAdmin.ToString() ?? "False"),
                new Claim(
                    CustomClaimTypes.UserContentCreator,
                    adminLoginDetails?.IsContentCreator.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.UserAuthenticatedCm,
                    adminLoginDetails?.IsContentManager.ToString() ?? "False"
                ),
                new Claim(CustomClaimTypes.UserPublishToAll, adminLoginDetails?.PublishToAll.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreReports, adminLoginDetails?.IsReportsViewer.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnUserAuthenticated, (delegateLoginDetails != null).ToString()),
                new Claim(CustomClaimTypes.IsSupervisor, adminLoginDetails?.IsSupervisor.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsTrainer, adminLoginDetails?.IsTrainer.ToString() ?? "False"),
                new Claim(
                    CustomClaimTypes.IsFrameworkDeveloper,
                    adminLoginDetails?.IsFrameworkDeveloper.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.UserCentreName,
                    adminLoginDetails?.CentreName ?? delegateLoginDetails?.CentreName
                ),
                new Claim(
                    CustomClaimTypes.IsFrameworkContributor,
                    adminLoginDetails?.IsFrameworkContributor.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.IsWorkforceManager,
                    adminLoginDetails?.IsWorkforceManager.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.IsWorkforceContributor,
                    adminLoginDetails?.IsWorkforceContributor.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.IsLocalWorkforceManager,
                    adminLoginDetails?.IsLocalWorkforceManager.ToString() ?? "False"
                )
            };

            var firstName = userEntity.UserAccount.FirstName;
            var surname = userEntity.UserAccount.LastName;
            claims.Add(new Claim(CustomClaimTypes.UserForename, firstName));
            claims.Add(new Claim(CustomClaimTypes.UserSurname, surname));
            if (delegateLoginDetails?.CandidateNumber != null)
            {
                claims.Add(new Claim(CustomClaimTypes.LearnCandidateNumber, delegateLoginDetails.CandidateNumber));
            }

            if (adminLoginDetails?.Id != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserAdminId, adminLoginDetails.Id.ToString()));
            }

            if (delegateLoginDetails?.Id != null)
            {
                claims.Add(new Claim(CustomClaimTypes.LearnCandidateId, delegateLoginDetails.Id.ToString()));
            }

            if (adminLoginDetails != null)
            {
                claims.Add(new Claim(CustomClaimTypes.AdminCategoryId, adminLoginDetails.CategoryId.ToString())); // TODO - does the null here break anything? Might have to default it to 0 and then revert to null when recovering via the helper
            }

            return claims;
        }
    }
}
