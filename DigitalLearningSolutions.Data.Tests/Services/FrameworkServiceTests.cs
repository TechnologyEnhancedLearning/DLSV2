namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    public class FrameworkServiceTests
    {
        private FrameworkService frameworkService;
        private const int AdminId = 1;
        private SqlConnection connection;
        private IFrameworkService fakeFrameworkService;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<FrameworkService>>();
            frameworkService = new FrameworkService(connection, logger);
            fakeFrameworkService = A.Fake<IFrameworkService>();
        }
        [Test]
        public void GetFrameworksForAdminId_should_return_one_framework()
        {
            // When
            var result = frameworkService.GetFrameworksForAdminId(AdminId);

            // Then
            result.Should().HaveCount(1);
        }
        [Test]
        public void GetFrameworksForAdminId_should_return_no_frameworks_when_there_are_no_frameworks_for_AdminId()
        {
            // When
            var result = frameworkService.GetFrameworksForAdminId(10);

            // Then
            result.Should().HaveCount(0);
        }
        [Test]
        public void GetFrameworkDetailByFrameworkId_should_return_a_detail_framework_with_user_role_3_when_a_valid_id_is_passed_with_owner_adminId()
        {
            //Given
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
                PublishStatus = "Draft",
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 3,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined",
                Description = null
            };

            //When
            var result = frameworkService.GetFrameworkDetailByFrameworkId(2, AdminId);

            //Then
            result.Should().BeEquivalentTo(detailFramework);
        }
        [Test]
        public void GetFrameworkDetailByFrameworkId_should_return_a_detail_framework_with_user_role_0_when_a_valid_id_is_passed_with_no_relationship_adminId()
        {
            //Given
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
                PublishStatus = "Draft",
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 0,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined",
                Description = null
            };

            //When
            var result = frameworkService.GetFrameworkDetailByFrameworkId(2, 22);

            //Then
            result.Should().BeEquivalentTo(detailFramework);
        }
        [Test]
        public void GetFrameworkDetailByFrameworkId_should_return_null_when_an_invalid_id_is_passed()
        {
            //When
            var result = frameworkService.GetFrameworkDetailByFrameworkId(22, AdminId);

            //Then

            result.Should().BeNull();
        }
        [Test]
        public void GetBaseFrameworkByFrameworkId_should_return_a_base_framework_with_user_role_3_when_a_valid_id_is_passed_with_owner_adminId()
        {
            //Given
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
                PublishStatus = "Draft",
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 3,
                FrameworkReviewID = null
            };

            //When
            var result = frameworkService.GetBaseFrameworkByFrameworkId(2, AdminId);

            //Then
            result.Should().BeEquivalentTo(baseFramework);
        }
        [Test]
        public void GetBaseFrameworkByFrameworkId_should_return_a_base_framework_with_user_role_0_when_a_valid_id_is_passed_with_no_relationship_adminId()
        {
            //Given
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
                PublishStatus = "Draft",
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 0,
                FrameworkReviewID = null
            };

            //When
            var result = frameworkService.GetBaseFrameworkByFrameworkId(2, 22);

            //Then
            result.Should().BeEquivalentTo(baseFramework);
        }
        [Test]
        public void GetBaseFrameworkByFrameworkId_should_return_null_when_an_invalid_id_is_passed()
        {
            //When
            var result = frameworkService.GetBaseFrameworkByFrameworkId(22, AdminId);

            //Then

            result.Should().BeNull();
        }
        [Test]
        public void GetBrandedFrameworkByFrameworkId_should_return_a_branded_framework_with_user_role_3_when_a_valid_id_is_passed_with_owner_adminId()
        {
            //Given
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
                PublishStatus = "Draft",
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 3,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined"
            };

            //When
            var result = frameworkService.GetBrandedFrameworkByFrameworkId(2, AdminId);

            //Then
            result.Should().BeEquivalentTo(brandedFramework);
        }
        [Test]
        public void GetBrandedFrameworkByFrameworkId_should_return_a_branded_framework_with_user_role_0_when_a_valid_id_is_passed_with_no_relationship_adminId()
        {
            //Given
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
                PublishStatus = "Draft",
                UpdatedBy = "Kevin Whittaker (Developer)",
                UserRole = 0,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined"
            };

            //When
            var result = frameworkService.GetBrandedFrameworkByFrameworkId(2, 22);

            //Then
            result.Should().BeEquivalentTo(brandedFramework);
        }
        [Test]
        public void GetBrandedFrameworkByFrameworkId_should_return_null_when_an_invalid_id_is_passed()
        {
            //When
            var result = frameworkService.GetBrandedFrameworkByFrameworkId(22, AdminId);

            //Then

            result.Should().BeNull();
        }
        [Test]
        public void GetFrameworkByFrameworkName_should_have_count_of_1_when_match_exists()
        {
            //When
            var result = frameworkService.GetFrameworkByFrameworkName("Digital Capability Framework", AdminId);
            // Then
            result.Should().HaveCount(1);
        }
        [Test]
        public void GetFrameworkByFrameworkName_should_have_count_of_0_when_no_match_exists()
        {
            //When
            var result = frameworkService.GetFrameworkByFrameworkName("Non-existent Framework", AdminId);
            // Then
            result.Should().HaveCount(0);
        }
        [Test]
        public void GetFrameworkByFrameworkName_should_have_count_of_0_when_empty_string_passed()
        {
            //When
            var result = frameworkService.GetFrameworkByFrameworkName("", AdminId);
            // Then
            result.Should().HaveCount(0);
        }
        [Test]
        public void GetAllFrameworks_should_have_count_of_1()
        {
            //When
            var result = frameworkService.GetAllFrameworks(AdminId);
            // Then
            result.Should().HaveCount(1);
        }
        [Test]
        public void CreateFramework_should_return_empty_BrandedFramework_if_exists()
        {
            //Given
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
                PublishStatus = null,
                UpdatedBy = null,
                UserRole = 0,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined",
                Description = null
            };
            var nullFramework = new BrandedFramework();
            //When
            var result = frameworkService.CreateFramework(detailFramework, AdminId);

            //Then
            result.Should().BeEquivalentTo(nullFramework);
        }
        [Test]
        public void CreateFramework_should_return_empty_BrandedFramework_if_blank_name_passed()
        {
            //Given
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
                PublishStatus = null,
                UpdatedBy = null,
                UserRole = 0,
                FrameworkReviewID = null,
                Brand = "Local content",
                Category = "Undefined",
                Topic = "Undefined",
                Description = null
            };
            var nullFramework = new BrandedFramework();
            //When
            var result = frameworkService.CreateFramework(detailFramework, AdminId);

            //Then
            result.Should().BeEquivalentTo(nullFramework);
        }
        [Test]
        public void UpdateFrameworkBranding_should_return_BrandedFramework_if_valid()
        {
            //Given
            var frameworkId = 2;
            var brandId = 6;
            var categoryId = 1;
            var topicId = 1;

            //When
            var result = frameworkService.UpdateFrameworkBranding(frameworkId, brandId, categoryId, topicId, AdminId);

            //Then
            result.Should().BeOfType<BrandedFramework>();
        }
        [Test]
        public void UpdateFrameworkBranding_should_return_BrandedFramework_if_frameworkId_invalid()
        {
            //Given
            var frameworkId = 22;
            var brandId = 6;
            var categoryId = 1;
            var topicId = 1;

            //When
            var result = frameworkService.UpdateFrameworkBranding(frameworkId, brandId, categoryId, topicId, AdminId);

            //Then
            result.Should().BeNull();
        }
        [Test]
        public void UpdateFrameworkBranding_should_return_BrandedFramework_if_branding_invalid()
        {
            //Given
            var frameworkId = 2;
            var brandId = 0;
            var categoryId = 0;
            var topicId = 0;

            //When
            var result = frameworkService.UpdateFrameworkBranding(frameworkId, brandId, categoryId, topicId, AdminId);

            //Then
            result.Should().BeNull();
        }
        [Test]
        public void InsertCompetencyGroup_should_return_id_of_existing_group_if_name_matches()
        {
            //Given
            var groupName = "Technical proficiency";

            //When
            var result = frameworkService.InsertCompetencyGroup(groupName, AdminId);
            //Then
            result.Should().Be(4);
        }
        [Test]
        public void InsertCompetencyGroup_should_return_minus_2_if_name_is_blank()
        {
            //Given
            var groupName = "";

            //When
            var result = frameworkService.InsertCompetencyGroup(groupName, AdminId);
            //Then
            result.Should().Be(-2);
        }
    }
}
