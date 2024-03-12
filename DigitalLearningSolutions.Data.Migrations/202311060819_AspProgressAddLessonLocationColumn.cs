namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202311060819)]
    public class AspProgressAddLessonLocationColumn : Migration
    {
        public override void Up()
        {
            Alter.Table("aspProgress").AddColumn("LessonLocation").AsString(255).Nullable();
        }
        public override void Down()
        {
            Delete.Column("LessonLocation").FromTable("aspProgress");
        }
    }
}
