namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CustomPromptsServiceTests
    {
        private ICustomPromptsService customPromptsService;
        private ICustomPromptsDataService customPromptsDataService;

        [SetUp]
        public void Setup()
        {
            customPromptsDataService = A.Fake<ICustomPromptsDataService>();
            customPromptsService = new CustomPromptsService(customPromptsDataService);
        }

        [Test]
        public void GetCustomPromptsForCentreByCentreId_Returns_Populated_CentreCustomPrompts()
        {
            // Given
            var expectedPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(options: null, mandatory: true);
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(text: "Department / team", options: null, mandatory: true);
            var expectedCustomerPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(customPrompt1: expectedPrompt1, customPrompt2: expectedPrompt2);
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(29))
                .Returns(CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(customField1PromptId: 1, customField1Prompt: "Custom Prompt", customField1Options: null));

            // When
            var result = customPromptsService.GetCustomPromptsForCentreByCentreId(29);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedCustomerPrompts);
                result.CustomField1.Should().BeEquivalentTo(expectedPrompt1);
                result.CustomField2.Should().BeEquivalentTo(expectedPrompt2);
                result.CustomField3.Should().BeNull();
            }
        }
    }
}
