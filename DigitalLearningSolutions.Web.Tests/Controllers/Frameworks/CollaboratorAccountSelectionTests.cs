namespace DigitalLearningSolutions.Web.Tests.Controllers.Frameworks
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Constants;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public partial class FrameworkControllerTests
    {
        private const int FrameworkId = 7;
        private const string CollaboratorEmail = "admin@example.com";

        private static readonly List<AdminAccountSummary> TwoAdminAccounts = new List<AdminAccountSummary>
        {
            new AdminAccountSummary { AdminId = 11, Email = CollaboratorEmail, CentreId = 1, CentreName = "Centre A" },
            new AdminAccountSummary { AdminId = 12, Email = CollaboratorEmail, CentreId = 2, CentreName = "Centre B" },
        };

        private void GivenFrameworkWithUserRole(int userRole)
        {
            A.CallTo(() => frameworkService.GetBaseFrameworkByFrameworkId(FrameworkId, AdminId))
                .Returns(new BaseFramework { ID = FrameworkId, FrameworkName = "Framework", UserRole = userRole });
        }

        [Test]
        public void AddCollaborator_shows_account_selection_when_email_has_more_than_one_admin_account()
        {
            // Given
            GivenFrameworkWithUserRole(2);
            A.CallTo(() => frameworkService.AddCollaboratorToFramework(FrameworkId, CollaboratorEmail, true, null))
                .Returns(CollaboratorResultCodes.AdminAccountSelectionRequired);
            A.CallTo(() => userService.GetActiveAdminAccountsByEmail(CollaboratorEmail)).Returns(TwoAdminAccounts);

            // When
            var result = controller.AddCollaborator("Edit", CollaboratorEmail, true, FrameworkId);

            // Then
            var model = result.Should().BeViewResult().WithViewName("Developer/SelectCollaboratorAccount")
                .ModelAs<SelectAdminAccountViewModel>();
            model.AdminAccounts.Should().BeEquivalentTo(TwoAdminAccounts);
            model.Email.Should().Be(CollaboratorEmail);
            model.CanModify.Should().BeTrue();
            A.CallTo(() => frameworkNotificationService.SendFrameworkCollaboratorInvite(A<int>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void AddCollaborator_returns_forbidden_when_user_cannot_modify_framework()
        {
            // Given
            GivenFrameworkWithUserRole(1);

            // When
            var result = controller.AddCollaborator("Edit", CollaboratorEmail, true, FrameworkId);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(403);
            A.CallTo(() => frameworkService.AddCollaboratorToFramework(A<int>._, A<string>._, A<bool>._, A<int?>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void SelectCollaboratorAccount_adds_selected_admin_account_and_sends_invite()
        {
            // Given
            GivenFrameworkWithUserRole(2);
            A.CallTo(() => frameworkService.AddCollaboratorToFramework(FrameworkId, CollaboratorEmail, false, 12))
                .Returns(99);
            var model = new SelectAdminAccountViewModel { Email = CollaboratorEmail, CanModify = false, SelectedAdminId = 12 };

            // When
            var result = controller.SelectCollaboratorAccount("Edit", FrameworkId, model);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("AddCollaborators");
            A.CallTo(() => frameworkNotificationService.SendFrameworkCollaboratorInvite(99, AdminId))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void SelectCollaboratorAccount_redisplays_page_when_no_account_selected()
        {
            // Given
            GivenFrameworkWithUserRole(2);
            A.CallTo(() => userService.GetActiveAdminAccountsByEmail(CollaboratorEmail)).Returns(TwoAdminAccounts);
            var model = new SelectAdminAccountViewModel { Email = CollaboratorEmail, CanModify = true };
            controller.ModelState.AddModelError(nameof(model.SelectedAdminId), SelectAdminAccountViewModel.SelectionRequiredErrorMessage);

            // When
            var result = controller.SelectCollaboratorAccount("Edit", FrameworkId, model);

            // Then
            result.Should().BeViewResult().WithViewName("Developer/SelectCollaboratorAccount")
                .ModelAs<SelectAdminAccountViewModel>().AdminAccounts.Should().HaveCount(2);
            A.CallTo(() => frameworkService.AddCollaboratorToFramework(A<int>._, A<string>._, A<bool>._, A<int?>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void SelectCollaboratorAccount_shows_error_when_selected_account_is_not_valid_for_email()
        {
            // Given
            GivenFrameworkWithUserRole(2);
            controller.WithMockTempData();
            A.CallTo(() => frameworkService.AddCollaboratorToFramework(FrameworkId, CollaboratorEmail, true, 999))
                .Returns(CollaboratorResultCodes.NoActiveAdminAccount);
            var model = new SelectAdminAccountViewModel { Email = CollaboratorEmail, CanModify = true, SelectedAdminId = 999 };

            // When
            var result = controller.SelectCollaboratorAccount("Edit", FrameworkId, model);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("AddCollaborators");
            controller.TempData["FrameworkError"].Should().Be("The email address must match a registered DLS Admin account");
            A.CallTo(() => frameworkNotificationService.SendFrameworkCollaboratorInvite(A<int>._, A<int>._))
                .MustNotHaveHappened();
        }
    }
}
