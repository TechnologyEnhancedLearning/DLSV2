namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
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
                .Returns(CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(customField1Prompt: "Custom Prompt", customField1Options: null));

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

        [Test]
        public void GetCustomPrompts_with_options_splits_correctly()
        {
            // Given
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(29))
                .Returns(CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult());

            // When
            var result = customPromptsService.GetCustomPromptsForCentreByCentreId(29);

            // Then
            using (new AssertionScope())
            {
                result.CustomField1.Should().NotBeNull();
                result.CustomField1.Options.Count.Should().Be(2);
                result.CustomField1.Options[0].Should().BeEquivalentTo("Clinical");
                result.CustomField1.Options[1].Should().BeEquivalentTo("Non-Clinical");
            }
        }
    }
}
