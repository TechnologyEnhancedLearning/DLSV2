namespace DigitalLearningSolutions.Data.Factories
{
    using MailKit.Net.Smtp;

    public interface ISmtpClientFactory
    {
        public ISmtpClient GetSmtpClient();
    }

    public class SmtpClientFactory : ISmtpClientFactory
    {
        public SmtpClientFactory() { }

        public ISmtpClient GetSmtpClient()
        {
            return new SmtpClient();
        }
    }
}
