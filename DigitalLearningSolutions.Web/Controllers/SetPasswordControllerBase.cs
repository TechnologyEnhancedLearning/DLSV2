namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public abstract class SetPasswordControllerBase : Controller
    {
        protected readonly IPasswordResetService PasswordResetService;
        protected readonly IPasswordService PasswordService;

        protected SetPasswordControllerBase(
            IPasswordResetService passwordResetService,
            IPasswordService passwordService
        )
        {
            PasswordResetService = passwordResetService;
            PasswordService = passwordService;
        }

        protected async Task<bool> IsSetPasswordLinkValid(
            string emailAddress,
            string resetHash,
            bool isSetPassword = false
        )
        {
            return await PasswordResetService.EmailAndResetPasswordHashAreValidAsync(
                emailAddress,
                resetHash,
                isSetPassword
            );
        }

        protected async Task<bool> IsSetPasswordLinkValid(
            ResetPasswordData resetPasswordData,
            bool isSetPassword = false
        )
        {
            return await IsSetPasswordLinkValid(
                resetPasswordData.Email,
                resetPasswordData.ResetPasswordHash,
                isSetPassword
            );
        }

        protected async Task ChangePassword(ConfirmPasswordViewModel viewModel, ResetPasswordData resetPasswordData)
        {
            await PasswordResetService.InvalidateResetPasswordForEmailAsync(resetPasswordData.Email);
            await PasswordService.ChangePasswordAsync(resetPasswordData.Email, viewModel.Password!);

            TempData.Clear();
        }
    }
}
