namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System.Collections.Generic;
    public class CommentReplies : Comment
    {
        public List<Comment> Replies { get; set; } = new List<Comment>();
    }
}
