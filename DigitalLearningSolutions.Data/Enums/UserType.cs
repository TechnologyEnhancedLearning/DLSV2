namespace DigitalLearningSolutions.Data.Enums
{
    public class UserType : Enumeration
    {
        public static readonly UserType AdminUser = new UserType(0, nameof(AdminUser), "AdminUsers", "AdminID");
        public static readonly UserType DelegateUser = new UserType(1, nameof(DelegateUser), "Candidates", "CandidateID");

        private UserType(int id, string name, string tableName, string idColumnName) : base(id, name)
        {
            TableName = tableName;
            IdColumnName = idColumnName;
        }

        public readonly string TableName;
        public readonly string IdColumnName;

        public static implicit operator UserType(string value)
        {
            return FromName<UserType>(value);
        }
    }
}
