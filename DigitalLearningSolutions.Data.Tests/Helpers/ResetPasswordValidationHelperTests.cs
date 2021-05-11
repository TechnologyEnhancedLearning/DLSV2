namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class ResetPasswordValidationHelperTests
    {
        [Test]
        public void Reset_Password_Is_Valid_119_Minutes_After_Creation()
        {
            // Given
            var createTime = DateTime.UtcNow;
            var resetPassword = Builder<ResetPassword>.CreateNew()
                .With(rp => rp.PasswordResetDateTime = createTime).Build();

            // When
            var resetIsValid = resetPassword.IsStillValidAt(createTime + TimeSpan.FromMinutes(119));

            // Then
            resetIsValid.Should().BeTrue();
        }

        [Test]
        public void Reset_Password_Is_Invalid_121_Minutes_After_Creation()
        {
            // Given
            var createTime = DateTime.UtcNow;
            var resetPassword = Builder<ResetPassword>.CreateNew()
                .With(rp => rp.PasswordResetDateTime = createTime).Build();

            // When
            var resetIsValid = resetPassword.IsStillValidAt(createTime + TimeSpan.FromMinutes(121));

            // Then
            resetIsValid.Should().BeFalse();
        }
    }
}
