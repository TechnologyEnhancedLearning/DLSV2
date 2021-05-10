namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public class PublishViewModel
    {
        public int FrameworkId { get; set; }
        public string? FrameworkName { get; set; }
        public IEnumerable<FrameworkReview>? FrameworkReviews { get; set; }
    }
}
