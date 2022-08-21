namespace DigitalLearningSolutions.Data.Models.Auth
{
    using System;

    public class ResetPasswordCreateModel
    {
        public ResetPasswordCreateModel(DateTime createTime, string hash, int userId)
        {
            CreateTime = createTime;
            Hash = hash;
            UserId = userId;
        }

        public readonly DateTime CreateTime;
        public readonly string Hash;
        public readonly int UserId;
    }
}
