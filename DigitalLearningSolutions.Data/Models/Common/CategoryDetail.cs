namespace DigitalLearningSolutions.Data.Models.Common
{
    using System.ComponentModel.DataAnnotations;
    public class CategoryDetail : Category
    {
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        [StringLength(255)]
        public string? ContactEmail { get; set; }
        [StringLength(20)]
        public string? CategoryContactPhone { get; set; }
        public bool Active { get; set; }
    }
}
