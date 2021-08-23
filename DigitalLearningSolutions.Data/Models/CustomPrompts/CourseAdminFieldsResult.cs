namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    public class CourseAdminFieldsResult
    {
        public string? CustomField1Prompt { get; set; }
        public string? CustomField1Options { get; set; }
        public bool CustomField1Mandatory { get; set; }

        public string? CustomField2Prompt { get; set; }
        public string? CustomField2Options { get; set; }
        public bool CustomField2Mandatory { get; set; }

        public string? CustomField3Prompt { get; set; }
        public string? CustomField3Options { get; set; }
        public bool CustomField3Mandatory { get; set; }

        public int CourseCategoryId { get; set; }
    }
}
