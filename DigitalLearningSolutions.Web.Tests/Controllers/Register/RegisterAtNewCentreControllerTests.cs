namespace DigitalLearningSolutions.Web.Tests.Controllers.Register
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Register;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Primitives;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class RegisterAtNewCentreControllerTests
    {
        private const string IpAddress = "1.1.1.1";
        private const int UserId = 2;
        private ICentresDataService centresDataService = null!;
        private IConfiguration config = null!;
        private RegisterAtNewCentreController controller = null!;
        private IEmailVerificationService emailVerificationService = null!;
        private IFeatureManager featureManager = null!;
        private PromptsService promptsService = null!;
        private IRegistrationService registrationService = null!;
        private HttpRequest request = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            centresDataService = A.Fake<ICentresDataService>();
            registrationService = A.Fake<IRegistrationService>();
            userService = A.Fake<IUserService>();
            userDataService = A.Fake<IUserDataService>();
            promptsService = A.Fake<PromptsService>();
            featureManager = A.Fake<IFeatureManager>();
            supervisorDelegateService = A.Fake<ISupervisorDelegateService>();
            emailVerificationService = A.Fake<IEmailVerificationService>();
            config = A.Fake<IConfiguration>();
            request = A.Fake<HttpRequest>();

            controller = new RegisterAtNewCentreController(
                    centresDataService,
                    config,
                    emailVerificationService,
                    featureManager,
                    promptsService,
                    registrationService,
                    supervisorDelegateService,
                    userService,
                    userDataService
                )
                .WithDefaultContext()
                .WithMockRequestContext(request)
                .WithMockServices()
                .WithMockTempData()
                .WithMockUser(true, userId: UserId);
        }

        [Test]
        public void IndexGet_with_invalid_centreId_param_shows_error()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns(null);

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void IndexGet_with_valid_centreId_param_sets_data_correctly()
        {
            // Given
            const int centreId = 7;
            A.CallTo(() => centresDataService.GetCentreName(centreId)).Returns("Some centre");

            // When
            var result = controller.Index(centreId);

            // Then
            A.CallTo(() => centresDataService.GetCentreName(centreId)).MustHaveHappened(1, Times.Exactly);
            var data = controller.TempData.Peek<InternalDelegateRegistrationData>()!;
            data.Centre.Should().Be(centreId);
            data.IsCentreSpecificRegistration.Should().BeTrue();
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public void IndexGet_with_no_centreId_param_allows_normal_registration()
        {
            // When
            var result = controller.Index();

            // Then
            A.CallTo(() => centresDataService.GetCentreName(A<int>._)).MustNotHaveHappened();
            var data = controller.TempData.Peek<InternalDelegateRegistrationData>()!;
            data.Centre.Should().BeNull();
            data.IsCentreSpecificRegistration.Should().BeFalse();
            result.Should().BeRedirectToActionResult().WithActionName("PersonalInformation");
        }

        [Test]
        public void PersonalInformationPost_with_invalid_emails_fails_validation()
        {
            // Given
            const int centreId = 3;
            controller.TempData.Set(new InternalDelegateRegistrationData());
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var model = new InternalPersonalInformationViewModel
            {
                Centre = centreId,
                CentreSpecificEmail = "centre email",
            };
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        model.CentreSpecificEmail!,
                        centreId,
                        userAccount.Id
                    )
                )
                .Returns(true);
            A.CallTo(() => userService.GetUserById(userAccount.Id)).Returns(
                new UserEntity(userAccount, new List<AdminAccount>(), new[] { new DelegateAccount() })
            );

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        model.CentreSpecificEmail!,
                        centreId,
                        userAccount.Id
                    )
                )
                .MustHaveHappened();
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void PersonalInformationPost_with_null_centre_email_skips_email_validation()
        {
            // Given
            const int centreId = 3;
            controller.TempData.Set(new InternalDelegateRegistrationData());
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var model = new InternalPersonalInformationViewModel
            {
                Centre = centreId,
                CentreSpecificEmail = null,
            };
            A.CallTo(() => userService.GetUserById(userAccount.Id)).Returns(
                new UserEntity(userAccount, new List<AdminAccount>(), new[] { new DelegateAccount() })
            );

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    A<string>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
        }

        [Test]
        public void PersonalInformationPost_with_valid_emails_is_allowed()
        {
            // Given
            controller.TempData.Set(new InternalDelegateRegistrationData());
            var model = new InternalPersonalInformationViewModel
            {
                Centre = ControllerContextHelper.CentreId + 1,
                CentreSpecificEmail = "centre email",
            };
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        model.CentreSpecificEmail!,
                        model.Centre!.Value,
                        UserId
                    )
                )
                .Returns(false);

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        model.CentreSpecificEmail!,
                        model.Centre!.Value,
                        UserId
                    )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
        }

        [Test]
        public void PersonalInformationPost_allows_inactive_account_at_centre()
        {
            // Given
            controller.TempData.Set(new InternalDelegateRegistrationData());
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var inactiveDelegateAccount = UserTestHelper.GetDefaultDelegateAccount(
                centreId: ControllerContextHelper.CentreId,
                active: false
            );
            var model = new InternalPersonalInformationViewModel
            {
                Centre = ControllerContextHelper.CentreId,
                CentreSpecificEmail = null,
            };
            A.CallTo(() => userService.GetUserById(userAccount.Id)).Returns(
                new UserEntity(userAccount, new List<AdminAccount>(), new[] { inactiveDelegateAccount })
            );

            // When
            var result = controller.PersonalInformation(model);

            // Then
            A.CallTo(() => userService.GetUserById(userAccount.Id)).MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("LearnerInformation");
        }

        [Test]
        public void PersonalInformationPost_returns_validation_error_when_user_already_has_active_account()
        {
            // Given
            controller.TempData.Set(new InternalDelegateRegistrationData());
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var activeDelegateAccount = UserTestHelper.GetDefaultDelegateAccount(
                centreId: ControllerContextHelper.CentreId,
                active: true
            );
            var model = new InternalPersonalInformationViewModel
            {
                Centre = ControllerContextHelper.CentreId,
                CentreSpecificEmail = null,
            };
            A.CallTo(() => userService.GetUserById(userAccount.Id)).Returns(
                new UserEntity(userAccount, new List<AdminAccount>(), new[] { activeDelegateAccount })
            );

            // When
            var result = controller.PersonalInformation(model);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.GetUserById(userAccount.Id)).MustHaveHappenedOnceExactly();
                var errorMessage = result.As<ViewResult>().ViewData.ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0).ToList().First().First().ErrorMessage;
                errorMessage.Should().Be("You are already registered at this centre");
                controller.ModelState.IsValid.Should().BeFalse();
            }
        }

        [Test]
        public void LearnerInformationPost_updates_tempdata_correctly()
        {
            // Given
            const string answer1 = "answer1";
            const string answer2 = "answer2";
            const string answer3 = "answer3";
            const string answer4 = "answer4";
            const string answer5 = "answer5";
            const string answer6 = "answer6";

            controller.TempData.Set(new InternalDelegateRegistrationData { Centre = 1 });
            var model = new InternalLearnerInformationViewModel
            {
                Answer1 = answer1,
                Answer2 = answer2,
                Answer3 = answer3,
                Answer4 = answer4,
                Answer5 = answer5,
                Answer6 = answer6,
            };

            // When
            controller.LearnerInformation(model);

            // Then
            var data = controller.TempData.Peek<InternalDelegateRegistrationData>()!;
            using (new AssertionScope())
            {
                data.Answer1.Should().Be(answer1);
                data.Answer2.Should().Be(answer2);
                data.Answer3.Should().Be(answer3);
                data.Answer4.Should().Be(answer4);
                data.Answer5.Should().Be(answer5);
                data.Answer6.Should().Be(answer6);
            }
        }

        [Test]
        public async Task Summary_post_registers_delegate_with_expected_values()
        {
            // Given
            const string candidateNumber = "TN1";
            var data = RegistrationDataHelper.GetDefaultInternalDelegateRegistrationData();

            SetUpFakesForSuccessfulRegistration(candidateNumber, data);

            // When
            var result = await controller.SummaryPost();

            // Then
            A.CallTo(
                    () =>
                        registrationService.CreateDelegateAccountForExistingUser(
                            A<InternalDelegateRegistrationModel>.That.Matches(
                                d =>
                                    d.Centre == data.Centre &&
                                    d.Answer1 == data.Answer1 &&
                                    d.Answer2 == data.Answer2 &&
                                    d.Answer3 == data.Answer3 &&
                                    d.Answer4 == data.Answer4 &&
                                    d.Answer5 == data.Answer5 &&
                                    d.Answer6 == data.Answer6
                            ),
                            ControllerContextHelper.UserId,
                            IpAddress,
                            false,
                            null
                        )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
        }

        [Test]
        public async Task Summary_post_sends_verification_email_if_centre_email_is_unverified()
        {
            // Given
            const bool emailIsVerifiedForUser = false;
            const string centreSpecificEmail = "centre@email.com";

            SetUpFakesForSuccessfulRegistration();

            A.CallTo(() => emailVerificationService.AccountEmailIsVerifiedForUser(A<int>._, A<string>._))
                .Returns(emailIsVerifiedForUser);
            A.CallTo(() => userService.GetUserByEmailAddress(A<string>._))
                .Returns(UserTestHelper.GetDefaultUserAccount());
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>._,
                    A<string>._
                )
            ).DoesNothing();

            // When
            await controller.SummaryPost();

            // Then
            A.CallTo(() => emailVerificationService.AccountEmailIsVerifiedForUser(A<int>._, A<string>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.GetUserByEmailAddress(A<string>._)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>.That.Matches(
                        list => ListTestHelper.ListOfStringsMatch(
                            list,
                            new List<string> { centreSpecificEmail }
                        )
                    ),
                    A<string>._
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Summary_post_does_not_send_verification_email_if_centre_email_is_already_verified_for_user()
        {
            // Given
            const bool emailIsVerifiedForUser = true;

            SetUpFakesForSuccessfulRegistration();

            A.CallTo(() => emailVerificationService.AccountEmailIsVerifiedForUser(A<int>._, A<string>._))
                .Returns(emailIsVerifiedForUser);

            // When
            await controller.SummaryPost();

            // Then
            A.CallTo(() => emailVerificationService.AccountEmailIsVerifiedForUser(A<int>._, A<string>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userService.GetUserById(A<int>._)).MustNotHaveHappened();
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>._,
                    A<string>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public async Task Summary_post_returns_redirect_to_index_view_with_missing_centre()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultInternalDelegateRegistrationData(centre: null);
            controller.TempData.Set(data);

            // When
            var result = await controller.SummaryPost();

            // Then
            A.CallTo(
                    () => registrationService.CreateDelegateAccountForExistingUser(
                        A<InternalDelegateRegistrationModel>._,
                        A<int>._,
                        A<string>._,
                        A<bool>._,
                        A<int>._
                    )
                )
                .MustNotHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public async Task Summary_post_returns_500_error_with_unexpected_register_error()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultInternalDelegateRegistrationData();
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.CreateDelegateAccountForExistingUser(
                        A<InternalDelegateRegistrationModel>._,
                        A<int>._,
                        A<string>._,
                        A<bool>._,
                        A<int?>._
                    )
                )
                .Throws(new DelegateCreationFailedException(DelegateCreationError.UnexpectedError));
            A.CallTo(() => request.Headers).Returns(
                new HeaderDictionary(
                    new Dictionary<string, StringValues> { { "X-Forwarded-For", new StringValues(IpAddress) } }
                )
            );

            // When
            var result = await controller.SummaryPost();

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public async Task Summary_post_returns_500_error_with_active_account_already_exists_register_error()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultInternalDelegateRegistrationData();
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.CreateDelegateAccountForExistingUser(
                        A<InternalDelegateRegistrationModel>._,
                        A<int>._,
                        A<string>._,
                        A<bool>._,
                        A<int?>._
                    )
                )
                .Throws(new DelegateCreationFailedException(DelegateCreationError.ActiveAccountAlreadyExists));
            A.CallTo(() => request.Headers).Returns(
                new HeaderDictionary(
                    new Dictionary<string, StringValues> { { "X-Forwarded-For", new StringValues(IpAddress) } }
                )
            );

            // When
            var result = await controller.SummaryPost();

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public async Task Summary_post_returns_redirect_to_index_with_email_in_use_register_error()
        {
            // Given
            var data = RegistrationDataHelper.GetDefaultInternalDelegateRegistrationData();
            controller.TempData.Set(data);
            A.CallTo(
                    () => registrationService.CreateDelegateAccountForExistingUser(
                        A<InternalDelegateRegistrationModel>._,
                        A<int>._,
                        A<string>._,
                        A<bool>._,
                        A<int?>._
                    )
                )
                .Throws(new DelegateCreationFailedException(DelegateCreationError.EmailAlreadyInUse));
            A.CallTo(() => request.Headers).Returns(
                new HeaderDictionary(
                    new Dictionary<string, StringValues> { { "X-Forwarded-For", new StringValues(IpAddress) } }
                )
            );

            // When
            var result = await controller.SummaryPost();

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        private void SetUpFakesForSuccessfulRegistration(
            string? candidateNumber = null,
            InternalDelegateRegistrationData? data = null
        )
        {
            candidateNumber ??= "TN1";
            data ??= RegistrationDataHelper.GetDefaultInternalDelegateRegistrationData();

            controller.TempData.Set(data);

            A.CallTo(
                    () => registrationService.CreateDelegateAccountForExistingUser(
                        A<InternalDelegateRegistrationModel>._,
                        A<int>._,
                        A<string>._,
                        A<bool>._,
                        A<int>._
                    )
                )
                .Returns((candidateNumber, true, false));

            A.CallTo(() => request.Headers).Returns(
                new HeaderDictionary(
                    new Dictionary<string, StringValues> { { "X-Forwarded-For", new StringValues(IpAddress) } }
                )
            );
        }
    }
}
