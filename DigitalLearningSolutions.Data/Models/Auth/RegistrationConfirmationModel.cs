namespace DigitalLearningSolutions.Data.Models.Auth
{
    using System;

    public class RegistrationConfirmationModel
    {
        public RegistrationConfirmationModel(DateTime createTime, string hash, int delegateId)
        {
            CreateTime = createTime;
            Hash = hash;
            DelegateId = delegateId;
        }

        public readonly DateTime CreateTime;
        public readonly string Hash;
        public readonly int DelegateId;
    }
}
