namespace DigitalLearningSolutions.Web.Models
{
    public class ResetPasswordData
    {
        public ResetPasswordData(string email, string resetPasswordHash)
        {
            ResetPasswordHash = resetPasswordHash;
            Email = email;
        }

        public readonly string ResetPasswordHash;
        public readonly string Email;
    }
}
