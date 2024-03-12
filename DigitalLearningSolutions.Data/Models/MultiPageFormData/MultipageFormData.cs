namespace DigitalLearningSolutions.Data.Models.MultiPageFormData
{
    using System;

    public class MultiPageFormData
    {
        public int Id { get; set; }

        public Guid TempDataGuid { get; set; }

        public string Json { get; set; } = string.Empty;

        public string Feature { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
    }
}
