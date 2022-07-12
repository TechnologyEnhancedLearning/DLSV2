namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    public class FrameworkServiceTests
    {
        private FrameworkService frameworkService;
        private const int ValidAdminId = 1;
        private const int InvalidAdminId = 10;
        private SqlConnection connection;
        private const int ValidFrameworkId = 2;
        private const int InvalidFrameworkId = 22;
        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<FrameworkService>>();
            frameworkService = new FrameworkService(connection, logger);
        }
        [Test]
        public void GetFrameworksForAdminId_should_return_three_framework()
        {
            // When
            var result = frameworkService.GetFrameworksForAdminId(ValidAdminId);

            // Then
            result.Should().HaveCount(3);
        }
        [Test]
        public void GetFrameworksForAdminId_should_return_no_frameworks_when_there_are_no_frameworks_for_AdminId()
        {
            // When
            var result = frameworkService.GetFrameworksForAdminId(InvalidAdminId);

            // Then
            result.Should().HaveCount(0);
        }
        [Test]
        public void GetFrameworkDetailByFrameworkId_should_return_a_detail_framework_with_user_role_3_when_a_valid_id_is_passed_with_owner_adminId()
        {
            // Given
            var detailFramework = new DetailFramework()
            {
                ID = 2,
                FrameworkName = "Digital Capability Framework",
                FrameworkConfig = null,
                OwnerAdminID = 1,
                Owner = "Kevin Whittaker (Developer)",
                BrandID = 6,
                CategoryID = 1,
                TopicID = 1,
                CreatedDate = DateTime.Parse("2020-12-10 11:58:52.590"),
                PublishStatusID = 1,
                UpdatedByAdminID = 1,
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 3,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined",
                Description = null
            };

            // When
            var result = frameworkService.GetFrameworkDetailByFrameworkId(ValidFrameworkId, ValidAdminId);

            // Then
            result.Should().BeEquivalentTo(detailFramework);
        }
        [Test]
        public void GetFrameworkDetailByFrameworkId_should_return_a_detail_framework_with_user_role_0_when_a_valid_id_is_passed_with_no_relationship_adminId()
        {
            // Given
            var detailFramework = new DetailFramework()
            {
                ID = 2,
                FrameworkName = "Digital Capability Framework",
                FrameworkConfig = null,
                OwnerAdminID = 1,
                Owner = "Kevin Whittaker (Developer)",
                BrandID = 6,
                CategoryID = 1,
                TopicID = 1,
                CreatedDate = DateTime.Parse("2020-12-10 11:58:52.590"),
                PublishStatusID = 1,
                UpdatedByAdminID = 1,
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 0,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined",
                Description = null
            };

            // When
            var result = frameworkService.GetFrameworkDetailByFrameworkId(ValidFrameworkId, InvalidAdminId);

            // Then
            result.Should().BeEquivalentTo(detailFramework);
        }
        [Test]
        public void GetFrameworkDetailByFrameworkId_should_return_null_when_an_invalid_id_is_passed()
        {
            // When
            var result = frameworkService.GetFrameworkDetailByFrameworkId(InvalidFrameworkId, ValidAdminId);

            // Then

            result.Should().BeNull();
        }
        [Test]
        public void GetBaseFrameworkByFrameworkId_should_return_a_base_framework_with_user_role_3_when_a_valid_id_is_passed_with_owner_adminId()
        {
            // Given
            var baseFramework = new BaseFramework()
            {
                ID = 2,
                FrameworkName = "Digital Capability Framework",
                OwnerAdminID = 1,
                Owner = "Kevin Whittaker (Developer)",
                BrandID = 6,
                CategoryID = 1,
                TopicID = 1,
                CreatedDate = DateTime.Parse("2020-12-10 11:58:52.590"),
                PublishStatusID = 1,
                UpdatedByAdminID = 1,
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 3,
                FrameworkReviewID = null
            };

            // When
            var result = frameworkService.GetBaseFrameworkByFrameworkId(ValidFrameworkId, ValidAdminId);

            // Then
            result.Should().BeEquivalentTo(baseFramework);
        }
        [Test]
        public void GetBaseFrameworkByFrameworkId_should_return_a_base_framework_with_user_role_0_when_a_valid_id_is_passed_with_no_relationship_adminId()
        {
            // Given
            var baseFramework = new BaseFramework()
            {
                ID = 2,
                FrameworkName = "Digital Capability Framework",
                OwnerAdminID = 1,
                Owner = "Kevin Whittaker (Developer)",
                BrandID = 6,
                CategoryID = 1,
                TopicID = 1,
                CreatedDate = DateTime.Parse("2020-12-10 11:58:52.590"),
                PublishStatusID = 1,
                UpdatedByAdminID = 1,
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 0,
                FrameworkReviewID = null
            };

            // When
            var result = frameworkService.GetBaseFrameworkByFrameworkId(2, 22);

            // Then
            result.Should().BeEquivalentTo(baseFramework);
        }
        [Test]
        public void GetBaseFrameworkByFrameworkId_should_return_null_when_an_invalid_id_is_passed()
        {
            // When
            var result = frameworkService.GetBaseFrameworkByFrameworkId(InvalidFrameworkId, ValidAdminId);

            // Then

            result.Should().BeNull();
        }
        [Test]
        public void GetBrandedFrameworkByFrameworkId_should_return_a_branded_framework_with_user_role_3_when_a_valid_id_is_passed_with_owner_adminId()
        {
            // Given
            var brandedFramework = new BrandedFramework()
            {
                ID = 2,
                FrameworkName = "Digital Capability Framework",
                OwnerAdminID = 1,
                Owner = "Kevin Whittaker (Developer)",
                BrandID = 6,
                CategoryID = 1,
                TopicID = 1,
                CreatedDate = DateTime.Parse("2020-12-10 11:58:52.590"),
                PublishStatusID = 1,
                UpdatedByAdminID = 1,
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 3,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined"
            };

            // When
            var result = frameworkService.GetBrandedFrameworkByFrameworkId(ValidFrameworkId, ValidAdminId);

            // Then
            result.Should().BeEquivalentTo(brandedFramework);
        }
        [Test]
        public void GetBrandedFrameworkByFrameworkId_should_return_a_branded_framework_with_user_role_0_when_a_valid_id_is_passed_with_no_relationship_adminId()
        {
            // Given
            var brandedFramework = new BrandedFramework()
            {
                ID = 2,
                FrameworkName = "Digital Capability Framework",
                OwnerAdminID = 1,
                Owner = "Kevin Whittaker (Developer)",
                BrandID = 6,
                CategoryID = 1,
                TopicID = 1,
                CreatedDate = DateTime.Parse("2020-12-10 11:58:52.590"),
                PublishStatusID = 1,
                UpdatedByAdminID = 1,
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 0,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined"
            };

            // When
            var result = frameworkService.GetBrandedFrameworkByFrameworkId(ValidFrameworkId, InvalidAdminId);

            // Then
            result.Should().BeEquivalentTo(brandedFramework);
        }
        [Test]
        public void GetBrandedFrameworkByFrameworkId_should_return_null_when_an_invalid_id_is_passed()
        {
            // When
            var result = frameworkService.GetBrandedFrameworkByFrameworkId(InvalidFrameworkId, ValidAdminId);

            // Then

            result.Should().BeNull();
        }
        [Test]
        public void GetFrameworkByFrameworkName_should_have_count_of_1_when_match_exists()
        {
            // When
            var result = frameworkService.GetFrameworkByFrameworkName("Digital Capability Framework", ValidAdminId);
            // Then
            result.Should().HaveCount(1);
        }
        [Test]
        public void GetFrameworkByFrameworkName_should_have_count_of_0_when_no_match_exists()
        {
            // When
            var result = frameworkService.GetFrameworkByFrameworkName("Non-existent Framework", ValidAdminId);
            // Then
            result.Should().HaveCount(0);
        }
        [Test]
        public void GetFrameworkByFrameworkName_should_have_count_of_0_when_empty_string_passed()
        {
            // When
            var result = frameworkService.GetFrameworkByFrameworkName("", ValidAdminId);
            // Then
            result.Should().HaveCount(0);
        }
        [Test]
        public void GetAllFrameworks_should_have_count_of_3()
        {
            // When
            var result = frameworkService.GetAllFrameworks(ValidAdminId);
            // Then
            result.Should().HaveCount(3);
        }
        [Test]
        public void CreateFramework_should_return_empty_BrandedFramework_if_exists()
        {
            // Given
            var detailFramework = new DetailFramework()
            {
                ID = 0,
                FrameworkName = "Digital Capability Framework",
                FrameworkConfig = null,
                OwnerAdminID = 1,
                Owner = null,
                BrandID = 6,
                CategoryID = 1,
                TopicID = 1,
                CreatedDate = default(DateTime),
                PublishStatusID = 1,
                UpdatedByAdminID = 1,
                UpdatedBy = null,
                UserRole = 0,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined",
                Description = null
            };
            var nullFramework = new BrandedFramework();
            // When
            var result = frameworkService.CreateFramework(detailFramework, ValidAdminId);

            // Then
            result.Should().BeEquivalentTo(nullFramework);
        }
        [Test]
        public void CreateFramework_should_return_empty_BrandedFramework_if_blank_name_passed()
        {
            // Given
            var detailFramework = new DetailFramework()
            {
                ID = 0,
                FrameworkName = "",
                FrameworkConfig = null,
                OwnerAdminID = 1,
                Owner = null,
                BrandID = 6,
                CategoryID = 1,
                TopicID = 1,
                CreatedDate = default(DateTime),
                PublishStatusID = 1,
                UpdatedByAdminID = 1,
                UpdatedBy = null,
                UserRole = 0,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined",
                Description = null
            };
            var nullFramework = new BrandedFramework();
            // When
            var result = frameworkService.CreateFramework(detailFramework, ValidAdminId);

            // Then
            result.Should().BeEquivalentTo(nullFramework);
        }
        [Test]
        public void UpdateFrameworkBranding_should_return_BrandedFramework_if_valid()
        {
            // Given
            var brandId = 6;
            var categoryId = 1;
            var topicId = 1;

            // When
            var result = frameworkService.UpdateFrameworkBranding(ValidFrameworkId, brandId, categoryId, topicId, ValidAdminId);

            // Then
            result.Should().BeOfType<BrandedFramework>();
        }
        [Test]
        public void UpdateFrameworkBranding_should_return_BrandedFramework_if_frameworkId_invalid()
        {
            // Given
            var brandId = 6;
            var categoryId = 1;
            var topicId = 1;

            // When
            var result = frameworkService.UpdateFrameworkBranding(InvalidFrameworkId, brandId, categoryId, topicId, ValidAdminId);

            // Then
            result.Should().BeNull();
        }
        [Test]
        public void UpdateFrameworkBranding_should_return_BrandedFramework_if_branding_invalid()
        {
            // Given
            var brandId = 0;
            var categoryId = 0;
            var topicId = 0;

            // When
            var result = frameworkService.UpdateFrameworkBranding(ValidFrameworkId, brandId, categoryId, topicId, ValidAdminId);

            // Then
            result.Should().BeNull();
        }
        [Test]
        public void InsertCompetencyGroup_should_return_id_of_existing_group_if_name_matches()
        {
            // Given
            var groupName = "Technical proficiency";

            // When
            var result = frameworkService.InsertCompetencyGroup(groupName, null, ValidAdminId);
            // Then
            result.Should().Be(4);
        }
        [Test]
        public void InsertCompetencyGroup_should_return_minus_2_if_name_is_blank()
        {
            // Given
            var groupName = "";

            // When
            var result = frameworkService.InsertCompetencyGroup(groupName, null, ValidAdminId);
            // Then
            result.Should().Be(-2);
        }
        [Test]
        public void InsertCompetency_should_return_id_of_existing_competency_if_name_matches()
        {
            // Given
            var name = "I can help others with technical issues";
            var description = "I can help others with technical issues";

            // When
            var result = frameworkService.InsertCompetency(name, description, ValidAdminId);
            // Then
            result.Should().Be(20);
        }
        [Test]
        public void InsertCompetency_should_return_minus_2_if_name_is_blank()
        {
            // Given
            var name = "";
            // When
            var result = frameworkService.InsertCompetency(name, null, ValidAdminId);
            // Then
            result.Should().Be(-2);
        }
        [Test]
        public void InsertFrameworkCompetency_should_return_id_of_existing_framework_competency_if_already_exists()
        {
            // Given
            int competencyId = 1;
            int? frameworkCompetencyGroupId = 1;
            // When
            var result = frameworkService.InsertFrameworkCompetency(competencyId, frameworkCompetencyGroupId, ValidAdminId, ValidFrameworkId);
            // Then
            result.Should().Be(1);
        }
        [Test]
        public void InsertFrameworkCompetency_should_return_minus2_if_parameters_are_invalid()
        {
            // Given
            int competencyId = 0;
            int? frameworkCompetencyGroupId = null;
            // When
            var result = frameworkService.InsertFrameworkCompetency(competencyId, frameworkCompetencyGroupId, ValidAdminId, ValidFrameworkId);
            // Then
            result.Should().Be(-2);
        }
        [Test]
        public void GetCollaboratorsForFrameworkId_should_return_list_of_CollaboratorDetail()
        {
            // When
            var result = frameworkService.GetCollaboratorsForFrameworkId(ValidFrameworkId);
            // Then
            result.Should().BeOfType<List<CollaboratorDetail>>();
        }
        [Test]
        public void AddCollaboratorToFramework_should_return_minus_3_if_collaborator_email_is_blank()
        {
            // Given
            string userEmail = "";
            bool canModify = false;

            // When
            var result = frameworkService.AddCollaboratorToFramework(ValidFrameworkId, userEmail, canModify);

            // Then
            result.Should().Be(-3);
        }
        [Test]
        public void GetFrameworkCompetencyGroups_returns_list_with_more_than_one_framework_competency_groups_for_valid_framework_id()
        {
            // When
            var result = frameworkService.GetFrameworkCompetencyGroups(ValidFrameworkId);

            // Then
            result.ToList().Count.Should().BeGreaterThan(0);
        }
        [Test]
        public void GetFrameworkCompetencyGroups_returns_list_with_no_framework_competency_groups_for_invalid_framework_id()
        {
            // When
            var result = frameworkService.GetFrameworkCompetencyGroups(InvalidFrameworkId);

            // Then
            result.ToList().Count.Should().Be(0);
        }
        [Test]
        public void GetFrameworkCompetenciesUngrouped_returns_list_with_no_framework_competencies_for_valid_framework_id()
        {
            // When
            var result = frameworkService.GetFrameworkCompetenciesUngrouped(InvalidFrameworkId);

            // Then
            result.ToList().Count.Should().Be(0);
        }
        [Test]
        public void GetFrameworkCompetenciesUngrouped_returns_list_with_no_framework_competencies_for_invalid_framework_id()
        {
            // When
            var result = frameworkService.GetFrameworkCompetenciesUngrouped(InvalidFrameworkId);

            // Then
            result.ToList().Count.Should().Be(0);
        }
        [Test]
        public void UpdateFrameworkName_returns_false_when_name_is_blank()
        {
            // Given
            string frameworkName = "";

            // When
            var result = frameworkService.UpdateFrameworkName(ValidFrameworkId, ValidAdminId, frameworkName);

            // Then
            result.Should().BeFalse();
        }
        [Test]
        public void UpdateFrameworkName_returns_false_when_framework_is_invalid()
        {
            // Given
            string frameworkName = "Digital Capability Framework";

            // When
            var result = frameworkService.UpdateFrameworkName(InvalidFrameworkId, ValidAdminId, frameworkName);

            // Then
            result.Should().BeFalse();
        }
        [Test]
        public void UpdateFrameworkName_returns_true_when_params_are_valid()
        {
            // Given
            string frameworkName = "Digital Capability Framework";

            // When
            var result = frameworkService.UpdateFrameworkName(ValidFrameworkId, ValidAdminId, frameworkName);

            // Then
            result.Should().BeTrue();
        }
        [Test]
        public void GetCompetencyGroupBaseById_returns_competency_group_base_when_id_is_valid()
        {
            // Given
            int id = 1;

            // When
            var result = frameworkService.GetCompetencyGroupBaseById(id);

            // Then
            result.Should().BeOfType<CompetencyGroupBase>();
        }
        [Test]
        public void GetCompetencyGroupBaseById_returns_null_when_id_is_invalid()
        {
            // Given
            int id = 999;

            // When
            var result = frameworkService.GetCompetencyGroupBaseById(id);

            // Then
            result.Should().BeNull();
        }
        [Test]
        public void GetFrameworkCompetencyById_returns_framework_competency_when_id_is_valid()
        {
            // Given
            int id = 1;

            // When
            var result = frameworkService.GetFrameworkCompetencyById(id);

            // Then
            result.Should().BeOfType<FrameworkCompetency>();
        }
        [Test]
        public void GetFrameworkCompetencyById_returns_null_when_id_is_invalid()
        {
            // Given
            int id = 999;

            // When
            var result = frameworkService.GetFrameworkCompetencyById(id);

            // Then
            result.Should().BeNull();
        }
        [Test]
        public void GetAllCompetencyQuestions_returns_list_of_more_than_0()
        {
            // When
            var result = frameworkService.GetAllCompetencyQuestions(ValidAdminId);

            // Then
            result.ToList().Count.Should().BeGreaterThan(0);
        }
        [Test]
        public void GetAssessmentQuestions_returns_list_of_more_than_0_when_framework_id_invalid()
        {
            // When
            var result = frameworkService.GetAssessmentQuestions(InvalidFrameworkId, ValidAdminId);

            // Then
            result.ToList().Count.Should().BeGreaterThan(0);
        }
        [Test]
        public void GetAssessmentQuestions_returns_list_of__10_when_framework_id_valid()
        {
            // When
            var result = frameworkService.GetAssessmentQuestions(InvalidFrameworkId, ValidAdminId);

            // Then
            result.ToList().Count.Should().Be(10);
        }
    }
}
