namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using NUnit.Framework;

    public class ProfessionalRegistrationNumberHelperTests
    {
        [TestCase(null)]
        [TestCase(false)]
        public void GetHasProfessionalRegistrationNumberForView_returns_null_when_has_not_been_prompted(
            bool? hasBeenPromptedForPrn
        )
        {
            // When
            var result =
                ProfessionalRegistrationNumberHelper.GetHasProfessionalRegistrationNumberForView(
                    hasBeenPromptedForPrn,
                    "prn"
                );

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetHasProfessionalRegistrationNumberForView_returns_false_when_has_been_prompted_and_no_prn()
        {
            // When
            var result =
                ProfessionalRegistrationNumberHelper.GetHasProfessionalRegistrationNumberForView(true, null);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void GetHasProfessionalRegistrationNumberForView_returns_true_when_has_been_prompted_and_has_prn()
        {
            // When
            var result =
                ProfessionalRegistrationNumberHelper.GetHasProfessionalRegistrationNumberForView(true, "prn-12");

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void ValidateProfessionalRegistrationNumber_does_not_set_errors_when_has_no_prn()
        {
            // Given
            var state = new ModelStateDictionary();

            // When
            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                state,
                false,
                null
            );

            // Then
            state.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidateProfessionalRegistrationNumber_does_not_set_errors_when_valid_prn()
        {
            // Given
            var state = new ModelStateDictionary();
            const string validPrn = "AB123456";

            // When
            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                state,
                true,
                validPrn
            );

            // Then
            state.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidateProfessionalRegistrationNumber_sets_error_when_hasPrn_is_not_set()
        {
            // Given
            var state = new ModelStateDictionary();

            // When
            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                state,
                null,
                null
            );

            // Then
            using (new AssertionScope())
            {
                state.IsValid.Should().BeFalse();
                state.Values.First().Errors.Should().OnlyContain(e => e.ErrorMessage == "Select a professional registration number status");
            }
        }

        [TestCase(null, "Enter a professional registration number")]
        [TestCase("", "Enter a professional registration number")]
        [TestCase("1234", "Professional registration number must be between 5 and 20 characters")]
        [TestCase("1234", "Professional registration number must be between 5 and 20 characters")]
        [TestCase(
            "01234_",
         "Invalid professional registration number format. " +
        "Valid formats include: 7 digits (e.g., 1234567), 1–2 letters followed by 6 digits (e.g., AB123456), " +
        "4–8 digits, an optional 'P' plus 5–6 digits, 'C' or 'P' plus 6 digits, " +
        "an optional letter plus 5–6 digits, 'L' plus 4–6 digits, " +
        "or 2 digits followed by a hyphen and 4–5 alphanumeric characters (e.g., 12-AB123)."

        )]
        [TestCase(
            "01234 ",
          "Invalid professional registration number format. " +
        "Valid formats include: 7 digits (e.g., 1234567), 1–2 letters followed by 6 digits (e.g., AB123456), " +
        "4–8 digits, an optional 'P' plus 5–6 digits, 'C' or 'P' plus 6 digits, " +
        "an optional letter plus 5–6 digits, 'L' plus 4–6 digits, " +
        "or 2 digits followed by a hyphen and 4–5 alphanumeric characters (e.g., 12-AB123)."

        )]
        [TestCase(
            "01234$",
          "Invalid professional registration number format. " +
        "Valid formats include: 7 digits (e.g., 1234567), 1–2 letters followed by 6 digits (e.g., AB123456), " +
        "4–8 digits, an optional 'P' plus 5–6 digits, 'C' or 'P' plus 6 digits, " +
        "an optional letter plus 5–6 digits, 'L' plus 4–6 digits, " +
        "or 2 digits followed by a hyphen and 4–5 alphanumeric characters (e.g., 12-AB123)."

        )]
        public void ValidateProfessionalRegistrationNumber_sets_error_when_prn_is_invalid(
            string prn,
            string expectedError
        )
        {
            // Given
            var state = new ModelStateDictionary();

            // When
            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                state,
                true,
                prn
            );

            // Then
            using (new AssertionScope())
            {
                state.IsValid.Should().BeFalse();
                state.Values.First().Errors.Should().OnlyContain(e => e.ErrorMessage == expectedError);
            }
        }
    }
}
