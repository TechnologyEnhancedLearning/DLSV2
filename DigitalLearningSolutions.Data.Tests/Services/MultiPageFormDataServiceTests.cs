namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using NUnit.Framework;

    public class MultiPageFormDataServiceTests
    {
        private IClockService clockService = null!;
        private IMultiPageFormDataDataService multiPageFormDataDataService = null!;
        private IMultiPageFormDataService multiPageFormDataService = null!;
        private ITempDataDictionary tempDataDictionary = null!;

        [SetUp]
        public void Setup()
        {
            clockService = A.Fake<IClockService>();
            multiPageFormDataDataService = A.Fake<IMultiPageFormDataDataService>();
            multiPageFormDataService = new MultiPageFormDataService(clockService, multiPageFormDataDataService);

            tempDataDictionary = new TempDataDictionary(new DefaultHttpContext(), A.Fake<ITempDataProvider>());
        }

        [Test]
        public void SetMultiPageFormData_inserts_MultiPageFormData_and_sets_TempData_Guid_when_TempData_Guid_is_null()
        {
            // Given
            const int objectToInsert = 12345;
            var feature = MultiPageFormDataFeature.AddNewCourse;
            var currentTime = DateTime.UtcNow;
            A.CallTo(() => clockService.UtcNow).Returns(currentTime);

            // When
            multiPageFormDataService.SetMultiPageFormData(objectToInsert, feature, tempDataDictionary);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(A<Guid>._, A<string>._)
                ).MustNotHaveHappened();
                A.CallTo(() => multiPageFormDataDataService.UpdateJsonByGuid(A<Guid>._, A<string>._))
                    .MustNotHaveHappened();
                A.CallTo(
                    () => multiPageFormDataDataService.InsertMultiPageFormData(
                        A<MultiPageFormData>.That.Matches(
                            d => d.Json == objectToInsert.ToString() &&
                                 d.Feature == feature.Name &&
                                 d.CreatedDate == currentTime
                        )
                    )
                ).MustHaveHappenedOnceExactly();
                tempDataDictionary[feature.TempDataKey].Should().BeOfType<Guid>();
            }
        }

        [Test]
        public void
            SetMultiPageFormData_inserts_MultiPageFormData_and_sets_TempData_Guid_when_no_existing_record_is_found()
        {
            // Given
            const int objectToInsert = 12345;
            var feature = MultiPageFormDataFeature.AddNewCourse;
            var currentTime = DateTime.UtcNow;
            A.CallTo(() => clockService.UtcNow).Returns(currentTime);
            var guid = Guid.NewGuid();
            tempDataDictionary[feature.TempDataKey] = guid;
            A.CallTo(() => multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(guid, feature.Name))
                .Returns(null);

            // When
            multiPageFormDataService.SetMultiPageFormData(objectToInsert, feature, tempDataDictionary);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(guid, feature.Name)
                ).MustHaveHappenedOnceExactly();
                A.CallTo(() => multiPageFormDataDataService.UpdateJsonByGuid(A<Guid>._, A<string>._))
                    .MustNotHaveHappened();
                A.CallTo(
                    () => multiPageFormDataDataService.InsertMultiPageFormData(
                        A<MultiPageFormData>.That.Matches(
                            d => d.Json == objectToInsert.ToString() &&
                                 d.Feature == feature.Name &&
                                 d.CreatedDate == currentTime
                        )
                    )
                ).MustHaveHappenedOnceExactly();
                tempDataDictionary[feature.TempDataKey].Should().BeOfType<Guid>();
            }
        }

        [Test]
        public void
            SetMultiPageFormData_updates_existing_MultiPageFormData_and_sets_TempData_Guid_if_existing_record_is_found()
        {
            // Given
            const int objectToInsert = 12345;
            var feature = MultiPageFormDataFeature.AddNewCourse;
            var currentTime = DateTime.UtcNow;
            var guid = Guid.NewGuid();
            tempDataDictionary[feature.TempDataKey] = guid;
            A.CallTo(() => multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(guid, feature.Name))
                .Returns(
                    new MultiPageFormData
                    {
                        Id = 1,
                        TempDataGuid = guid,
                        Json = "67890",
                        Feature = feature.Name,
                        CreatedDate = currentTime,
                    }
                );

            // When
            multiPageFormDataService.SetMultiPageFormData(objectToInsert, feature, tempDataDictionary);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(guid, feature.Name)
                ).MustHaveHappenedOnceExactly();
                A.CallTo(() => multiPageFormDataDataService.UpdateJsonByGuid(guid, objectToInsert.ToString()))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => multiPageFormDataDataService.InsertMultiPageFormData(A<MultiPageFormData>._)
                ).MustNotHaveHappened();
                tempDataDictionary[feature.TempDataKey].Should().Be(guid);
            }
        }

        [Test]
        public void GetMultiPageFormData_throws_exception_when_TempData_Guid_is_null()
        {
            // When
            Action act = () => multiPageFormDataService.GetMultiPageFormData<int>(
                MultiPageFormDataFeature.AddNewCourse,
                tempDataDictionary
            );

            // Then
            act.Should().Throw<MultiPageFormDataException>()
                .WithMessage("Attempted to get data with no Guid identifier");
        }

        [Test]
        public void GetMultiPageFormData_throws_exception_when_no_data_is_found()
        {
            // Given
            var guid = Guid.NewGuid();
            var feature = MultiPageFormDataFeature.AddNewCourse;
            tempDataDictionary[feature.TempDataKey] = guid;
            A.CallTo(() => multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(guid, feature.Name))
                .Returns(null);

            // When
            Action act = () => multiPageFormDataService.GetMultiPageFormData<int>(
                feature,
                tempDataDictionary
            );

            // Then
            act.Should().Throw<MultiPageFormDataException>()
                .WithMessage($"MultiPageFormData not found for {guid}");
        }

        [Test]
        public void GetMultiPageFormData_returns_expected_object_and_keeps_TempData_Guid()
        {
            // Given
            const int expectedValue = 67890;
            var guid = Guid.NewGuid();
            var feature = MultiPageFormDataFeature.AddNewCourse;
            tempDataDictionary[feature.TempDataKey] = guid;

            A.CallTo(() => multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(guid, feature.Name))
                .Returns(
                    new MultiPageFormData
                    {
                        Id = 1,
                        TempDataGuid = guid,
                        Json = expectedValue.ToString(),
                        Feature = feature.Name,
                        CreatedDate = DateTime.UtcNow,
                    }
                );

            // When
            var result = multiPageFormDataService.GetMultiPageFormData<int>(
                feature,
                tempDataDictionary
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(expectedValue);
                tempDataDictionary[feature.TempDataKey].Should().Be(guid);
            }
        }

        [Test]
        public void ClearMultiPageFormData_throws_exception_when_TempData_Guid_is_null()
        {
            // When
            Action act = () => multiPageFormDataService.ClearMultiPageFormData(
                MultiPageFormDataFeature.AddNewCourse,
                tempDataDictionary
            );

            // Then
            act.Should().Throw<MultiPageFormDataException>()
                .WithMessage("Attempted to clear data with no Guid identifier");
        }

        [Test]
        public void ClearMultiPageFormData_clears_both_database_value_and_TempData_value()
        {
            // Given
            var guid = Guid.NewGuid();
            var feature = MultiPageFormDataFeature.AddNewCourse;
            tempDataDictionary[feature.TempDataKey] = guid;

            // When
            multiPageFormDataService.ClearMultiPageFormData(
                feature,
                tempDataDictionary
            );

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => multiPageFormDataDataService.DeleteByGuid(guid)).MustHaveHappenedOnceExactly();
                tempDataDictionary[feature.TempDataKey].Should().BeNull();
            }
        }

        [Test]
        public void FormDataExistsForGuidAndFeature_returns_true_when_data_is_not_null()
        {
            // Given
            var guid = Guid.NewGuid();
            var feature = MultiPageFormDataFeature.AddNewCourse;
            A.CallTo(() => multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(guid, feature.Name))
                .Returns(
                    new MultiPageFormData
                    {
                        Id = 1,
                        TempDataGuid = guid,
                        Json = "123",
                        Feature = feature.Name,
                        CreatedDate = DateTime.UtcNow,
                    }
                );

            // When
            var result = multiPageFormDataService.FormDataExistsForGuidAndFeature(
                feature,
                guid
            );

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void FormDataExistsForGuidAndFeature_returns_false_when_data_is_null()
        {
            // Given
            var guid = Guid.NewGuid();
            var feature = MultiPageFormDataFeature.AddNewCourse;
            A.CallTo(() => multiPageFormDataDataService.GetMultiPageFormDataByGuidAndFeature(guid, feature.Name))
                .Returns(null);

            // When
            var result = multiPageFormDataService.FormDataExistsForGuidAndFeature(
                feature,
                guid
            );

            // Then
            result.Should().BeFalse();
        }
    }
}
