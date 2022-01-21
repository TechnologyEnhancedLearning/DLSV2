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

        [TestCase(true, false)]
        [TestCase(false, true)]
        public void ValidateProfessionalRegistrationNumber_does_not_set_errors_when_not_delegate_or_has_no_prn(
            bool isDelegate,
            bool hasPrn
        )
        {
            // Given
            var state = new ModelStateDictionary();

            // When
            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                state,
                hasPrn,
                null,
                isDelegate
            );

            // Then
            state.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidateProfessionalRegistrationNumber_does_not_set_errors_when_valid_prn()
        {
            // Given
            var state = new ModelStateDictionary();
            const string validPrn = "abc-123";

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
                state.Values.First().Errors.Should().OnlyContain(e => e.ErrorMessage == "Select your professional registration number status");
            }
        }

        [TestCase(null, "Enter professional registration number")]
        [TestCase("", "Enter professional registration number")]
        [TestCase("123", "Professional registration number must be between 5 and 20 characters")]
        [TestCase("0123456789-0123456789", "Professional registration number must be between 5 and 20 characters")]
        [TestCase(
            "01234_",
            "Invalid professional registration number format - Only alphanumeric (a-z, A-Z and 0-9) and hyphens (-) allowed"
        )]
        [TestCase(
            "01234 ",
            "Invalid professional registration number format - Only alphanumeric (a-z, A-Z and 0-9) and hyphens (-) allowed"
        )]
        [TestCase(
            "01234$",
            "Invalid professional registration number format - Only alphanumeric (a-z, A-Z and 0-9) and hyphens (-) allowed"
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
