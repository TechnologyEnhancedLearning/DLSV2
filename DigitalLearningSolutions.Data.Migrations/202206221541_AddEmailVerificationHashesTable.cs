namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202206221541)]
    public class AddEmailVerificationHashesTable : Migration
    {
        public override void Up()
        {
            Create.Table("EmailVerificationHashes")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("EmailVerificationHash").AsString(64).NotNullable()
                .WithColumn("CreatedDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Alter.Table("Users")
                .AddColumn("EmailVerificationHashID").AsInt32().Nullable()
                .ForeignKey("EmailVerificationHashes", "ID");

            Alter.Table("UserCentreDetails")
                .AddColumn("EmailVerificationHashID").AsInt32().Nullable()
                .ForeignKey("EmailVerificationHashes", "ID");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_Users_EmailVerificationHashID_EmailVerificationHashes_ID").OnTable("Users");
            Delete.ForeignKey("FK_UserCentreDetails_EmailVerificationHashID_EmailVerificationHashes_ID")
                .OnTable("UserCentreDetails");
            Delete.Column("EmailVerificationHashID").FromTable("Users");
            Delete.Column("EmailVerificationHashID").FromTable("UserCentreDetails");
            Delete.Table("EmailVerificationHashes");
        }
    }
}
