namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Services;

    public class ResetPasswordController : SetPasswordControllerBase
    {
        public ResetPasswordController(IPasswordResetService passwordResetService, IPasswordService passwordService) :
            base(passwordResetService, passwordService) { }
    }
}
