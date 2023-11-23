namespace DigitalLearningSolutions.Data.Models.Common
{
    using System.ComponentModel.DataAnnotations;
    public class Topic
    {
        public int CourseTopicID { get; set; }
        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string CourseTopic { get; set; }
        public bool Active { get; set; }
    }
}
