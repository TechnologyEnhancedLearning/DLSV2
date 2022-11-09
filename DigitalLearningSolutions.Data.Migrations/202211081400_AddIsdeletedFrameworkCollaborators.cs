namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202211081400)]
    public class AddIsdeletedFrameworkCollaborators : Migration
    {
        public override void Up()
        {
            Alter.Table("FrameworkCollaborators").AddColumn("IsDeleted").AsBoolean().WithDefaultValue(false);
        }
        public override void Down()
        {
            Delete.Column("IsDeleted").FromTable("FrameworkCollaborators");
        }
    }
}
