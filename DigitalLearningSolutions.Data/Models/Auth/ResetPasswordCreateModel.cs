namespace DigitalLearningSolutions.Data.Models.Auth
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class ResetPasswordCreateModel
    {
        public ResetPasswordCreateModel(DateTime createTime, string hash, int userId, UserType userType)
        {
            CreateTime = createTime;
            Hash = hash;
            UserId = userId;
            UserType = userType;
        }

        public readonly DateTime CreateTime;
        public readonly string Hash;
        public readonly int UserId;
        public readonly UserType UserType;
    }
}
