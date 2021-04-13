namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    public class BrandingViewModel
    {
        public SelectList BrandSelectList { get; set; }
        public SelectList CategorySelectList { get; set; }
        public SelectList TopicSelectList { get; set; }
        public DetailFramework DetailFramework { get; set; }
    }
}
