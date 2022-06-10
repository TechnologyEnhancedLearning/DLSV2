namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
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
        private const string DefaultPrimaryEmailAllCaps = "PRIMARY@EMAIL.COM";
        private const string DefaultCentreSpecificEmail = "centre@email.com";
        private const string DefaultCentreSpecificEmailAllCaps = "CENTRE@EMAIL.COM";
        private const string DifferentCentreEmail = "different@email.com";
        private const string DefaultErrorMessage = "error message";

        private const string WrongEmailForCentreErrorMessage =
            "This email address does not match the one held by the centre";

        private const string DuplicateEmailErrorMessage =
            "A user with this email address is already registered; if this is you, " +
            "please log in and register at this centre via the My Account page";

        private static ModelStateDictionary modelState = null!;
        private ICentresDataService centresDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            userService = A.Fake<IUserService>();
            centresDataService = A.Fake<ICentresDataService>();
            modelState = new ModelStateDictionary();
        }

        [Test]
        public void
            ValidateEmailAddressesForDelegateRegistration_adds_no_new_error_messages_when_emails_are_already_invalid()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel(centreSpecificEmail: null);

            modelState.AddModelError(nameof(PersonalInformationViewModel.PrimaryEmail), DefaultErrorMessage);
            modelState.AddModelError(nameof(PersonalInformationViewModel.CentreSpecificEmail), DefaultErrorMessage);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForDelegateRegistration(model, modelState, userService);

            // Then
            A.CallTo(() => userService.EmailIsInUse(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(A<int>._)).MustNotHaveHappened();
            modelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            modelState[nameof(PersonalInformationViewModel.CentreSpecificEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(nameof(PersonalInformationViewModel.PrimaryEmail), DefaultErrorMessage);
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.CentreSpecificEmail),
                DefaultErrorMessage
            );
        }

        [Test]
        public void ValidateEmailAddressesForDelegateRegistration_adds_errors_on_emails_if_already_in_use()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(true);
            A.CallTo(() => userService.EmailIsInUse(DefaultCentreSpecificEmail)).Returns(true);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForDelegateRegistration(model, modelState, userService);

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultCentreSpecificEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(A<int>._)).MustNotHaveHappened();
            modelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            modelState[nameof(PersonalInformationViewModel.CentreSpecificEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.PrimaryEmail),
                DuplicateEmailErrorMessage
            );
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.CentreSpecificEmail),
                DuplicateEmailErrorMessage
            );
        }

        [Test]
        public void ValidateEmailAddressesForDelegateRegistration_adds_no_errors_on_emails_if_valid_and_unique()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(() => userService.EmailIsInUse(DefaultCentreSpecificEmail)).Returns(false);

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForDelegateRegistration(model, modelState, userService);

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultCentreSpecificEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(A<int>._)).MustNotHaveHappened();
            modelState.ValidationState.Should().Be(ModelValidationState.Valid);
        }

        [Test]
        public void
            ValidateEmailAddressesForAdminRegistration_adds_no_new_error_messages_when_emails_are_already_invalid()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel(centreSpecificEmail: null);

            modelState.AddModelError(nameof(PersonalInformationViewModel.PrimaryEmail), DefaultErrorMessage);
            modelState.AddModelError(nameof(PersonalInformationViewModel.CentreSpecificEmail), DefaultErrorMessage);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, DifferentCentreEmail));

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresDataService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            modelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            modelState[nameof(PersonalInformationViewModel.CentreSpecificEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(nameof(PersonalInformationViewModel.PrimaryEmail), DefaultErrorMessage);
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.CentreSpecificEmail),
                DefaultErrorMessage
            );
        }

        [Test]
        public void ValidateEmailAddressesForAdminRegistration_adds_errors_on_emails_if_already_in_use()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(true);
            A.CallTo(() => userService.EmailIsInUse(DefaultCentreSpecificEmail)).Returns(true);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, DifferentCentreEmail));

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresDataService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultCentreSpecificEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            modelState[nameof(PersonalInformationViewModel.PrimaryEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            modelState[nameof(PersonalInformationViewModel.CentreSpecificEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.PrimaryEmail),
                DuplicateEmailErrorMessage
            );
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.CentreSpecificEmail),
                DuplicateEmailErrorMessage
            );
        }

        [Test]
        public void
            ValidateEmailAddressesForAdminRegistration_adds_wrong_email_error_on_primary_if_primary_does_not_match_centre_and_centre_email_is_null()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel(centreSpecificEmail: null);

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .Returns((false, DifferentCentreEmail));

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresDataService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
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
            ValidateEmailAddressesForAdminRegistration_adds_wrong_email_error_on_centre_email_if_both_are_provided_and_neither_email_matches_centre()
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel();

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(() => userService.EmailIsInUse(DefaultCentreSpecificEmail)).Returns(false);
            A.CallTo(
                () => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId)
            ).Returns((false, DifferentCentreEmail));

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresDataService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultCentreSpecificEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            modelState[nameof(PersonalInformationViewModel.CentreSpecificEmail)].ValidationState.Should()
                .Be(ModelValidationState.Invalid);
            AssertModelStateErrorIsExpected(
                nameof(PersonalInformationViewModel.CentreSpecificEmail),
                WrongEmailForCentreErrorMessage
            );
        }

        [Test]
        [TestCase(DefaultPrimaryEmail)]
        [TestCase(DefaultPrimaryEmailAllCaps)]
        public void
            ValidateEmailAddressesForAdminRegistration_does_not_add_wrong_email_error_if_primary_matches_centre_and_centre_email_is_null(
                string primaryEmail
            )
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel(primaryEmail, null);

            A.CallTo(() => userService.EmailIsInUse(primaryEmail)).Returns(false);
            A.CallTo(
                () => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId)
            ).Returns((false, DefaultPrimaryEmail));

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresDataService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(primaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            modelState.ValidationState.Should().Be(ModelValidationState.Valid);
        }

        [Test]
        [TestCase(DefaultPrimaryEmail)]
        [TestCase(DefaultPrimaryEmailAllCaps)]
        public void
            ValidateEmailAddressesForAdminRegistration_does_not_add_wrong_email_error_if_primary_matches_centre_and_centre_email_is_not_null(
                string primaryEmail
            )
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel(primaryEmail);

            A.CallTo(() => userService.EmailIsInUse(primaryEmail)).Returns(false);
            A.CallTo(() => userService.EmailIsInUse(DefaultCentreSpecificEmail)).Returns(false);
            A.CallTo(
                () => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId)
            ).Returns((false, DefaultPrimaryEmail));

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresDataService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(primaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(DefaultCentreSpecificEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            modelState.ValidationState.Should().Be(ModelValidationState.Valid);
        }

        [Test]
        [TestCase(DefaultCentreSpecificEmail)]
        [TestCase(DefaultCentreSpecificEmailAllCaps)]
        public void
            ValidateEmailAddressesForAdminRegistration_does_not_add_wrong_email_error_if_centre_email_matches_centre(
                string centreSpecificEmail
            )
        {
            // Given
            var model = GetDefaultPersonalInformationViewModel(centreSpecificEmail: centreSpecificEmail);

            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).Returns(false);
            A.CallTo(() => userService.EmailIsInUse(centreSpecificEmail)).Returns(false);
            A.CallTo(
                () => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId)
            ).Returns((false, DefaultCentreSpecificEmail));

            // When
            RegistrationEmailValidator.ValidateEmailAddressesForAdminRegistration(
                model,
                modelState,
                userService,
                centresDataService
            );

            // Then
            A.CallTo(() => userService.EmailIsInUse(DefaultPrimaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.EmailIsInUse(centreSpecificEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.GetCentreAutoRegisterValues(DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            modelState.ValidationState.Should().Be(ModelValidationState.Valid);
        }

        private PersonalInformationViewModel GetDefaultPersonalInformationViewModel(
            string primaryEmail = DefaultPrimaryEmail,
            string? centreSpecificEmail = DefaultCentreSpecificEmail
        )
        {
            return new PersonalInformationViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Centre = DefaultCentreId,
                PrimaryEmail = primaryEmail,
                CentreSpecificEmail = centreSpecificEmail,
            };
        }

        private static void AssertModelStateErrorIsExpected(string modelProperty, string expectedErrorMessage)
        {
            var errorMessage = modelState[modelProperty].Errors.First().ErrorMessage;
            errorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }
    }
}
