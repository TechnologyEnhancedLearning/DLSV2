namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202406140805)]
    public class UpdateProgressMarkSubmittedTimeAsNullable : Migration
    {
        public override void Up()
        {
            Alter.Table("Progress")
                .AlterColumn("SubmittedTime")
                .AsDateTime()
                .Nullable();
        }
        public override void Down()
        {
            Alter.Table("Progress")
               .AlterColumn("SubmittedTime")
               .AsDateTime()
               .NotNullable();
        }
    }
}
