namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Configuration
{
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts;
    using FluentAssertions;
    using NUnit.Framework;

    public class AddRegistrationPromptConfigureAnswersViewModelTests
    {
        [Test]
        [TestCase("test\r\ntest", true)]
        [TestCase("test\r\nTest", true)]
        [TestCase("test  \r\ntest", true)]
        [TestCase("test  \r\nTest", true)]
        [TestCase("test \r\nToast \r\nTest", true)]
        [TestCase("test \r\nToast", false)]
        public void OptionsStringContainsDuplicates_detects_duplicates_in_options_string(
            string optionsString,
            bool expectedResult
        )
        {
            // given
            var model = new RegistrationPromptAnswersViewModel(optionsString);

            // when
            var result = model.OptionsStringContainsDuplicates();

            // then
            result.Should().Be(expectedResult);
        }
    }
}
