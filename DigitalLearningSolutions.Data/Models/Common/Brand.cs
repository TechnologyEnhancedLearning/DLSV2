namespace DigitalLearningSolutions.Data.Models.Common
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class Brand
    {
        public int BrandID { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string BrandName { get; set; }
    }
}
