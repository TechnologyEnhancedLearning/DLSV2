namespace DigitalLearningSolutions.Data.Models.Support
{
    using System;

    public class Resource
    {
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime UploadDateTime { get; set; }
        public long FileSize { get; set; }
        public string Tag { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
