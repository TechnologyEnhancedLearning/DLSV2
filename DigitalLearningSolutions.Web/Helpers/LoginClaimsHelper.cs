namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using DigitalLearningSolutions.Web.Models;

    public static class LoginClaimsHelper
    {
        public static List<Claim> GetClaimsForSignIn(
            AdminLoginDetails? adminLoginDetails,
            DelegateLoginDetails? delegateLoginDetails
        )
        {
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Email,
                    adminLoginDetails?.EmailAddress ?? delegateLoginDetails?.EmailAddress ?? string.Empty
                ),
                new Claim(
                    CustomClaimTypes.UserCentreId,
                    adminLoginDetails?.CentreId.ToString() ?? delegateLoginDetails?.CentreId.ToString()
                ),
                new Claim(CustomClaimTypes.UserCentreManager, adminLoginDetails?.IsCentreManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreAdmin, adminLoginDetails?.IsCentreAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserUserAdmin, adminLoginDetails?.IsUserAdmin.ToString() ?? "False"),
                new Claim(
                    CustomClaimTypes.UserContentCreator,
                    adminLoginDetails?.IsContentCreator.ToString() ?? "False"
                ),
                new Claim(
                    CustomClaimTypes.UserAuthenticatedCm,
                    adminLoginDetails?.IsContentManager.ToString() ?? "False"
                ),
                new Claim(CustomClaimTypes.UserPublishToAll, adminLoginDetails?.PublishToAll.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreReports, adminLoginDetails?.SummaryReports.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnUserAuthenticated, (delegateLoginDetails != null).ToString()),
                new Claim(CustomClaimTypes.IsSupervisor, adminLoginDetails?.IsSupervisor.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsNominatedSupervisor, adminLoginDetails?.IsNominatedSupervisor.ToString() ?? "False"),
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

            var firstName = adminLoginDetails?.FirstName ?? delegateLoginDetails?.FirstName;
            var surname = adminLoginDetails?.LastName ?? delegateLoginDetails?.LastName;

            if (firstName != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserForename, firstName));
            }

            if (surname != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserSurname, surname));
            }

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
                claims.Add(new Claim(CustomClaimTypes.AdminCategoryId, adminLoginDetails.CategoryId.ToString()));
            }

            return claims;
        }
    }
}
