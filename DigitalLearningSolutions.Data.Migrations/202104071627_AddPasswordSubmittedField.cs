

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202104071627)]
    public class AddPasswordSubmittedField : Migration
    {
        public override void Up()
        {
            Alter.Table("Progress").AddColumn("PasswordSubmitted").AsBoolean().NotNullable().WithDefaultValue(0);
        }
        public override void Down()
        {
            Delete.Column("PasswordSubmitted").FromTable("Progress");
        }


    }
}
