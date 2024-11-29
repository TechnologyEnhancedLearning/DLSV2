namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202411051212)]
    public class UpdateCourseCategories : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(@$"UPDATE CourseCategories SET CategoryName = LTRIM(RTRIM(CategoryName))");
        }

    }
}
