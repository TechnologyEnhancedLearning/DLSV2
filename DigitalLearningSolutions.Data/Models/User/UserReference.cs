namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Enums;

    public class UserReference
    {
        public readonly int Id;
        public readonly UserType UserType;

        public UserReference(int id, UserType userType)
        {
            this.Id = id;
            this.UserType = userType;
        }
    }
}
