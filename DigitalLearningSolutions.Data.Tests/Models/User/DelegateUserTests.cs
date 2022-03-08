namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using DigitalLearningSolutions.Data.Models.User;
    using FluentAssertions;
    using NUnit.Framework;

    public class DelegateUserTests
    {
        [Test]
        public void PrnDisplayString_is_the_prn_when_the_delegate_has_been_prompted_and_has_provided_a_prn()
        {
            // Given
            const string? prn = "12345";

            // When
            var result = new DelegateUser { HasBeenPromptedForPrn = true, ProfessionalRegistrationNumber = prn };

            // Then
            result.PrnDisplayString().Should().Be(prn);
        }

        [Test]
        public void
            PrnDisplayString_is_Not_professionally_registered_when_the_delegate_has_been_prompted_and_has_not_provided_a_prn()
        {
            // When
            var result = new DelegateUser { HasBeenPromptedForPrn = true, ProfessionalRegistrationNumber = null };

            // Then
            result.PrnDisplayString().Should().Be("Not professionally registered");
        }

        [Test]
        public void PrnDisplayString_is_Not_yet_provided_when_the_delegate_has_not_been_prompted_for_a_prn()
        {
            // When
            var result = new DelegateUser { HasBeenPromptedForPrn = false, ProfessionalRegistrationNumber = null };

            // Then
            result.PrnDisplayString().Should().Be("Not yet provided");
        }
    }
}
