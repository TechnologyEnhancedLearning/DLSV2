namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using NUnit.Framework;

    public class UnlockServiceTests
    {
        private UnlockService unlockService;
        private IUnlockDataService unlockDataService;
        private IConfigService configService;

        [SetUp]
        public void Setup()
        {
            unlockDataService = A.Fake<IUnlockDataService>();
            configService = A.Fake<IConfigService>();

            unlockService = new UnlockService(unlockDataService, configService);
        }

        [Test]
        public void Trying_to_send_mail_with_null_config_values_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailPassword)).Returns(null);

            // Then
            Assert.Throws<ConfigValueMissingException>(() => unlockService.SendUnlockRequest(1));
        }
    }
}
