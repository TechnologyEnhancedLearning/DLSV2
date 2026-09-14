namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202609071108)]
    public class TD_6926_Alter_Table_SelfAssessments : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments")
                .AlterColumn("SignOffRequestorStatement")
                .AsString(3000)
                .Nullable();
            Alter.Table("SelfAssessments")
                .AlterColumn("SignOffSupervisorStatement")
                .AsString(3000)
                .Nullable();
        }

        public override void Down()
        {
            Alter.Table("SelfAssessments")
                .AlterColumn("SignOffRequestorStatement")
                .AsString(2000)
                .Nullable();
            Alter.Table("SelfAssessments")
                .AlterColumn("SignOffSupervisorStatement")
                .AsString(2000)
                .Nullable();
        }
    }
}
