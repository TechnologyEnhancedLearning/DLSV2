namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202201111458)]
    public class LearningResourceReferencesOriginalRatingField : Migration
    {
        public override void Up()
        {
            Alter.Table("LearningResourceReferences")
                .AddColumn("OriginalRating").AsDecimal(2, 1).NotNullable().WithDefaultValue(0);
        }
        public override void Down()
        {
            Delete.Column("OriginalRating").FromTable("LearningResourceReferences");
        }
    }
}
