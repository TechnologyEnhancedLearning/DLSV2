namespace DigitalLearningSolutions.Data.Tests.NBuilderHelpers
{
    using System;
    using NUnit.Framework;

    public static class NBuilderAlphabeticalPropertyNamingHelper
    {
        public static string IndexToAlphabeticalString(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), @"Index must be greater than or equal to zero");
            }

            var remainder = index % 26;
            var place = index / 26;

            var s = new string('Z', place);
            s += GetNthLetterOfAlphabet(remainder);

            return s;
        }

        private static char GetNthLetterOfAlphabet(int n)
        {
            return (char)(n + 65);
        }
    }

    public class NBuilderAlphabeticalPropertyNamingHelperTests
    {
        [Test]
        [TestCase(0, ExpectedResult = "A")]
        [TestCase(25, ExpectedResult = "Z")]
        [TestCase(26, ExpectedResult = "ZA")]
        [TestCase(51, ExpectedResult = "ZZ")]
        [TestCase(52, ExpectedResult = "ZZA")]
        [TestCase(77, ExpectedResult = "ZZZ")]
        [TestCase(78, ExpectedResult = "ZZZA")]
        [TestCase(103, ExpectedResult = "ZZZZ")]
        public string IndexToAlphabeticalString_returns_expected_string(int inputIndex)
        {
            return NBuilderAlphabeticalPropertyNamingHelper.IndexToAlphabeticalString(inputIndex);
        }
    }

}
