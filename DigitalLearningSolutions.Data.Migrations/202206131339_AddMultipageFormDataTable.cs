namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202206131339)]
    public class AddMultiPageFormDataTable : Migration
    {
        public override void Up()
        {
            Create.Table("MultiPageFormData")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("TempDataGuid").AsGuid().NotNullable()
                .WithColumn("Json").AsCustom("NVARCHAR(MAX)").NotNullable()
                .WithColumn("Feature").AsString(100).NotNullable()
                .WithColumn("CreatedDate").AsDateTime().NotNullable();

            Create.Index("IX_MultiPageFormData_TempDataGuid_Feature").OnTable("MultiPageFormData")
                .OnColumn("TempDataGuid").Unique().OnColumn("Feature").Unique().WithOptions().Unique();
        }

        public override void Down()
        {
            Delete.Table("MultiPageFormData");
        }
    }
}
