﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CourseCategoriesDataServiceTests
    {
        private CourseCategoriesDataService courseCategoriesDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<CourseCategoriesDataService>>();
            courseCategoriesDataService = new CourseCategoriesDataService(connection, logger);
        }

        [Test]
        public void GetCategoriesForCentreAndCentrallyManagedCourses_should_return_expected_items()
        {
            // Given
            var expectedCategories = new List<Category>
            {
                new Category { CategoryName = "Clinical Skills", CourseCategoryID = 6 },
                new Category { CategoryName = "Digital Workplace", CourseCategoryID = 4 },
                new Category { CategoryName = "Office 2007", CourseCategoryID = 2 },
                new Category { CategoryName = "Office 2010", CourseCategoryID = 3 },
                new Category { CategoryName = "test", CourseCategoryID = 5 },
                new Category { CategoryName = "Undefined", CourseCategoryID = 1 }
            };

            // When
            var result = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(101).ToList();

            // Then
            result.Should().BeEquivalentTo(expectedCategories);
        }

        [Test]
        public void GetCourseCategoryName_returns_course_category_name()
        {
            // When
            var result = courseCategoriesDataService.GetCourseCategoryName(3);

            // Then
            result!.Should().Be("Office 2010");
        }

        [Test]
        public void GetCourseCategoryName_returns_null_for_nonexistent_category()
        {
            // When
            var result = courseCategoriesDataService.GetCourseCategoryName(-1);

            // Then
            result.Should().BeNull();
        }
    }
}
