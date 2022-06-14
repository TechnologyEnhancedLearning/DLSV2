namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class MultiPageFormDataDataServiceTests
    {
        private const string TestJson = "test json";

        private static readonly Guid TestGuid = Guid.NewGuid();

        private readonly MultiPageFormData expectedData = new MultiPageFormData
        {
            Id = 1,
            TempDataGuid = TestGuid,
            Json = TestJson,
            Feature = MultiPageFormDataFeature.AddNewCourse.Name,
            CreatedDate = new DateTime(2022, 06, 14, 14, 20, 12),
        };

        private SqlConnection connection = null!;

        private IMultiPageFormDataDataService multiPageFormDataDataService = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            multiPageFormDataDataService = new MultiPageFormDataDataService(connection);
        }

        [Test]
        public void GetMultiPageFormDataByGuidAndFeature_returns_expected_MultiPageFormData()
        {
            // Given
            using var transaction = new TransactionScope();
            InsertMultiPageFormData();

            // When
            var result = multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(
                TestGuid,
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().BeEquivalentTo(expectedData);
        }

        [Test]
        public void GetMultiPageFormDataByGuidAndFeature_returns_null_with_incorrect_guid()
        {
            // Given
            using var transaction = new TransactionScope();
            InsertMultiPageFormData();

            // When
            var result = multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(
                Guid.NewGuid(),
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetMultiPageFormDataByGuidAndFeature_returns_null_with_incorrect_feature()
        {
            // Given
            using var transaction = new TransactionScope();
            InsertMultiPageFormData();

            // When
            var result = multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(
                TestGuid,
                "incorrect feature"
            );

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void InsertMultiPageFormData_inserts_expected_values()
        {
            using var transaction = new TransactionScope();

            // When
            multiPageFormDataDataService.InsertMultiPageFormData(expectedData);
            var result = multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(
                TestGuid,
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().BeEquivalentTo(expectedData, options => options.Excluding(d => d.Id));
        }

        [Test]
        public void UpdateJsonByGuid_updates_expected_value()
        {
            // Given
            using var transaction = new TransactionScope();
            const string newJsonString = "new json";
            InsertMultiPageFormData();

            // When
            multiPageFormDataDataService.UpdateJsonByGuid(TestGuid, newJsonString);
            var result = multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(
                TestGuid,
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().NotBeNull();
            result!.Json.Should().BeEquivalentTo(newJsonString);
        }

        [Test]
        public void DeleteByGuid_deletes_expected_record()
        {
            // Given
            using var transaction = new TransactionScope();
            InsertMultiPageFormData();

            // When
            multiPageFormDataDataService.DeleteByGuid(TestGuid);
            var result = multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(
                TestGuid,
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().BeNull();
        }

        private void InsertMultiPageFormData()
        {
            connection.Execute(
                @"SET IDENTITY_INSERT dbo.MultiPageFormData ON
                    INSERT MultiPageFormData (ID, TempDataGuid, Json, Feature, CreatedDate)
                    VALUES (@Id, @TempDataGuid, @Json, @Feature, @CreatedDate)
                    SET IDENTITY_INSERT dbo.MultiPageFormData OFF",
                expectedData
            );
        }
    }
}
