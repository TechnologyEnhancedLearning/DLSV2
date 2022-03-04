namespace DigitalLearningSolutions.Data.Models.User
{
    using System;
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
            _ => RegistrationType.RegisteredByCentre,
        };

        public static string GetPropertyNameForDelegateRegistrationPromptAnswer(int customPromptNumber)
        {
            return customPromptNumber switch
            {
                1 => nameof(Answer1),
                2 => nameof(Answer2),
                3 => nameof(Answer3),
                4 => nameof(Answer4),
                5 => nameof(Answer5),
                6 => nameof(Answer6),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
