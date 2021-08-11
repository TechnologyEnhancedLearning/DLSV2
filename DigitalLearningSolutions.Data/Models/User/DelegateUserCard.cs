namespace DigitalLearningSolutions.Data.Models.User
{
    public class DelegateUserCard : DelegateUser
    {
        public bool SelfReg { get; set; }
        public bool ExternalReg { get; set; }
        public bool Active { get; set; }
        public int? AdminId { get; set; }
        public string? AliasId { get; set; }
        public int JobGroupId { get; set; }
        public bool IsPasswordSet => Password != null;
        public bool IsAdmin => AdminId.HasValue;
    }
}
