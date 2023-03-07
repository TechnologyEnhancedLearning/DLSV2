namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using FluentAssertions;
    using NUnit.Framework;

    public class DelegateUserCardTests
    {
        [Test]
        public void DelegateUserCard_gets_registration_type_correctly_self_registered()
        {
            // When
            var result = new DelegateUserCard() { SelfReg = true, ExternalReg = false };

            // Then
            result.RegistrationType.Should().Be(RegistrationType.SelfRegistered);
        }

        [Test]
        public void DelegateUserCard_gets_registration_type_correctly_self_registered_external()
        {
            // When
            var result = new DelegateUserCard() { SelfReg = true, ExternalReg = true };

            // Then
            result.RegistrationType.Should().Be(RegistrationType.SelfRegisteredExternal);
        }

        [Test]
        public void DelegateUserCard_gets_registration_type_correctly_registered_by_centre()
        {
            // When
            var result1 = new DelegateUserCard() { SelfReg = false, ExternalReg = false };
            var result2 = new DelegateUserCard() { SelfReg = false, ExternalReg = true };

            // Then
            result1.RegistrationType.Should().Be(RegistrationType.RegisteredByCentre);
            result2.RegistrationType.Should().Be(RegistrationType.RegisteredByCentre);
        }
    }
}
