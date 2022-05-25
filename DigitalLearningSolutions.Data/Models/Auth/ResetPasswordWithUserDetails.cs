namespace DigitalLearningSolutions.Data.Models.Auth
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.DbModels;

    public class ResetPasswordWithUserDetails : ResetPassword
    {
        public int UserId { get; set; }
        public string Email { get; set; }
    }
}
