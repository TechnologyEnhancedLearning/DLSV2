namespace DigitalLearningSolutions.Web.Tests.TestHelpers.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Controllers.FrameworksController;
    using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
    using Microsoft.AspNetCore.Mvc;
    using System;

    public static class FrameworksHelper
    {
        public static BrandedFramework CreateDefaultBrandedFramework(
             int frameworkId = 1,
             string frameworkName = "Framework 1",
             string brand = "Brand 1",
             string? category = "Category 1",
             string? topic = "Topic 1",
             int ownerAdminId = 1,
             string owner = "admin",
            int brandId = 1,
            int categoryId = 1,
            int topicId = 1,
           DateTime createdDate = default(DateTime),
            int publishStatusId = 1,
            int updatedByAdminId = 1,
            string updatedBy = "admin",
            int userRole = 3
         )
        {
            return new BrandedFramework
            {
                ID = frameworkId,
                FrameworkName = frameworkName,
                OwnerAdminID = ownerAdminId,
                Owner = owner,
                BrandID = brandId,
                CategoryID = categoryId,
                TopicID = topicId,
                CreatedDate = createdDate,
                PublishStatusID = publishStatusId,
                UpdatedByAdminID = updatedByAdminId,
                UpdatedBy = updatedBy,
                UserRole = userRole,
                Brand = brand,
                Category = category,
                Topic = topic
            };
        }
    }
}
