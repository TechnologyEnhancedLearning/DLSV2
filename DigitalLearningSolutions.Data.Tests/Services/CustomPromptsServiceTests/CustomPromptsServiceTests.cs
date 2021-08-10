namespace DigitalLearningSolutions.Data.Tests.Services.CustomPromptsServiceTests
{
    using DigitalLearningSolutions.Data.DataServices.CustomPromptsDataService;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services.CustomPromptsService;
    using FakeItEasy;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public partial class CustomPromptsServiceTests
    {
        private ICustomPromptsDataService customPromptsDataService = null!;
        private ICustomPromptsService customPromptsService = null!;
        private ILogger<CustomPromptsService> logger = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            customPromptsDataService = A.Fake<ICustomPromptsDataService>();
            logger = A.Fake<ILogger<CustomPromptsService>>();
            userDataService = A.Fake<IUserDataService>();
            customPromptsService = new CustomPromptsService(customPromptsDataService, logger, userDataService);
        }
    }
}
