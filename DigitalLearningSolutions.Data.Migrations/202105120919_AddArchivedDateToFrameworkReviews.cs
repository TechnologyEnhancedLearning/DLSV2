namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202105120919)]
    public class AddArchivedDateToFrameworkReviews : Migration
    {
        public override void Up()
        {
            Alter.Table("FrameworkReviews").AddColumn("SignOffRequired").AsBoolean().NotNullable().WithDefaultValue(false);
            Alter.Table("FrameworkReviews").AddColumn("Archived").AsDateTime().Nullable();
        }
        public override void Down()
        {
            Delete.Column("SignOffRequired").FromTable("FrameworkReviews");
            Delete.Column("Archived").FromTable("FrameworkReviews");
        }
    }
}
