namespace DigitalLearningSolutions.Data.Enums
{
    public class UserType : Enumeration
    {
        public static UserType AdminUser = new UserType(0, "AdminUser", "AdminUsers", "AdminID");
        public static UserType DelegateUser = new UserType(1, "DelegateUser", "Candidates", "CandidateID");

        private UserType(int id, string name, string tableName, string idColumnName) : base(id, name)
        {
            TableName = tableName;
            IdColumnName = idColumnName;
        }

        public readonly string TableName;
        public readonly string IdColumnName;
    }
}
