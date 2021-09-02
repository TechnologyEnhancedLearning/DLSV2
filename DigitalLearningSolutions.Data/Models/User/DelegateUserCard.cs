namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Enums;

    public class DelegateUserCard : DelegateUser
    {
        public bool SelfReg { get; set; }
        public bool ExternalReg { get; set; }
        public int? AdminId { get; set; }
        public bool IsPasswordSet => Password != null;
        public bool IsAdmin => AdminId.HasValue;

        public RegistrationType RegistrationType => (SelfReg, ExternalReg) switch
        {
            (true, true) => RegistrationType.SelfRegisteredExternal,
            (true, false) => RegistrationType.SelfRegistered,
            _ => RegistrationType.RegisteredByCentre
        };
    }
}
