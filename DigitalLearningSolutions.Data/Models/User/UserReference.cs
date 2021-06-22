namespace DigitalLearningSolutions.Data.Models.User
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class UserReference
    {
        public readonly int Id;
        public readonly UserType UserType;

        public UserReference(int id, UserType userType)
        {
            Id = id;
            UserType = userType;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as UserReference);
        }

        private bool Equals(UserReference? other)
        {
            return other != null && Id == other.Id && UserType.Equals(other.UserType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, UserType);
        }
    }
}
