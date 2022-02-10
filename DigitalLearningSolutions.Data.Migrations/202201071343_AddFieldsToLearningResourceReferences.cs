namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202201071343)]
    public class AddFieldsToLearningResourceReferences : Migration
    {
        public override void Up()
        {
            Alter.Table("LearningResourceReferences")
                .AddColumn("ResourceLink").AsString(256).Nullable()
                .AddColumn("OriginalDescription").AsString(4000).Nullable()
                .AddColumn("OriginalResourceType").AsString(128).Nullable()
                .AddColumn("OriginalCatalogueName").AsString(128).Nullable();
        }
        public override void Down()
        {
            Delete.Column("ResourceLink").FromTable("LearningResourceReferences");
            Delete.Column("OriginalDescription").FromTable("LearningResourceReferences");
            Delete.Column("OriginalResourceType").FromTable("LearningResourceReferences");
            Delete.Column("OriginalCatalogueName").FromTable("LearningResourceReferences");
        }
    }
}
