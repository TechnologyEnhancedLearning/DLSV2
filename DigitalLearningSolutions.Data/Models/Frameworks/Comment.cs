namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System;
    public class Comment
    {
        public int ID { get; set; }
        public int ReplyToFrameworkCommentID { get; set; }
        public int AdminID { get; set; }
        public string? Commenter { get; set; }
        public bool UserIsCommenter { get; set; }
        public DateTime AddedDate { get; set; }
        public string? Comments { get; set; }
        public DateTime LastEdit { get; set; }
    }
}
