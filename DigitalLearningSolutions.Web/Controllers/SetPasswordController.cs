namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Services;

    public class SetPasswordController : ResetPasswordController
    {
        public SetPasswordController(IPasswordResetService passwordResetService, IPasswordService passwordService) : base(passwordResetService, passwordService) { }
    }
}
