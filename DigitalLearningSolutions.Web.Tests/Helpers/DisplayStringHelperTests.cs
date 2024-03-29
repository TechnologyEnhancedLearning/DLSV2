﻿namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class DisplayStringHelperTests
    {
        private const long Gibibyte = 1073741824;
        private const long Mebibyte = 1048576;

        [Test]
        public void GenerateNumberWithLimitDisplayString_returns_expected_string_with_limit()
        {
            // When
            var result = DisplayStringHelper.FormatNumberWithLimit(1, 5);

            // Then
            result.Should().Be("1 / 5");
        }

        [Test]
        public void GenerateNumberWithLimitDisplayString_returns_expected_string_with_no_limit()
        {
            // When
            var result = DisplayStringHelper.FormatNumberWithLimit(1, -1);

            // Then
            result.Should().Be("1");
        }

        [Test]
        public void FormatBytesWithLimit_returns_expected_string_for_bytes()
        {
            // When
            var result = DisplayStringHelper.FormatBytesWithLimit(12, 120);

            // Then
            result.Should().Be("12B / 120B");
        }

        [Test]
        public void FormatBytesWithLimit_returns_expected_string_for_kilobytes()
        {
            // When
            var result = DisplayStringHelper.FormatBytesWithLimit(12, 1200);

            // Then
            result.Should().Be("12B / 1KB");
        }

        [Test]
        public void FormatBytesWithLimit_returns_expected_string_for_gibibytes()
        {
            // When
            var result = DisplayStringHelper.FormatBytesWithLimit(12, Gibibyte);

            // Then
            result.Should().Be("12B / 1GB");
        }

        [Test]
        public void FormatBytesWithLimit_returns_expected_string_for_gibibytes_with_decimal()
        {
            // When
            var result = DisplayStringHelper.FormatBytesWithLimit(12, Gibibyte + 500000000);

            // Then
            result.Should().Be("12B / 1.5GB");
        }

        [Test]
        public void FormatBytesWithLimit_returns_expected_string_for_mebibytes_with_decimal()
        {
            // When
            var result = DisplayStringHelper.FormatBytesWithLimit(12, Mebibyte + 800000);

            // Then
            result.Should().Be("12B / 2MB");
        }

        [Test]
        public void FormatBytesWithLimit_returns_expected_string_when_less_than_next_size()
        {
            // Given
            var bytes = Gibibyte - 10;

            // When
            var result = DisplayStringHelper.FormatBytesWithLimit(12, bytes);

            // Then
            result.Should().Be("12B / 1024MB");
        }

        [Test]
        public void FormatBytesWithLimit_throws_exception_with_negative_bytes()
        {
            // When
            Action action = () => DisplayStringHelper.FormatBytesWithLimit(-1, 0);

            // Then
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void ConvertNumberToMonthsString_should_return_null_for_zero()
        {
            // When
            var result = DisplayStringHelper.ConvertNumberToMonthsString(0);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void ConvertNumberToMonthsString_should_return_month_for_one()
        {
            // When
            var result = DisplayStringHelper.ConvertNumberToMonthsString(1);

            // Then
            result.Should().Be("1 month");
        }

        [Test]
        public void ConvertNumberToMonthsString_should_return_months_for_more_than_one()
        {
            // When
            var result = DisplayStringHelper.ConvertNumberToMonthsString(2);

            // Then
            result.Should().Be("2 months");
        }

        [Test]
        public void GetNonSortableFullNameForDisplayOnly_returns_expected_name_with_no_first_name()
        {
            // When
            var result = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(null, "LastName");

            // Then
            result.Should().Be("LastName");
        }

        [Test]
        public void GetNonSortableFullNameForDisplayOnly_returns_expected_name_with_first_name()
        {
            // When
            var result = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly("FirstName", "LastName");

            // Then
            result.Should().Be("FirstName LastName");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetNameWithEmailForDisplay_returns_name_only_when_email_is_null_or_whitespace(string? email)
        {
            // Given
            const string fullName = "FirstName LastName";

            // When
            var result = DisplayStringHelper.GetNameWithEmailForDisplay(fullName, email);

            // Then
            result.Should().Be(fullName);
        }

        [Test]
        public void GetNameWithEmailForDisplay_returns_correctly_formatted_name_with_email_when_both_are_provided()
        {
            // Given
            const string fullName = "FirstName LastName";
            const string email = "email@email.com";

            // When
            var result = DisplayStringHelper.GetNameWithEmailForDisplay(fullName, email);

            // Then
            result.Should().Be("FirstName LastName (email@email.com)");
        }

        [Test]
        public void GetPotentiallyInactiveAdminName_returns_correctly_formatted_name_for_active_admin()
        {
            // Given
            const string firstName = "FirstName";
            const string lastName = "LastName";
            const bool active = true;

            // When
            var result = DisplayStringHelper.GetPotentiallyInactiveAdminName(firstName, lastName, active);

            // Then
            result.Should().Be("FirstName LastName");
        }

        [Test]
        public void GetPotentiallyInactiveAdminName_returns_correctly_formatted_name_for_inactive_admin()
        {
            // Given
            const string firstName = "FirstName";
            const string lastName = "LastName";
            const bool active = false;

            // When
            var result = DisplayStringHelper.GetPotentiallyInactiveAdminName(firstName, lastName, active);

            // Then
            result.Should().Be("FirstName LastName (inactive)");
        }

        [Test]
        public void GetPotentiallyInactiveAdminName_returns_null_for_null_input()
        {
            // When
            var result = DisplayStringHelper.GetPotentiallyInactiveAdminName(null, null, null);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetPluralitySuffix_returns_s_when_number_is_zero()
        {
            // When
            var result = DisplayStringHelper.GetPluralitySuffix(0);

            // Then
            result.Should().Be("s");
        }

        [Test]
        public void GetPluralitySuffix_returns_s_when_number_is_greater_than_1()
        {
            // When
            var result = DisplayStringHelper.GetPluralitySuffix(2);

            // Then
            result.Should().Be("s");
        }

        [Test]
        public void GetPluralitySuffix_returns_empty_string_when_number_is_1()
        {
            // When
            var result = DisplayStringHelper.GetPluralitySuffix(1);

            // Then
            result.Should().Be(string.Empty);
        }

        [Test]
        public void ReplaceNonAlphaNumericSpaceChars_null_input_returns_null()
        {
            // When
            var result = DisplayStringHelper.ReplaceNonAlphaNumericSpaceChars(null, "a");

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void ReplaceNonAlphaNumericSpaceChars_returns_cleaned_string_with_replacement()
        {
            // Given
            var input = "abcdefghijklmnopqrstuvwxyz" +
                        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                        "1234567890" +
                        "`¬¦!\"£$%^&*)(_+-=[]{};'#:@~\\|,./<>? ";

            var expectedOutput = "abcdefghijklmnopqrstuvwxyz" +
                                 "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                 "1234567890" +
                                 "rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr ";

            // When
            var result = DisplayStringHelper.ReplaceNonAlphaNumericSpaceChars(input, "r");

            // Then
            result.Should().Be(expectedOutput);
        }

        [Test]
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("some@fake.email", " (some@fake.email)")]
        public void GetEmailDisplayString_returns_expected_result(string? email, string expectedResult)
        {
            // When
            var result = DisplayStringHelper.GetEmailDisplayString(email);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase(0, "0m")]
        [TestCase(59, "59m")]
        [TestCase(60, "1h 0m")]
        [TestCase(89, "1h 29m")]
        [TestCase(2332, "38h 52m")]
        public void GetTimeStringFromMinutes_returns_expected_result(int minutes, string expectedResult)
        {
            // When
            var result = DisplayStringHelper.GetTimeStringFromMinutes(minutes);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase(0, "0 minutes")]
        [TestCase(59, "59 minutes")]
        [TestCase(60, "1 hours 0 minutes")]
        [TestCase(89, "1 hours 29 minutes")]
        [TestCase(2332, "38 hours 52 minutes")]
        public void GetTimeStringForScreenReaderFromMinutes_returns_expected_result(int minutes, string expectedResult)
        {
            // When
            var result = DisplayStringHelper.GetTimeStringForScreenReaderFromMinutes(minutes);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase("", "")]
        [TestCase("NicePascalCaseString", "Nice Pascal Case String")]
        [TestCase("ASlightlyMoreAwkwardOne", "A Slightly More Awkward One")]
        [TestCase("XMLFile", "XML File")]
        [TestCase("AnXMLFile", "An XML File")]
        [TestCase("An Already Separated String", "An Already Separated String")]
        [TestCase("A Partially SeparatedString", "A Partially Separated String")]
        [TestCase("IDRecord", "ID Record")]
        [TestCase("ID", "ID")]
        public void AddSpacesToPascalCaseString_returns_expected_result(string input, string expectedResult)
        {
            // When
            var result = DisplayStringHelper.AddSpacesToPascalCaseString(input);

            // Then
            result.Should().Be(expectedResult);
        }
    }
}
