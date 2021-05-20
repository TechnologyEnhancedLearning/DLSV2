namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public class PublishViewModel
    {
        public BaseFramework BaseFramework { get; set; }
        public IEnumerable<FrameworkReview>? FrameworkReviews { get; set; }
    }
}
