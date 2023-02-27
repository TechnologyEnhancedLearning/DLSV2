namespace DigitalLearningSolutions.Web.Tests.Attributes
{
    using DigitalLearningSolutions.Web.Attributes;
    using NUnit.Framework;

    public class CommonPasswordsAttributeTests
    {
        private const string ErrorMessage = "error message";

        [Test]
        [TestCase("kducj&123JUJJJ", true)]
        [TestCase("monkey", false)]
        [TestCase("OpenSunshine123", false)]
        public void Password_might_contain_common_element(string password, bool expectedResult)
        {
            // Given
            const string value = "kducj&123JUJJJ";
            var attribute = new CommonPasswordsAttribute(ErrorMessage);

            // When
            var result = attribute.IsValid(value);

            // Then
            result.Equals(expectedResult);
        }
    }
}
