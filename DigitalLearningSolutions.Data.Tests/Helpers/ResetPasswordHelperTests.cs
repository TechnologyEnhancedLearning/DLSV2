namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using DigitalLearningSolutions.Data.Models.User;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class ResetPasswordHelperTests
    {
        [Test]
        public void Reset_Password_Is_Valid_119_Minutes_After_Creation()
        {
            // Given
            var createTime = DateTime.UtcNow;
            var resetPassword = Builder<ResetPassword>.CreateNew()
                .With(rp => rp.PasswordResetDateTime = createTime).Build();

            // When
            var resetIsValid = resetPassword.IsStillValidAt(createTime + TimeSpan.FromMinutes(119), ResetPasswordHelpers.ResetPasswordHashExpiryTime);

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
            var resetIsValid = resetPassword.IsStillValidAt(createTime + TimeSpan.FromMinutes(121), ResetPasswordHelpers.ResetPasswordHashExpiryTime);

            // Then
            resetIsValid.Should().BeFalse();
        }

        [Test]
        public void Set_Password_Is_Valid_4319_Minutes_After_Creation()
        {
            // Given
            var createTime = DateTime.UtcNow;
            var resetPassword = Builder<ResetPassword>.CreateNew()
                .With(rp => rp.PasswordResetDateTime = createTime).Build();

            // When
            var resetIsValid = resetPassword.IsStillValidAt(createTime + TimeSpan.FromMinutes(4319), ResetPasswordHelpers.SetPasswordHashExpiryTime);

            // Then
            resetIsValid.Should().BeTrue();
        }

        [Test]
        public void Set_Password_Is_Invalid_121_Minutes_After_Creation()
        {
            // Given
            var createTime = DateTime.UtcNow;
            var resetPassword = Builder<ResetPassword>.CreateNew()
                .With(rp => rp.PasswordResetDateTime = createTime).Build();

            // When
            var resetIsValid = resetPassword.IsStillValidAt(createTime + TimeSpan.FromMinutes(4321), ResetPasswordHelpers.SetPasswordHashExpiryTime);

            // Then
            resetIsValid.Should().BeFalse();
        }

        [Test]
        public void Gets_reset_password_ids_from_users_correctly()
        {
            // Given
            (AdminUser?, List<DelegateUser>) users = (
                Builder<AdminUser>.CreateNew().With(u => u.ResetPasswordId = 1).Build(),
                Builder<DelegateUser>.CreateListOfSize(20)
                    .TheFirst(1).With(u => u.ResetPasswordId = null)
                    .TheNext(3).With(u => u.ResetPasswordId = 8)
                    .TheNext(2).With(u => u.ResetPasswordId = 27)
                    .TheNext(9).With(u => u.ResetPasswordId = 64)
                    .TheNext(5).With(u => u.ResetPasswordId = 120)
                    .Build().ToList());

            // When
            var resetPasswordIds = users.GetDistinctResetPasswordIds();

            // Then
            resetPasswordIds.Should().BeEquivalentTo(new[] { 1, 8, 27, 64, 120 });
        }
    }
}
