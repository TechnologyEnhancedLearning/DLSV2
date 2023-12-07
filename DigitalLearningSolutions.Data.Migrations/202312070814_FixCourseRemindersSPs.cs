namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202312070814)]
    public class FixCourseRemindersSPs : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_3197_FixLinksInCourseReminderEmails_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_3197_FixLinksInCourseReminderEmails_DOWN);
        }
    }
}
