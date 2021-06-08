namespace DigitalLearningSolutions.Data.Models.User
{
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
            if (obj is UserReference ur)
            {
                return Id == ur.Id && UserType.Equals(ur.UserType);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 93 + 37 * (Id.GetHashCode() + 23 * UserType.GetHashCode());
            }
        }
    }
}
