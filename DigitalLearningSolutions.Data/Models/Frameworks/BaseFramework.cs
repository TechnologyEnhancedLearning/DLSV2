using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class BaseFramework
    {
        public int ID { get; set; }
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string FrameworkName { get; set; }
        public int OwnerAdminID { get; set; }
        public string? Owner { get; set; }
        public int? BrandID { get; set; }
        public int? CategoryID { get; set; }
        public int TopicID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int PublishStatusID { get; set; }
        public string? PublishStatus { get; set; }
        public int UpdatedByAdminID { get; set; }
        public string? UpdatedBy { get; set; }
        public int UserRole { get; set; }
    }
}
