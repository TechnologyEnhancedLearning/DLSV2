namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using NUnit.Framework;

    public class RegistrationEmailValidatorTests
    {
        private const int DefaultCentreId = 7;
        private const string DefaultPrimaryEmail = "primary@email.com";
        private const string DefaultSecondaryEmail = "centre@email.com";
        private const string DefaultErrorMessage = "error message";

        private const string WrongEmailForCentreErrorMessage =
            "This email address does not match the one held by the centre";

        private const string DuplicateEmailErrorMessage =
            "A user with this email address is already registered; if this is you, " +
            "please log in and register at this centre via the My Account page";

        private static ModelStateDictionary modelState = null!;
        private ICentresService centresService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            userService = A.Fake<IUserService>();
            centresService = A.Fake<ICentresService>();
            modelState = new ModelStateDictionary();
        }

        [Test]
        public void
            ValidateEmailAddressesForDelegateRegistration_adds_no_new_error_messages_when_emails_are_already_invalid()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel(secondaryEmail: null);

            modelState.AddModelError(nameof(PersonalInformationViewModel.PrimaryEmail), DefaultErrorMessage);
            modelState.AddModelError(nameof(PersonalInformationViewModel.SecondaryEmail), DefaultErrorMessage);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForDelegateRegistration(model, modelState, userService);

            // Then
            A.CallTo(() => userService.EmailIsInUse(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => centresService.DoesEmailMatchCentre(A<string>._, A<int>._)).MustNotHaveHappened();
            modelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            modelState[nameof(PersonalInformationViewModel.SecondaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(nameof(PersonalInformationViewModel.PrimaryEmail), DefaultErrorMessage);
            AssertModelStateErrorIsExpected(nameof(PersonalInformationViewModel.SecondaryEmail), DefaultErrorMessage);
        }

        [Test]
        public void ValidateEmailAddressesForDelegateRegistration_adds_errors_on_emails_if_already_in_use()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(true);
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).Returns(true);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForDelegateRegistration(model, modelState, userService);

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresService.DoesEmailMatchCentre(A<string>._, A<int>._)).MustNotHaveHappened();
            modelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            modelState[nameof(PersonalInformationViewModel.SecondaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.PrimaryEmail),
                DuplicateEmailErrorMessage
            );
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.SecondaryEmail),
                DuplicateEmailErrorMessage
            );
        }

        [Test]
        public void ValidateEmailAddressesForDelegateRegistration_adds_no_errors_on_emails_if_valid_and_unique()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).Returns(false);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForDelegateRegistration(model, modelState, userService);

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresService.DoesEmailMatchCentre(A<string>._, A<int>._)).MustNotHaveHappened();
            modelState.ValidationState.Should().Be(ModelValidationState.Valid);
        }

        [Test]
        public void
            ValidateEmailAddressesForAdminRegistration_adds_no_new_error_messages_when_emails_are_already_invalid()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel(secondaryEmail: null);

            modelState.AddModelError(nameof(PersonalInformationViewModel.PrimaryEmail), DefaultErrorMessage);
            modelState.AddModelError(nameof(PersonalInformationViewModel.SecondaryEmail), DefaultErrorMessage);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => centresService.DoesEmailMatchCentre(A<string>._, A<int>._)).MustNotHaveHappened();
            modelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            modelState[nameof(PersonalInformationViewModel.SecondaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(nameof(PersonalInformationViewModel.PrimaryEmail), DefaultErrorMessage);
            AssertModelStateErrorIsExpected(nameof(PersonalInformationViewModel.SecondaryEmail), DefaultErrorMessage);
        }

        [Test]
        public void ValidateEmailAddressesForAdminRegistration_adds_errors_on_emails_if_already_in_use()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(true);
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).Returns(true);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresService.DoesEmailMatchCentre(A<string>._, A<int>._)).MustNotHaveHappened();
            modelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            modelState[nameof(PersonalInformationViewModel.SecondaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.PrimaryEmail),
                DuplicateEmailErrorMessage
            );
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.SecondaryEmail),
                DuplicateEmailErrorMessage
            );
        }

        [Test]
        public void
            ValidateEmailAddressesForAdminRegistration_adds_wrong_email_error_on_primary_if_primary_does_not_match_centre_and_secondary_is_null()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel(secondaryEmail: null);

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(
                () => centresService.DoesEmailMatchCentre(
                    DefaultPrimaryEmail,
                    DefaultCentreId
                )
            ).Returns(false);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultPrimaryEmail, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            modelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.PrimaryEmail),
                WrongEmailForCentreErrorMessage
            );
        }

        [Test]
        public void
            ValidateEmailAddressesForAdminRegistration_adds_wrong_email_error_on_secondary_if_neither_email_matches_centre_and_secondary_is_not_null()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).Returns(false);
            A.CallTo(
                () => centresService.DoesEmailMatchCentre(
                    DefaultPrimaryEmail,
                    DefaultCentreId
                )
            ).Returns(false);
            A.CallTo(
                () => centresService.DoesEmailMatchCentre(
                    DefaultSecondaryEmail,
                    DefaultCentreId
                )
            ).Returns(false);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultPrimaryEmail, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultSecondaryEmail, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            modelState[nameof(PersonalInformationViewModel.SecondaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.SecondaryEmail),
                WrongEmailForCentreErrorMessage
            );
        }

        [Test]
        public void
            ValidateEmailAddressesForAdminRegistration_does_not_add_wrong_email_error_if_primary_is_unique_and_matches_centre_and_secondary_is_null()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel(secondaryEmail: null);

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(
                () => centresService.DoesEmailMatchCentre(
                    DefaultPrimaryEmail,
                    DefaultCentreId
                )
            ).Returns(true);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultPrimaryEmail, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            modelState.ValidationState.Should().Be(ModelValidationState.Valid);
        }

        [Test]
        public void
            ValidateEmailAddressesForAdminRegistration_does_not_add_wrong_email_error_if_emails_are_unique_and_secondary_matches_centre()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).Returns(false);
            A.CallTo(
                () => centresService.DoesEmailMatchCentre(
                    DefaultPrimaryEmail,
                    DefaultCentreId
                )
            ).Returns(false);
            A.CallTo(
                () => centresService.DoesEmailMatchCentre(
                    DefaultSecondaryEmail,
                    DefaultCentreId
                )
            ).Returns(true);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultSecondaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultPrimaryEmail, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => centresService.DoesEmailMatchCentre(DefaultSecondaryEmail, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            modelState.ValidationState.Should().Be(ModelValidationState.Valid);
        }

        private PersonalInformationViewModel GetDefaultPersonalInformationViewModel(
            string primaryEmail = DefaultPrimaryEmail,
            string? secondaryEmail = DefaultSecondaryEmail
        )
        {
            return new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = DefaultCentreId,
                PrimaryEmail = primaryEmail,
                SecondaryEmail = secondaryEmail,
            };
        }

        private static void AssertModelStateErrorIsExpected(string modelProperty, string expectedErrorMessage)
        {
            var errorMessage = modelState[modelProperty].Errors.First().ErrorMessage;
            errorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }
    }
}
