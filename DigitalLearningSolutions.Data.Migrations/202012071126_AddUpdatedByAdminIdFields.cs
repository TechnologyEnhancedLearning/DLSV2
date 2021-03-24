namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202012071126)]
    public class AddUpdatedByAdminIdFields : Migration
    {
        public override void Up()
        {
            Alter.Table("Competencies").AddColumn("UpdatedByAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID").WithDefaultValue(1);
            Alter.Table("CompetencyGroups").AddColumn("UpdatedByAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID").WithDefaultValue(1);
            Alter.Table("FrameworkCompetencyGroups").AddColumn("FrameworkID").AsInt32().NotNullable().ForeignKey("Frameworks", "ID");
        }

        public override void Down()
        {
            Delete.Column("UpdatedByAdminID").FromTable("Competencies");
            Delete.Column("UpdatedByAdminID").FromTable("CompetencyGroups");
            Delete.Column("FrameworkID").FromTable("FrameworkCompetencyGroups");
        }
    }
}
