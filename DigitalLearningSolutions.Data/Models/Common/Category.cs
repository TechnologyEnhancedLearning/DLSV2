namespace DigitalLearningSolutions.Data.Models.Common
{
    using System.ComponentModel.DataAnnotations;
    public class Category
    {
        public int CourseCategoryID { get; set; }
        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string CategoryName { get; set; }
    }
}
