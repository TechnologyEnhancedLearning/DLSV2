namespace DigitalLearningSolutions.Data.Models.Common
{
    using System.ComponentModel.DataAnnotations;
    public class BrandDetail : Brand
    {
        [StringLength(500, MinimumLength = 3)]
        public string? BrandDescription { get; set; }
        public byte[]? BrandImage { get; set; }
        [StringLength(50)]
        public string? ImageFileType { get; set; }
        public bool IncludeOnLanding { get; set; }
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        [StringLength(255)]
        public string? ContactEmail {get; set; }
        public int OwnerOrganisationId { get; set; }
        public bool Active { get; set; }
        public int OrderByNumber { get; set; }
        public byte[]? BrandLogo { get; set; }
        public int PopularityHigh { get; set; }
    }
}
