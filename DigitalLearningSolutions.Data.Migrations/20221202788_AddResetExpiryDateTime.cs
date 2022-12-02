namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(20221202788)]
    public class AddResetExpiryDateTimeResults : Migration
    {
        public override void Up()
        {
            Alter.Table("ResetPassword").AddColumn("ResetExpiryDateTime").AsDateTime().Nullable();
        }
        public override void Down()
        {
            Delete.Column("ResetExpiryDateTime").FromTable("ResetPassword");
        }
    }
}
