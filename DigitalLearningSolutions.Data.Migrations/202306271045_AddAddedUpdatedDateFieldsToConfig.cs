namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202306271045)]
    public class AddAddedUpdatedDateFieldsToConfig : Migration
    {
        public override void Up()
        {
            Alter.Table("Config").AddColumn("CreatedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .AddColumn("UpdatedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
            Execute.Sql(
                @$"UPDATE Config SET CreatedDate = DATEADD(YEAR,-1,GetDate()); UPDATE Config SET UpdatedDate = DATEADD(YEAR,-1,GetDate());"
            );
        }
        public override void Down()
        {
            Delete.Column("CreatedDate").FromTable("Config");
            Delete.Column("UpdatedDate").FromTable("Config");
        }
    }
}
