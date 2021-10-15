namespace DigitalLearningSolutions.Data.Tests.Enums
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using FluentAssertions;
    using NUnit.Framework;

    public class DelegateCreationErrorTests
    {
        public static IEnumerable<object[]> GetTestCases() {
            yield return new object[] { "-1", DelegateCreationError.UnexpectedError };
            yield return new object[] { "-4", DelegateCreationError.EmailAlreadyInUse };
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases))]
        public void ParsesErrorCodesCorrectly(string errorCode, DelegateCreationError expectedError)
        {
            // When
            var error = DelegateCreationError.FromStoredProcedureErrorCode(errorCode);

            // Then
            error.Should().Be(expectedError);
        }
    }
}
