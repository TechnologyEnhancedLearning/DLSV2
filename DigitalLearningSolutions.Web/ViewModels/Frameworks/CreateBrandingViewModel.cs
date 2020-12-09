namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    public class CreateBrandingViewModel
    {
        public SelectList BrandSelectList { get; set; }
        public SelectList CategorySelectList { get; set; }
        public SelectList TopicSelectList { get; set; }
        public BrandedFramework BrandedFramework { get; set; }
    }
}
