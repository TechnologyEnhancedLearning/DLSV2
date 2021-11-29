﻿namespace DigitalLearningSolutions.Data.Models.LearningHubApiClient
{
    using System.Collections.Generic;

    public class ResourceSearchResult
    {
        public List<ResourceMetadata> Results { get; set; }

        public int Offset { get; set; }

        public int TotalNumResources { get; set; }
    }
}
