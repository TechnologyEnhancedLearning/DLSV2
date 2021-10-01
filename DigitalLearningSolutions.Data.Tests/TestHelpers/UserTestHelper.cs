﻿namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;

    public static class UserTestHelper
    {
        public static DelegateUser GetDefaultDelegateUser(
            int id = 2,
            int centreId = 2,
            string centreName = "North West Boroughs Healthcare NHS Foundation Trust",
            bool centreActive = true,
            DateTime? dateRegistered = null,
            string firstName = "Firstname",
            string lastName = "Test",
            string? emailAddress = "email@test.com",
            string password = "password",
            int? resetPasswordId = null,
            bool approved = true,
            string candidateNumber = "SV1234",
            int jobGroupId = 1,
            string? jobGroupName = "Nursing / midwifery",
            string? answer1 = null,
            string? answer2 = null,
            string? answer3 = null,
            string? answer4 = null,
            string? answer5 = null,
            string? answer6 = null,
            string? aliasId = null,
            bool active = true
        )
        {
            dateRegistered ??= DateTime.Parse("2010-09-22 06:52:09.080");
            return new DelegateUser
            {
                Id = id,
                CentreId = centreId,
                CentreName = centreName,
                CentreActive = centreActive,
                DateRegistered = dateRegistered,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                Password = password,
                ResetPasswordId = resetPasswordId,
                Approved = approved,
                CandidateNumber = candidateNumber,
                JobGroupId = jobGroupId,
                JobGroupName = jobGroupName,
                Answer1 = answer1,
                Answer2 = answer2,
                Answer3 = answer3,
                Answer4 = answer4,
                Answer5 = answer5,
                Answer6 = answer6,
                AliasId = aliasId,
                Active = active
            };
        }

        public static AdminUser GetDefaultAdminUser(
            int id = 7,
            int centreId = 2,
            string centreName = "North West Boroughs Healthcare NHS Foundation Trust",
            bool centreActive = true,
            bool active = true,
            bool approved = true,
            string firstName = "forename",
            string lastName = "surname",
            string emailAddress = "test@gmail.com",
            string password = "Password",
            int? resetPasswordId = null,
            bool isCentreAdmin = true,
            bool isCentreManager = true,
            bool isContentCreator = false,
            bool isContentManager = true,
            bool publishToAll = true,
            bool summaryReports = false,
            bool isUserAdmin = true,
            int categoryId = 1,
            string? categoryName = "Undefined",
            bool isSupervisor = true,
            bool isTrainer = true,
            bool isFrameworkDeveloper = true,
            bool importOnly = true,
            int failedLoginCount = 0
        )
        {
            return new AdminUser
            {
                Id = id,
                CentreId = centreId,
                CentreName = centreName,
                CentreActive = centreActive,
                Active = active,
                Approved = approved,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                Password = password,
                ResetPasswordId = resetPasswordId,
                IsCentreAdmin = isCentreAdmin,
                IsCentreManager = isCentreManager,
                IsContentCreator = isContentCreator,
                IsContentManager = isContentManager,
                PublishToAll = publishToAll,
                SummaryReports = summaryReports,
                IsUserAdmin = isUserAdmin,
                CategoryId = categoryId,
                CategoryName = categoryName,
                IsSupervisor = isSupervisor,
                IsTrainer = isTrainer,
                IsFrameworkDeveloper = isFrameworkDeveloper,
                ImportOnly = importOnly,
                FailedLoginCount = failedLoginCount,
            };
        }

        public static AdminUser GetDefaultCategoryNameAllAdminUser()
        {
            return GetDefaultAdminUser(
                centreName: "Guy's and St Thomas' NHS Foundation Trust",
                id: 11,
                centreId: 59,
                firstName: "xxxxxxx",
                lastName: "xxxxxx",
                emailAddress: "ub.e@onlrxghciatsk",
                password: "AKqPfVDoD0/Ri1sRMHn3VQPU4DafOB/9cKp9XPDGyHpO2GB00G0A/3Ss68XPV6fbEg==",
                isContentManager: false,
                publishToAll: false,
                isUserAdmin: false,
                categoryId: 0,
                categoryName: "All",
                isSupervisor: false,
                isTrainer: false,
                isFrameworkDeveloper: false,
                importOnly: false,
                failedLoginCount: 5,
                active: true
            );
        }

        public static DelegateRegistrationModel GetDefaultDelegateRegistrationModel(
            string firstName = "FirstName",
            string lastName = "Test",
            string email = "email@test.com",
            int centre = 2,
            int jobGroup = 3,
            string passwordHash = "APasswordHash",
            string? answer1 = "Answer1",
            string? answer2 = "Answer2",
            string? answer3 = "Answer3",
            string? answer4 = "Answer4",
            string? answer5 = "Answer5",
            string? answer6 = "Answer6",
            bool approved = false
        )
        {
            return new DelegateRegistrationModel(
                firstName,
                lastName,
                email,
                centre,
                jobGroup,
                passwordHash,
                answer1,
                answer2,
                answer3,
                answer4,
                answer5,
                answer6
            ) { Approved = approved };
        }

        public static RegistrationModel GetDefaultRegistrationModel(
            string firstName = "FirstName",
            string lastName = "Test",
            string email = "email@test.com",
            int centre = 2,
            int jobGroup = 3,
            string passwordHash = "APasswordHash",
            bool approved = false
        )
        {
            return new RegistrationModel(
                    firstName,
                    lastName,
                    email,
                    centre,
                    jobGroup,
                    passwordHash
                )
                { Approved = approved };
        }

        public static async Task<DelegateUser> GetDelegateUserByCandidateNumberAsync(
            this DbConnection connection,
            string candidateNumber
        )
        {
            var users = await connection.QueryAsync<DelegateUser>(
                @"SELECT
                        FirstName,
                        LastName,
                        EmailAddress,
                        CentreID,
                        Password,
                        Approved,
                        Answer1,
                        Answer2,
                        Answer3,
                        Answer4,
                        Answer5,
                        Answer6
                    FROM Candidates
                    WHERE CandidateNumber = @candidateNumber",
                new { candidateNumber }
            );

            return users.Single();
        }

        public static AccountDetailsData GetDefaultAccountDetailsData(
            int? adminId = null,
            int? delegateId = null,
            string password = "password",
            string firstName = "firstname",
            string surname = "lastname",
            string email = "email@email.com",
            byte[]? profileImage = null
        )
        {
            return new AccountDetailsData(
                adminId,
                delegateId,
                password,
                firstName,
                surname,
                email,
                profileImage
            );
        }

        public static CentreAnswersData GetDefaultCentreAnswersData(
            int centreId = 1,
            int jobGroupId = 1,
            string? answer1 = null,
            string? answer2 = null,
            string? answer3 = null,
            string? answer4 = null,
            string? answer5 = null,
            string? answer6 = null
        )
        {
            return new CentreAnswersData(centreId, jobGroupId, answer1, answer2, answer3, answer4, answer5, answer6);
        }
    }
}
