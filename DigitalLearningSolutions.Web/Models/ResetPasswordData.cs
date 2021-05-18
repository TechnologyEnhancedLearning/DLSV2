namespace DigitalLearningSolutions.Web.Models
{
    public class ResetPasswordData
    {
        public ResetPasswordData(string email, string resetPasswordHash)
        {
            this.ResetPasswordHash = resetPasswordHash;
            this.Email = email;
        }

        public readonly string ResetPasswordHash;
        public readonly string Email;
    }
}
