namespace DigitalLearningSolutions.Data.Models.Auth
{
    using System;

    public class ResetPasswordCreateModel
    {
        public ResetPasswordCreateModel(DateTime createTime, string hash, int userId,DateTime expiryTime)
        {
            CreateTime = createTime;
            Hash = hash;
            UserId = userId;
            ExpiryTime = expiryTime;
        }

        public readonly DateTime CreateTime;
        public readonly string Hash;
        public readonly int UserId;
        public readonly DateTime ExpiryTime;
    }
}
