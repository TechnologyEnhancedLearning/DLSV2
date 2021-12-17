namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202112141120)]
    public class AddLearningResourceReferencesTable : Migration
    {
        public override void Up()
        {
            Create.Table("LearningResourceReferences")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("ResourceRefID").AsInt32().NotNullable().Unique()
                .WithColumn("OriginalResourceName").AsString(255).NotNullable()
                .WithColumn("AdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID")
                .WithColumn("Added").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime);
        }

        public override void Down()
        {
            Delete.Table("LearningResourceReferences");
        }
    }
}
