namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(20210620210831)]
    public class AddArchiveDateToGetCompletedCoursesSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.DLSV2_236_GetCompletedCoursesTweak_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.DLSV2_236_GetCompletedCoursesTweak_DOWN);
        }

    }
}
