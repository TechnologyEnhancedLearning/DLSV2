namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        [Test]
        public void GetAdminUserById_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var returnedAdminUser = userDataService.GetAdminUserById(7);

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetAdminUserById_Returns_admin_user_category_name_all()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultCategoryNameAllAdminUser();

            // When
            var returnedAdminUser = userDataService.GetAdminUserById(11);

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetAdminUserByUsername_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var returnedAdminUser = userDataService.GetAdminUserByUsername("Username");

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetAdminUserByUsername_Returns_admin_user_category_name_all()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultCategoryNameAllAdminUser();

            // When
            var returnedAdminUser = userDataService.GetAdminUserByUsername("ebtnaxrbatnexr");

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetAdminUserByEmailAddress_Returns_admin_user()
        {
            // Given
            using (new TransactionScope())
            {
                var expectedAdminUser = UserTestHelper.GetDefaultAdminUser(resetPasswordId: 1);
                connection.Execute("UPDATE AdminUsers SET ResetPasswordID = 1 WHERE AdminID = 7");

                // When
                var returnedAdminUser = userDataService.GetAdminUserByEmailAddress("test@gmail.com");

                // Then
                returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
            }
        }

        [Test]
        public void GetAdminUserByEmailAddress_Returns_admin_user_category_name_all()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultCategoryNameAllAdminUser();

            // When
            var returnedAdminUser = userDataService.GetAdminUserByEmailAddress("ub.e@onlrxghciatsk");

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetAdminUsersByCentreId_gets_all_admins_at_centre()
        {
            // Given
            var expectedAdminIds = new List<int> { 7, 1408, 2464 };

            // When
            var admins = userDataService.GetAdminUsersByCentreId(2);
            var returnedIds = admins.Select(a => a.Id);

            // Then
            Assert.That(returnedIds.SequenceEqual(expectedAdminIds));
        }

        [Test]
        public void GetAdminUsersByCentreId_populates_correct_properties_on_admin()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var admin = userDataService.GetAdminUsersByCentreId(2).Single(a => a.Id == 7);

            // Then
            admin.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void UpdateAdminUser_updates_user()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                var firstName = "TestFirstName";
                var lastName = "TestLastName";
                var email = "test@email.com";

                // When
                userDataService.UpdateAdminUser(firstName, lastName, email, null, 7);
                var updatedUser = userDataService.GetAdminUserById(7)!;

                // Then
                updatedUser.FirstName.Should().BeEquivalentTo(firstName);
                updatedUser.LastName.Should().BeEquivalentTo(lastName);
                updatedUser.EmailAddress.Should().BeEquivalentTo(email);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetNumberOfActiveAdminsAtCentre_returns_expected_count()
        {
            // When
            var count = userDataService.GetNumberOfActiveAdminsAtCentre(2);

            // Then
            count.Should().Be(3);
        }

        [Test]
        public void UpdateAdminUserPermissions_updates_user()
        {
            using var transaction = new TransactionScope();
            try
            {
                // When
                userDataService.UpdateAdminUserPermissions(7, true, true, true, true, true, true, true, 1);
                var updatedUser = userDataService.GetAdminUserById(7)!;

                // Then
                updatedUser.IsCentreAdmin.Should().BeTrue();
                updatedUser.IsSupervisor.Should().BeTrue();
                updatedUser.IsNominatedSupervisor.Should().BeTrue();
                updatedUser.IsTrainer.Should().BeTrue();
                updatedUser.IsContentCreator.Should().BeTrue();
                updatedUser.IsContentManager.Should().BeTrue();
                updatedUser.ImportOnly.Should().BeTrue();
                updatedUser.CategoryId.Should().Be(1);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateAdminUserFailedLoginCount_updates_user()
        {
            using var transaction = new TransactionScope();
            try
            {
                // When
                userDataService.UpdateAdminUserFailedLoginCount(7, 3);
                var updatedUser = userDataService.GetAdminUserById(7)!;

                // Then
                updatedUser.FailedLoginCount.Should().Be(3);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void DeactivateAdminUser_updates_user()
        {
            using var transaction = new TransactionScope();
            var adminUser = UserTestHelper.GetDefaultAdminUser();

            try
            {
                // When
                userDataService.DeactivateAdmin(adminUser.Id);
                var updatedAdminUser = userDataService.GetAdminUserById(adminUser.Id)!;

                // Then
                updatedAdminUser.Active.Should().Be(false);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void ReactivateAdminUser_activates_user_and_resets_admin_permissions()
        {
            using var transaction = new TransactionScope();
            // Given
            const int adminId = 16;
            connection.SetAdminToInactiveWithCentreManagerAndSuperAdminPermissions(adminId);

            // When
            var deactivatedAdmin = userDataService.GetAdminUserById(adminId)!;
            userDataService.ReactivateAdmin(adminId);
            var reactivatedAdmin = userDataService.GetAdminUserById(adminId)!;

            // Then
            using (new AssertionScope())
            {
                deactivatedAdmin.Active.Should().Be(false);
                deactivatedAdmin.IsCentreManager.Should().Be(true);
                deactivatedAdmin.IsUserAdmin.Should().Be(true);
                reactivatedAdmin.Active.Should().Be(true);
                reactivatedAdmin.IsCentreManager.Should().Be(false);
                reactivatedAdmin.IsUserAdmin.Should().Be(false);
            }
        }

        [Test]
        public void DeleteAdmin_deletes_admin_record()
        {
            // Given
            const int adminId = 25;
            using var transaction = new TransactionScope();

            // When
            userDataService.DeleteAdminUser(adminId);
            var result = userDataService.GetAdminUserById(adminId);

            // Then
            result.Should().BeNull();
        }
    }
}
