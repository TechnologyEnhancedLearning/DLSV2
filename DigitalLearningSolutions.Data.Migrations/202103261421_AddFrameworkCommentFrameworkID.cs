namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202103261421)]
    public class AddFrameworkCommentFrameworkID : Migration
    {
        public override void Up()
        {
            Alter.Table("FrameworkComments").AddColumn("FrameworkID").AsInt32().NotNullable().ForeignKey("FK_Frameworks_ID_FrameworkComments_FrameworkID", "Frameworks", "ID");
        }
        public override void Down()
        {
            Delete.Column("FrameworkID").FromTable("FrameworkComments");
        }

    }
}
