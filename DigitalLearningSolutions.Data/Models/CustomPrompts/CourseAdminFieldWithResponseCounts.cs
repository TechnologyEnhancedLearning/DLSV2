namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CourseAdminFieldWithResponseCounts : CourseAdminField
    {
        public CourseAdminFieldWithResponseCounts(int customPromptNumber, string text, string? options) :
            base(customPromptNumber, text, options)
        { }

        public IEnumerable<ResponseCount> ResponseCounts { get; set; }
    }
}
