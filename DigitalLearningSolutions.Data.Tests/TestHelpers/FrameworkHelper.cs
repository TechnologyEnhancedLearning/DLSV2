namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.Frameworks;

    public static class FrameworkHelper
    {
        public static BaseFramework CreateDefaultBaseFramework(
           int id = 1,
           string frameworkName = "name",
           int ownerAdminId = 1,
           string owner = "owner",
           int brandId = 6,
           int categoryId = 1,
           int topicId = 1,
           DateTime createdDate = default(DateTime),
           int publishStatusId = 1,
           string publishStatus = "Draft",
           int updatedByAdminId = 1,
           string updatedBy = "owner",
           int userRole = 3
       )
        {
            return new BaseFramework()
            {
                ID = id,
                FrameworkName = frameworkName,
                OwnerAdminID = ownerAdminId,
                Owner = owner,
                BrandID = brandId,
                CategoryID = categoryId,
                TopicID = topicId,
                CreatedDate = createdDate,
                PublishStatusID = publishStatusId,
                PublishStatus = publishStatus,
                UpdatedByAdminID = updatedByAdminId,
                UpdatedBy = updatedBy,
                UserRole = userRole
            };
        }
    }
}
