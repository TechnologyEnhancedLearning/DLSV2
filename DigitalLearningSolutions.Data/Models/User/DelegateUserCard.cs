namespace DigitalLearningSolutions.Data.Models.User
{
    public class DelegateUserCard : DelegateUser
    {
        public bool SelfReg { get; set; }
        public bool ExternalReg { get; set; }
        public int? AdminId { get; set; }
    }
}
