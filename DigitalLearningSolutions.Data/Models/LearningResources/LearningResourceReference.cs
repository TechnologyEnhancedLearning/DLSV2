namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    using System;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;

    public class LearningResourceReference
    {
        public int Id { get; set; }
        public int ResourceRefId { get; set; }
        public int AdminId { get; set; }
        private DateTime Added { get; set; }
        public string OriginalResourceName { get; set; }
        public string? OriginalDescription { get; set; }
        public string? OriginalCatalogueName { get; set; }
        public string? OriginalResourceType { get; set; }
        public decimal OriginalRating { get; set; }
        public string? ResourceLink { get; set; }

        public ResourceReferenceWithResourceDetails MapToResourceReferenceWithResourceDetails()
        {
            var originalCatalogue = new Catalogue
            {
                Name = OriginalCatalogueName ?? string.Empty,
            };

            return new ResourceReferenceWithResourceDetails
            {
                RefId = ResourceRefId,
                Title = OriginalResourceName,
                Description = OriginalDescription ?? string.Empty,
                Link = ResourceLink ?? string.Empty,
                ResourceType = OriginalResourceType ?? string.Empty,
                Rating = OriginalRating,
                Catalogue = originalCatalogue,
            };
        }
    }
}
