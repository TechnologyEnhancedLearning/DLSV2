namespace DigitalLearningSolutions.Web.Tests.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FluentAssertions;
    using NUnit.Framework;

    public class MyAccountEditDetailsFormDataTests
    {
        [Test]
        public void
            MyAccountEditDetailsFormData_Validate_returns_validation_results_for_AllCentreSpecificEmailsDictionary()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var allCentreSpecificEmailsDictionary = new Dictionary<string, string?>
            {
                { "1", "email@centre1.com" },
                { "2", "email @centre2.com" },
                { "3", "email_centre3" },
                { "4", $"email@centre4.com{new string('m', 300)}" },
                { "5", null },
            };

            var model = new MyAccountEditDetailsFormData
            {
                FirstName = userAccount.FirstName,
                LastName = userAccount.LastName,
                Email = userAccount.PrimaryEmail,
                IsDelegateUser = false,
                IsCheckDetailRedirect = false,
                AllCentreSpecificEmailsDictionary = allCentreSpecificEmailsDictionary,
            };

            var expectedValidationResults = new List<ValidationResult>
            {
                new ValidationResult(
                    CommonValidationErrorMessages.WhitespaceInEmail,
                    new[] { $"{nameof(MyAccountEditDetailsFormData.AllCentreSpecificEmailsDictionary)}_2" }
                ),
                new ValidationResult(
                    CommonValidationErrorMessages.InvalidEmail,
                    new[] { $"{nameof(MyAccountEditDetailsFormData.AllCentreSpecificEmailsDictionary)}_3" }
                ),
                new ValidationResult(
                    CommonValidationErrorMessages.TooLongEmail,
                    new[] { $"{nameof(MyAccountEditDetailsFormData.AllCentreSpecificEmailsDictionary)}_4" }
                ),
            };

            // When
            var validationResults = model.Validate(new ValidationContext(model));

            // Then
            validationResults.Should().BeEquivalentTo(expectedValidationResults);
        }

        [Test]
        public void
            MyAccountEditDetailsFormData_Validate_only_validates_AllCentreSpecificEmailsDictionary_once_if_called_multiple_times()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var allCentreSpecificEmailsDictionary = new Dictionary<string, string?>
            {
                { "1", "email @centre1.com" },
            };

            var model = new MyAccountEditDetailsFormData
            {
                FirstName = userAccount.FirstName,
                LastName = userAccount.LastName,
                Email = userAccount.PrimaryEmail,
                IsDelegateUser = false,
                IsCheckDetailRedirect = false,
                AllCentreSpecificEmailsDictionary = allCentreSpecificEmailsDictionary,
            };

            var expectedValidationResults = new List<ValidationResult>
            {
                new ValidationResult(
                    CommonValidationErrorMessages.WhitespaceInEmail,
                    new[] { $"{nameof(MyAccountEditDetailsFormData.AllCentreSpecificEmailsDictionary)}_1" }
                ),
            };

            // When
            var validationResults = model.Validate(new ValidationContext(model));
            var validationResults2 = model.Validate(new ValidationContext(model));

            // Then
            validationResults.Should().BeEquivalentTo(expectedValidationResults);
            validationResults2.Should().BeEmpty();
        }
    }
}
