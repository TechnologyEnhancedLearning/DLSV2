namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;

    public class RegistrationServiceTests
    {
        private const string ApproverEmail = "approver@email.com";
        private const string ApprovedIpPrefix = "123.456.789";
        private const string NewCandidateNumber = "TU67";
        private const string RefactoredSystemBaseUrl = "refactoredUrl";
        private const string OldSystemBaseUrl = "oldUrl";

        private ICentresDataService centresDataService = null!;
        private IConfiguration config = null!;
        private IEmailService emailService = null!;
        private IPasswordDataService passwordDataService = null!;
        private IPasswordResetService passwordResetService = null!;
        private IRegistrationDataService registrationDataService = null!;
        private IRegistrationService registrationService = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;
        private IUserDataService userDataService = null!;
        private INotificationDataService notificationDataService = null!;
        private ISupervisorService supervisorService = null!;
        private AdminUser? supervisorAdminUser;
        private DelegateUser? supervisorDelegateUser;

        [SetUp]
        public void Setup()
        {
            registrationDataService = A.Fake<IRegistrationDataService>();
            passwordDataService = A.Fake<IPasswordDataService>();
            passwordResetService = A.Fake<IPasswordResetService>();
            emailService = A.Fake<IEmailService>();
            centresDataService = A.Fake<ICentresDataService>();
            config = A.Fake<IConfiguration>();
            supervisorDelegateService = A.Fake<ISupervisorDelegateService>();
            notificationDataService = A.Fake<INotificationDataService>();
            userDataService = A.Fake<IUserDataService>();
            supervisorService = A.Fake<ISupervisorService>();
            supervisorAdminUser = A.Fake<AdminUser>();
            supervisorDelegateUser = A.Fake<DelegateUser>();
            
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(OldSystemBaseUrl);
            A.CallTo(() => config["AppRootPath"]).Returns(RefactoredSystemBaseUrl);

            A.CallTo(() => centresDataService.GetCentreIpPrefixes(RegistrationModelTestHelper.Centre))
                .Returns(new[] { ApprovedIpPrefix });
            A.CallTo(() => centresDataService.GetCentreManagerDetails(A<int>._))
                .Returns(("Test", "Approver", ApproverEmail));

            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._))
                .Returns(NewCandidateNumber);

            registrationService = new RegistrationService(
                registrationDataService,
                passwordDataService,
                passwordResetService,
                emailService,
                centresDataService,
                config,
                supervisorDelegateService,
                userDataService,
                notificationDataService,
                new NullLogger<RegistrationService>(),
                supervisorService
            );
        }

        [Test]
        public void Registering_delegate_with_approved_IP_registers_delegate_as_approved()
        {
            // Given
            const string clientIp = ApprovedIpPrefix + ".100";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            var (_, approved) = registrationService.RegisterDelegate(
                model,
                clientIp,
                false
            );

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegate(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved)
                        )
                )
                .MustHaveHappened();
            approved.Should().BeTrue();
        }

        [Test]
        public void Registering_delegate_on_localhost_registers_delegate_as_approved()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegate(model, "::1", false);

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegate(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved)
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_with_unapproved_IP_registers_delegate_as_unapproved()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegate(model, "987.654.321.100", false);

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegate(
                            A<DelegateRegistrationModel>.That.Matches(d => !d.Approved)
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_sends_approval_email_with_old_site_approval_link()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegate(model, string.Empty, false);

            // Then
            A.CallTo(
                () =>
                notificationDataService.GetAdminRecipientsForCentreNotification(
                    model.Centre,
                    4
                    )
                ).MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_sends_approval_email_with_refactored_tracking_system_link()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegate(model, string.Empty, true);

            // Then
            A.CallTo(
                () =>
                notificationDataService.GetAdminRecipientsForCentreNotification(
                    model.Centre,
                    4
                    )
                ).MustHaveHappened();
        }

        [Test]
        public void Registering_automatically_approved_does_not_send_email()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegate(model, "123.456.789.100", false);

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_set_password()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegate(model, string.Empty, false);

            // Then
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.PasswordHash
                    )
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_returns_candidate_number()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            var candidateNumber =
                registrationService.RegisterDelegate(model, string.Empty, false)
                    .candidateNumber;

            // Then
            candidateNumber.Should().Be(NewCandidateNumber);
        }

        [Test]
        public void Registering_delegate_should_add_CandidateId_to_all_SupervisorDelegate_records_found_by_email()
        {
            // Given
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);
            A.CallTo(
                    () => userDataService.GetDelegateUserByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.Centre
                    )
                )
                .Returns(new DelegateUser { Id = 777 });

            // When
            registrationService.RegisterDelegate(model, string.Empty, false, 999);

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>.That.IsSameSequenceAs(supervisorDelegateIds),
                    777
                )
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_not_update_any_SupervisorDelegate_records_if_none_found()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenNoPendingSupervisorDelegateRecordsForEmail();

            // When
            registrationService.RegisterDelegate(model, string.Empty, false, 999);

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void Error_when_registering_delegate_returns_error_code()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(() => registrationDataService.RegisterDelegate(model)).Returns("-1");

            // When
            Action act = () => registrationService.RegisterDelegate(model, string.Empty, false);

            // Then
            act.Should().Throw<DelegateCreationFailedException>();
        }

        [Test]
        public void Error_when_registering_delegate_fails_fast()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(() => registrationDataService.RegisterDelegate(model)).Returns("-1");

            // When
            Action act = () => registrationService.RegisterDelegate(model, string.Empty, false);

            // Then
            act.Should().Throw<DelegateCreationFailedException>();
            A.CallTo(
                () =>
                    emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_delegate_calls_data_service_to_set_prn()
        {
            // Given
            const string clientIp = ApprovedIpPrefix + ".100";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            var (_, approved) = registrationService.RegisterDelegate(
                model,
                clientIp,
                false
            );

            // Then
            A.CallTo(
                    () =>
                        userDataService.UpdateDelegateProfessionalRegistrationNumber(
                            A<int>._,
                            model.ProfessionalRegistrationNumber,
                            true
                        )
                )
                .MustHaveHappenedOnceExactly();
            approved.Should().BeTrue();
        }

        [Test]
        public void Registering_admin_delegate_registers_delegate_as_approved()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(model, 1, true);

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegate(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved)
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void Registering_admin_delegate_does_not_send_email()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(model, 1, true);

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_admin_delegate_should_set_password()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(model, 1, true);

            // Then
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.PasswordHash
                    )
            ).MustHaveHappened();
        }

        [Test]
        public void RegisterCentreManager_calls_all_relevant_registration_methods()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(model, 1, true);

            // Then
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => registrationDataService.RegisterAdmin(model, true))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.SetCentreAutoRegistered(RegistrationModelTestHelper.Centre))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void Error_in_RegisterCentreManager_throws_exception()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._)).Returns("-1");

            // When
            Action act = () => registrationService.RegisterCentreManager(model, 1, true);

            // Then
            act.Should().Throw<Exception>();
        }

        [Test]
        public void Error_in_RegisterCentreManager_fails_fast()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._)).Returns("-1");

            // When
            Action act = () => registrationService.RegisterCentreManager(model, 1, true);

            // Then
            act.Should().Throw<Exception>();
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustNotHaveHappened();
            A.CallTo(() => registrationDataService.RegisterAdmin(model, A<bool>._))
                .MustNotHaveHappened();
            A.CallTo(() => centresDataService.SetCentreAutoRegistered(RegistrationModelTestHelper.Centre))
                .MustNotHaveHappened();
        }

        [Test]
        public void RegisterCentreManager_calls_data_service_to_set_prn()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(model, 1, true);

            // Then
            A.CallTo(
                    () =>
                        userDataService.UpdateDelegateProfessionalRegistrationNumber(
                            A<int>._,
                            model.ProfessionalRegistrationNumber,
                            true
                        )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RegisterDelegateByCentre_sets_password_if_passwordHash_not_null()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(() => registrationDataService.RegisterDelegate(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, "");

            // Then
            A.CallTo(() => registrationDataService.RegisterDelegate(model)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                    () => passwordDataService.SetPasswordByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.PasswordHash
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void RegisterDelegateByCentre_does_not_set_password_if_passwordHash_is_null()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            model.PasswordHash = null;
            A.CallTo(() => registrationDataService.RegisterDelegate(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, "");

            // Then
            A.CallTo(() => registrationDataService.RegisterDelegate(model)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_schedules_welcome_email_if_notify_date_set()
        {
            // Given
            const string baseUrl = "base.com";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                passwordHash: null,
                notifyDate: new DateTime(2200, 1, 1)
            );
            A.CallTo(() => registrationDataService.RegisterDelegate(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, baseUrl);

            // Then
            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    model.Email,
                    NewCandidateNumber,
                    baseUrl,
                    model.NotifyDate!.Value,
                    "RegisterDelegateByCentre_Refactor"
                )
            ).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void RegisterDelegateByCentre_does_not_schedule_welcome_email_if_notify_date_not_set()
        {
            // Given
            const string baseUrl = "base.com";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(() => registrationDataService.RegisterDelegate(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, baseUrl);

            // Then
            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    A<string>._,
                    NewCandidateNumber,
                    A<string>._,
                    A<DateTime>._,
                    A<string>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_should_add_CandidateId_to_all_SupervisorDelegate_records_found_by_email()
        {
            // Given
            const string baseUrl = "base.com";
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);
            A.CallTo(() => registrationDataService.RegisterDelegate(model))
                .Returns(NewCandidateNumber);
            A.CallTo(
                    () => userDataService.GetDelegateUserByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.Centre
                    )
                )
                .Returns(new DelegateUser { Id = 777 });

            // When
            registrationService.RegisterDelegateByCentre(model, baseUrl);

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>.That.IsSameSequenceAs(supervisorDelegateIds),
                    777
                )
            ).MustHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_calls_data_service_to_set_prn()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(() => registrationDataService.RegisterDelegate(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, "");

            // Then
            A.CallTo(
                    () =>
                        userDataService.UpdateDelegateProfessionalRegistrationNumber(
                            A<int>._,
                            model.ProfessionalRegistrationNumber,
                            true
                        )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void PromoteDelegateToAdmin_throws_AdminCreationFailedException_if_delegate_has_no_first_name()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(firstName: null);
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, false);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(delegateUser);

            // When
            var result = Assert.Throws<AdminCreationFailedException>(
                () => registrationService.PromoteDelegateToAdmin(adminRoles, 1, 1, supervisorAdminUser, supervisorDelegateUser)
            );

            // Then
            result.Error.Should().Be(AdminCreationError.UnexpectedError);
        }

        [Test]
        public void PromoteDelegateToAdmin_throws_AdminCreationFailedException_if_delegate_has_no_email()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: null);
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, false);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(delegateUser);

            // When
            var result = Assert.Throws<AdminCreationFailedException>(
                () => registrationService.PromoteDelegateToAdmin(adminRoles, 1, 1, supervisorAdminUser, supervisorDelegateUser)
            );

            // Then
            result.Error.Should().Be(AdminCreationError.UnexpectedError);
        }

        [Test]
        public void
            PromoteDelegateToAdmin_throws_email_in_use_AdminCreationFailedException_if_active_admin_already_exists()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, false);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(adminUser);

            // When
            registrationService.PromoteDelegateToAdmin(adminRoles, 0, 1, supervisorAdminUser, supervisorDelegateUser);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => userDataService.UpdateAdminUserPermissions(
                        adminUser.Id,
                        adminRoles.IsCentreAdmin || adminUser.IsCentreAdmin,
                        adminRoles.IsSupervisor || adminRoles.IsSupervisor,
                        adminRoles.IsNominatedSupervisor || adminUser.IsNominatedSupervisor,
                        adminRoles.IsTrainer || adminUser.IsTrainer,
                        adminRoles.IsContentCreator || adminUser.IsContentCreator,
                        adminRoles.IsContentManager || adminUser.IsContentManager,
                        adminRoles.ImportOnly || adminUser.ImportOnly,
                        adminUser.CategoryId,
                        adminRoles.IsCentreManager || adminUser.IsCentreManager
                    )
                ).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            PromoteDelegateToAdmin_throws_email_in_use_AdminCreationFailedException_if_inactive_admin_at_different_centre_already_exists()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var adminUser = UserTestHelper.GetDefaultAdminUser(centreId: 3, active: false);
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, false);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(adminUser);

            // When
            var result = Assert.Throws<AdminCreationFailedException>(
                () => registrationService.PromoteDelegateToAdmin(adminRoles, 1, 1, supervisorAdminUser, supervisorDelegateUser)
            );

            // Then
            using (new AssertionScope())
            {
                UpdateToExistingAdminAccountMustNotHaveHappened();
                result.Error.Should().Be(AdminCreationError.EmailAlreadyInUse);
            }
        }

        [Test]
        public void PromoteDelegateToAdmin_updates_existing_admin_if_inactive_admin_at_same_centre_already_exists()
        {
            // Given
            const int categoryId = 1;
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var adminUser = UserTestHelper.GetDefaultAdminUser(active: false);
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, true);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(adminUser);

            // When
            registrationService.PromoteDelegateToAdmin(adminRoles, categoryId, 1, supervisorAdminUser, supervisorDelegateUser);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.ReactivateAdmin(adminUser.Id)).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => userDataService.UpdateAdminUser(
                        delegateUser.FirstName!,
                        delegateUser.LastName,
                        delegateUser.EmailAddress!,
                        delegateUser.ProfileImage,
                        adminUser.Id
                    )
                ).MustHaveHappenedOnceExactly();
                A.CallTo(() => passwordDataService.SetPasswordByAdminId(adminUser.Id, delegateUser.Password!))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => userDataService.UpdateAdminUserPermissions(
                        adminUser.Id,
                        adminRoles.IsCentreAdmin,
                        adminRoles.IsSupervisor,
                        adminRoles.IsNominatedSupervisor,
                        adminRoles.IsTrainer,
                        adminRoles.IsContentCreator,
                        adminRoles.IsContentManager,
                        adminRoles.ImportOnly,
                        categoryId,
                        adminRoles.IsCentreManager
                    )
                ).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void PromoteDelegateToAdmin_calls_data_service_with_expected_value()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, true);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(null);

            // When
            registrationService.PromoteDelegateToAdmin(adminRoles, 1, 1, supervisorAdminUser, supervisorDelegateUser);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => registrationDataService.RegisterAdmin(
                        A<AdminRegistrationModel>.That.Matches(
                            a =>
                                a.FirstName == delegateUser.FirstName &&
                                a.LastName == delegateUser.LastName &&
                                a.Email == delegateUser.EmailAddress &&
                                a.Centre == delegateUser.CentreId &&
                                a.PasswordHash == delegateUser.Password &&
                                a.Active &&
                                a.Approved &&
                                a.IsCentreAdmin == adminRoles.IsCentreAdmin &&
                                a.IsCentreManager == adminRoles.IsCentreManager &&
                                a.IsContentManager == adminRoles.IsContentManager &&
                                a.ImportOnly == adminRoles.IsCmsAdministrator &&
                                a.IsContentCreator == adminRoles.IsContentCreator &&
                                a.IsTrainer == adminRoles.IsTrainer &&
                                a.IsSupervisor == adminRoles.IsSupervisor
                        ),
                        false
                    )
                ).MustHaveHappened();
                UpdateToExistingAdminAccountMustNotHaveHappened();

                A.CallTo(
                    () => supervisorService.UpdateNotificationSent(
                        A<int>._)
                ).MustHaveHappened();

                A.CallTo(
                    () => emailService.SendEmail(A<Email>._)
                ).MustHaveHappened();
            }
        }

        private void GivenNoPendingSupervisorDelegateRecordsForEmail()
        {
            A.CallTo(
                    () => supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailAndCentre(
                        A<int>._,
                        A<string>._
                    )
                )
                .Returns(new List<SupervisorDelegate>());
        }

        private void GivenPendingSupervisorDelegateIdsForEmailAre(IEnumerable<int> supervisorDelegateIds)
        {
            var supervisorDelegates = supervisorDelegateIds.Select(id => new SupervisorDelegate { ID = id });
            A.CallTo(
                    () => supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailAndCentre(
                        A<int>._,
                        A<string>._
                    )
                )
                .Returns(supervisorDelegates);
        }

        private void UpdateToExistingAdminAccountMustNotHaveHappened()
        {
            A.CallTo(() => userDataService.ReactivateAdmin(A<int>._)).MustNotHaveHappened();
            A.CallTo(
                () => userDataService.UpdateAdminUser(
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<byte[]>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            A.CallTo(() => passwordDataService.SetPasswordByAdminId(A<int>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(
                () => userDataService.UpdateAdminUserPermissions(
                    A<int>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<int>._,
                    A<bool>._
                )
            ).MustNotHaveHappened();
        }
    }
}
