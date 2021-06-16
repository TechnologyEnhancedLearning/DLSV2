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
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((UserReference)obj);
        }

        private bool Equals(UserReference other)
        {
            return Id == other.Id && UserType.Equals(other.UserType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, UserType);
        }
    }
}
