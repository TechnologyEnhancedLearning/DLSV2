namespace DigitalLearningSolutions.Data.Models.Support
{
    using System;

    public class Resource
    {
        public Resource(string description, DateTime uploadDate, long fileSize, string tag, string fileName)
        {
            Description = description;
            UploadDate = uploadDate;
            FileSize = fileSize;
            Tag = tag;
            FileName = fileName;
        }

        public string Description { get; }
        public DateTime UploadDate { get; }
        public long FileSize { get; }
        public string Tag { get; }
        public string FileName { get; }
    }
}
