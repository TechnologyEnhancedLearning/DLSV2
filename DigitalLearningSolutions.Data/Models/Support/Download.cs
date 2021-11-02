namespace DigitalLearningSolutions.Data.Models.Support
{
    using System;

    public class Download
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public long FileSize { get; set; }
        public string Tag { get; set; }
    }
}
