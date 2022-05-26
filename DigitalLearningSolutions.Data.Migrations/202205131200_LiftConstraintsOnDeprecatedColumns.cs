namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202205131200)]
    public class LiftConstraintsOnDeprecatedColumns : Migration
    {
        public override void Up()
        {
            Delete.DefaultConstraint().OnTable("AdminAccounts").OnColumn("CategoryID");

            Alter.Table("AdminAccounts").AlterColumn("Password_deprecated").AsString(250).Nullable();
            Alter.Table("AdminAccounts").AlterColumn("EITSProfile_deprecated").AsCustom("varchar(max)").Nullable();

            Alter.Table("DelegateAccounts").AlterColumn("LastName_deprecated").AsString(250).Nullable();
        }

        public override void Down()
        {
            Alter.Table("AdminAccounts").AlterColumn("CategoryID").AsInt32().Nullable().WithDefaultValue(0);

            Alter.Table("AdminAccounts").AlterColumn("Password_deprecated").AsString(250).NotNullable();
            Alter.Table("AdminAccounts").AlterColumn("EITSProfile_deprecated").AsCustom("varchar(max)").NotNullable();

            Alter.Table("DelegateAccounts").AlterColumn("LastName_deprecated").AsString(250).NotNullable();
        }
    }
}
