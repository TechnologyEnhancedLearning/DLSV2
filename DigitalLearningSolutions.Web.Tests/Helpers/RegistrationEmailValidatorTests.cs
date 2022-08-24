namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using NUnit.Framework;

    public class RegistrationEmailValidatorTests
    {
        private const int DefaultUserId = 1;
        private const int DefaultCentreId = 7;
        private const string DefaultPrimaryEmail = "primary@email.com";
        private const string DefaultCentreSpecificEmail = "centre@email.com";
        private const string DefaultErrorMessage = "error message";
        private const string DefaultFieldName = "FieldName";

        private static ModelStateDictionary modelState = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;
        private ICentresService centresService = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            userService = A.Fake<IUserService>();
            centresService = A.Fake<ICentresService>();
            modelState = new ModelStateDictionary();
        }

        [Test]
        public void ValidatePrimaryEmailIfNecessary_adds_error_message_when_primary_email_is_in_use()
        {
            // Given
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultPrimaryEmail)).Returns(true);

            // When
            RegistrationEmailValidator.ValidatePrimaryEmailIfNecessary(
                DefaultPrimaryEmail,
                DefaultFieldName,
                modelState,
                userDataService,
                DefaultErrorMessage
            );

            // Then
            AssertModelStateErrorIsExpected(DefaultFieldName, DefaultErrorMessage);
        }

        [Test]
        public void ValidatePrimaryEmailIfNecessary_does_not_add_error_message_when_primary_email_is_not_in_use()
        {
            // Given
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultPrimaryEmail)).Returns(false);

            // When
            RegistrationEmailValidator.ValidatePrimaryEmailIfNecessary(
                DefaultPrimaryEmail,
                DefaultFieldName,
                modelState,
                userDataService,
                DefaultErrorMessage
            );

            // Then
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidatePrimaryEmailIfNecessary_does_not_add_error_message_when_email_is_null()
        {
            // When
            RegistrationEmailValidator.ValidatePrimaryEmailIfNecessary(
                null,
                DefaultFieldName,
                modelState,
                userDataService,
                DefaultErrorMessage
            );

            // Then
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidatePrimaryEmailIfNecessary_does_not_add_another_error_message_when_field_already_has_error()
        {
            // Given
            modelState.AddModelError(DefaultFieldName, DefaultErrorMessage);

            A.CallTo(() => userDataService.PrimaryEmailIsInUse(DefaultPrimaryEmail)).Returns(true);

            // When
            RegistrationEmailValidator.ValidatePrimaryEmailIfNecessary(
                DefaultPrimaryEmail,
                DefaultFieldName,
                modelState,
                userDataService,
                "different error"
            );

            // Then
            modelState[DefaultFieldName].Errors.Count.Should().Be(1);
            AssertModelStateErrorIsExpected(DefaultFieldName, DefaultErrorMessage);
        }

        [Test]
        public void ValidateCentreEmailIfNecessary_adds_error_message_when_centre_email_is_in_use_at_centre()
        {
            // Given
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentre(DefaultCentreSpecificEmail, DefaultCentreId)
            ).Returns(true);

            // When
            RegistrationEmailValidator.ValidateCentreEmailIfNecessary(
                DefaultCentreSpecificEmail,
                DefaultCentreId,
                DefaultFieldName,
                modelState,
                userDataService
            );

            // Then
            AssertModelStateErrorIsExpected(DefaultFieldName, CommonValidationErrorMessages.EmailInUseAtCentre);
        }

        [Test]
        public void
            ValidateCentreEmailIfNecessary_does_not_add_error_message_when_centre_email_is_not_in_use_at_centre()
        {
            // Given
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentre(DefaultCentreSpecificEmail, DefaultCentreId)
            ).Returns(false);

            // When
            RegistrationEmailValidator.ValidateCentreEmailIfNecessary(
                DefaultCentreSpecificEmail,
                DefaultCentreId,
                DefaultFieldName,
                modelState,
                userDataService
            );

            // Then
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidateCentreEmailIfNecessary_does_not_add_error_message_when_email_is_null()
        {
            // When
            RegistrationEmailValidator.ValidateCentreEmailIfNecessary(
                null,
                DefaultCentreId,
                DefaultFieldName,
                modelState,
                userDataService
            );

            // Then
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidateCentreEmailIfNecessary_does_not_add_another_error_message_when_field_already_has_error()
        {
            // Given
            modelState.AddModelError(DefaultFieldName, DefaultErrorMessage);

            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentre(DefaultCentreSpecificEmail, DefaultCentreId)
            ).Returns(true);

            // When
            RegistrationEmailValidator.ValidateCentreEmailIfNecessary(
                DefaultCentreSpecificEmail,
                DefaultCentreId,
                DefaultFieldName,
                modelState,
                userDataService
            );

            // Then
            modelState[DefaultFieldName].Errors.Count.Should().Be(1);

            // note: error message is not CommonValidationErrorMessages.EmailInUseAtCentre
            AssertModelStateErrorIsExpected(DefaultFieldName, DefaultErrorMessage);
        }

        [Test]
        public void
            ValidateCentreEmailIfNecessary_does_nothing_if_centreId_is_null()
        {
            // When
            RegistrationEmailValidator.ValidateCentreEmailIfNecessary(
                DefaultCentreSpecificEmail,
                null,
                DefaultFieldName,
                modelState,
                userDataService
            );

            // Then
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentre(
                    A<string>._,
                    A<int>._
                )
            ).MustNotHaveHappened();

            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidateCentreEmailWithUserIdIfNecessary_adds_error_message_when_centre_email_is_in_use_at_centre()
        {
            // Given
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    DefaultCentreSpecificEmail,
                    DefaultCentreId,
                    DefaultUserId
                )
            ).Returns(true);

            // When
            RegistrationEmailValidator.ValidateCentreEmailWithUserIdIfNecessary(
                DefaultCentreSpecificEmail,
                DefaultCentreId,
                DefaultUserId,
                DefaultFieldName,
                modelState,
                userDataService
            );

            // Then
            AssertModelStateErrorIsExpected(DefaultFieldName, CommonValidationErrorMessages.EmailInUseAtCentre);
        }

        [Test]
        public void
            ValidateCentreEmailWithUserIdIfNecessary_does_not_add_error_message_when_centre_email_is_not_in_use_at_centre()
        {
            // Given
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    DefaultCentreSpecificEmail,
                    DefaultCentreId,
                    DefaultUserId
                )
            ).Returns(false);

            // When
            RegistrationEmailValidator.ValidateCentreEmailWithUserIdIfNecessary(
                DefaultCentreSpecificEmail,
                DefaultCentreId,
                DefaultUserId,
                DefaultFieldName,
                modelState,
                userDataService
            );

            // Then
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidateCentreEmailWithUserIdIfNecessary_does_not_add_error_message_when_email_is_null()
        {
            // When
            RegistrationEmailValidator.ValidateCentreEmailWithUserIdIfNecessary(
                null,
                DefaultCentreId,
                DefaultUserId,
                DefaultFieldName,
                modelState,
                userDataService
            );

            // Then
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void
            ValidateCentreEmailWithUserIdIfNecessary_does_not_add_another_error_message_when_field_already_has_error()
        {
            // Given
            modelState.AddModelError(DefaultFieldName, DefaultErrorMessage);

            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    DefaultCentreSpecificEmail,
                    DefaultCentreId,
                    DefaultUserId
                )
            ).Returns(true);

            // When
            RegistrationEmailValidator.ValidateCentreEmailWithUserIdIfNecessary(
                DefaultCentreSpecificEmail,
                DefaultCentreId,
                DefaultUserId,
                DefaultFieldName,
                modelState,
                userDataService
            );

            // Then
            modelState[DefaultFieldName].Errors.Count.Should().Be(1);

            // note: error message is not CommonValidationErrorMessages.EmailInUseAtCentre
            AssertModelStateErrorIsExpected(DefaultFieldName, DefaultErrorMessage);
        }

        [Test]
        public void
            ValidateCentreEmailWithUserIdIfNecessary_does_nothing_if_centreId_is_null()
        {
            // When
            RegistrationEmailValidator.ValidateCentreEmailWithUserIdIfNecessary(
                DefaultCentreSpecificEmail,
                null,
                DefaultUserId,
                DefaultFieldName,
                modelState,
                userDataService
            );

            // Then
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    A<string>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();

            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void
            ValidateEmailsForCentreManagerIfNecessary_adds_error_message_when_neither_primary_nor_centre_email_is_valid_for_centre_manager()
        {
            // Given
            A.CallTo(
                () => centresService.IsAnEmailValidForCentreManager(
                    DefaultPrimaryEmail,
                    DefaultCentreSpecificEmail,
                    DefaultCentreId
                )
            ).Returns(false);

            // When
            RegistrationEmailValidator.ValidateEmailsForCentreManagerIfNecessary(
                DefaultPrimaryEmail,
                DefaultCentreSpecificEmail,
                DefaultCentreId,
                DefaultFieldName,
                modelState,
                centresService
            );

            // Then
            AssertModelStateErrorIsExpected(
                DefaultFieldName,
                CommonValidationErrorMessages.WrongEmailForCentreDuringAdminRegistration
            );
        }

        [Test]
        public void
            ValidateEmailsForCentreManagerIfNecessary_does_not_add_error_message_when_either_primary_or_centre_email_is_valid_for_centre_manager()
        {
            // Given
            A.CallTo(
                () => centresService.IsAnEmailValidForCentreManager(
                    DefaultPrimaryEmail,
                    DefaultCentreSpecificEmail,
                    DefaultCentreId
                )
            ).Returns(true);

            // When
            RegistrationEmailValidator.ValidateEmailsForCentreManagerIfNecessary(
                DefaultPrimaryEmail,
                DefaultCentreSpecificEmail,
                DefaultCentreId,
                DefaultFieldName,
                modelState,
                centresService
            );

            // Then
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void
            ValidateEmailsForCentreManagerIfNecessary_does_not_add_another_error_message_when_the_model_is_already_invalid()
        {
            // Given
            modelState.AddModelError(DefaultFieldName, DefaultErrorMessage);

            A.CallTo(
                () => centresService.IsAnEmailValidForCentreManager(
                    DefaultPrimaryEmail,
                    DefaultCentreSpecificEmail,
                    DefaultCentreId
                )
            ).Returns(false);

            // When
            RegistrationEmailValidator.ValidateEmailsForCentreManagerIfNecessary(
                DefaultPrimaryEmail,
                DefaultCentreSpecificEmail,
                DefaultCentreId,
                DefaultFieldName,
                modelState,
                centresService
            );

            // Then
            modelState[DefaultFieldName].Errors.Count.Should().Be(1);

            // note: error message is not CommonValidationErrorMessages.WrongEmailForCentreDuringAdminRegistration
            AssertModelStateErrorIsExpected(DefaultFieldName, DefaultErrorMessage);
        }

        [Test]
        public void
            ValidateEmailsForCentreManagerIfNecessary_does_nothing_if_centreId_is_null()
        {
            // When
            RegistrationEmailValidator.ValidateEmailsForCentreManagerIfNecessary(
                DefaultPrimaryEmail,
                DefaultCentreSpecificEmail,
                null,
                DefaultFieldName,
                modelState,
                centresService
            );

            // Then
            A.CallTo(
                () => centresService.IsAnEmailValidForCentreManager(
                    A<string>._,
                    A<string>._,
                    A<int>._
                )
            ).MustNotHaveHappened();

            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidateEmailNotHeldAtCentre_raises_error_if_email_held_at_centre()
        {
            // Given
            A.CallTo(() => userService.EmailIsHeldAtCentre("email", 1)).Returns(true);

            // When
            RegistrationEmailValidator.ValidateEmailNotHeldAtCentreIfEmailNotYetVerified(
                "email",
                1,
                "EmailField",
                modelState,
                userService
            );

            // Then
            modelState["EmailField"].Errors.Should()
                .Contain(e => e.ErrorMessage == CommonValidationErrorMessages.EmailInUseAtCentre);
        }

        [Test]
        public void ValidateEmailNotHeldAtCentre_does_not_raise_error_if_email_not_held_at_centre()
        {
            // Given
            A.CallTo(() => userService.EmailIsHeldAtCentre("email", 1)).Returns(false);

            // When
            RegistrationEmailValidator.ValidateEmailNotHeldAtCentreIfEmailNotYetVerified(
                "email",
                1,
                "EmailField",
                modelState,
                userService
            );

            // Then
            modelState.IsValid.Should().BeTrue();
        }

        [Test]
        public void ValidateEmailNotHeldAtCentre_does_not_raise_error_if_email_already_validated()
        {
            // Given
            A.CallTo(() => userService.EmailIsHeldAtCentre("email", 1)).Returns(true);
            modelState.AddModelError("EmailField", DefaultErrorMessage);

            // When
            RegistrationEmailValidator.ValidateEmailNotHeldAtCentreIfEmailNotYetVerified(
                "email",
                1,
                "EmailField",
                modelState,
                userService
            );

            // Then
            modelState["EmailField"].Errors.Count.Should().Be(1);
        }

        [Test]
        public void ValidateEmailNotHeldAtCentre_does_not_raise_error_if_email_is_null()
        {
            // Given
            string? email = null;

            // When
            RegistrationEmailValidator.ValidateEmailNotHeldAtCentreIfEmailNotYetVerified(
                email,
                1,
                "EmailField",
                modelState,
                userService
            );

            // Then
            modelState.IsValid.Should().BeTrue();
        }


        private static void AssertModelStateErrorIsExpected(string modelProperty, string expectedErrorMessage)
        {
            var errorMessage = modelState[modelProperty].Errors.First().ErrorMessage;
            errorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }
    }
}
