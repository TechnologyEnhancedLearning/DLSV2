namespace DigitalLearningSolutions.Data.Models.Support
{
    using System;

    public class Resource
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime UploadDateTime { get; set; }
        public long FileSize { get; set; }
        public string Tag { get; set; }
        public string FileName { get; set; }
    }
}
