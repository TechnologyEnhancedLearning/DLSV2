namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class MultiPageFormDataServiceTests
    {
        private readonly MultiPageFormData dataInDb = new MultiPageFormData
        {
            Id = 2,
            TempDataGuid = Guid.NewGuid(),
            Json = "test json",
            Feature = MultiPageFormDataFeature.AddNewCourse.Name,
            CreatedDate = new DateTime(2022, 06, 14, 14, 20, 12),
        };

        private SqlConnection connection = null!;

        private IMultiPageFormDataService multiPageFormDataService = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            multiPageFormDataService = new MultiPageFormDataService(connection);
        }

        [Test]
        public void GetMultiPageFormDataByGuidAndFeature_returns_expected_MultiPageFormData()
        {
            // Given
            using var transaction = new TransactionScope();
            InsertMultiPageFormData(dataInDb);

            // When
            var result = multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(
                dataInDb.TempDataGuid,
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().BeEquivalentTo(dataInDb);
        }

        [Test]
        public void GetMultiPageFormDataByGuidAndFeature_returns_null_with_incorrect_guid()
        {
            // Given
            using var transaction = new TransactionScope();
            InsertMultiPageFormData(dataInDb);

            // When
            var result = multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(
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
            InsertMultiPageFormData(dataInDb);

            // When
            var result = multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(
                dataInDb.TempDataGuid,
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
            multiPageFormDataService.InsertMultiPageFormData(dataInDb);
            var result = multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(
                dataInDb.TempDataGuid,
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().BeEquivalentTo(dataInDb, options => options.Excluding(d => d.Id));
        }

        [Test]
        public void UpdateJsonByGuid_updates_expected_value()
        {
            // Given
            using var transaction = new TransactionScope();
            const string newJsonString = "new json";
            InsertMultiPageFormData(dataInDb);

            // When
            multiPageFormDataService.UpdateJsonByGuid(dataInDb.TempDataGuid, newJsonString);
            var result = multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(
                dataInDb.TempDataGuid,
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().NotBeNull();
            result!.Json.Should().BeEquivalentTo(newJsonString);
        }

        [Test]
        public void UpdateJsonByGuid_does_not_update_other_records()
        {
            // Given
            using var transaction = new TransactionScope();
            const string newJsonString = "new json";
            var secondDataInDb = Builder<MultiPageFormData>
                .CreateNew()
                .With(d => d.TempDataGuid = Guid.NewGuid())
                .Build();
            InsertMultiPageFormData(dataInDb);
            InsertMultiPageFormData(secondDataInDb);

            // When
            multiPageFormDataService.UpdateJsonByGuid(secondDataInDb.TempDataGuid, newJsonString);
            var result = multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(
                dataInDb.TempDataGuid,
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().BeEquivalentTo(dataInDb);
        }

        [Test]
        public void DeleteByGuid_deletes_expected_record()
        {
            // Given
            using var transaction = new TransactionScope();
            InsertMultiPageFormData(dataInDb);

            // When
            multiPageFormDataService.DeleteByGuid(dataInDb.TempDataGuid);
            var result = multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(
                dataInDb.TempDataGuid,
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void DeleteByGuid_does_not_delete_other_records()
        {
            // Given
            using var transaction = new TransactionScope();
            var secondDataInDb = Builder<MultiPageFormData>
                .CreateNew()
                .With(d => d.TempDataGuid = Guid.NewGuid())
                .Build();
            InsertMultiPageFormData(dataInDb);
            InsertMultiPageFormData(secondDataInDb);

            // When
            multiPageFormDataService.DeleteByGuid(secondDataInDb.TempDataGuid);
            var result = multiPageFormDataService.GetMultiPageFormDataByGuidAndFeature(
                dataInDb.TempDataGuid,
                MultiPageFormDataFeature.AddNewCourse.Name
            );

            // Then
            result.Should().BeEquivalentTo(dataInDb);
        }

        private void InsertMultiPageFormData(MultiPageFormData data)
        {
            connection.Execute(
                @"SET IDENTITY_INSERT dbo.MultiPageFormData ON
                    INSERT MultiPageFormData (ID, TempDataGuid, Json, Feature, CreatedDate)
                    VALUES (@Id, @TempDataGuid, @Json, @Feature, @CreatedDate)
                    SET IDENTITY_INSERT dbo.MultiPageFormData OFF",
                data
            );
        }
    }
}
