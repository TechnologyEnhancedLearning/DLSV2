namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111181057)]
    public class UpdateLearningLogItemsTable : Migration
    {
        public override void Up()
        {
            Alter.Table("LearningLogItems")
                .AddColumn("LastAccessedDate").AsDateTime().Nullable()
                .AddColumn("LinkedCompetencyLearningResourceID").AsInt32().Nullable()
                .ForeignKey("CompetencyLearningResources", "ID")
                .AlterColumn("LoggedByID").AsInt32().Nullable()
                .AlterColumn("LinkedCustomisationID").AsInt32().Nullable()
                .AlterColumn("LoggedByAdminID").AsInt32().Nullable();

            Rename.Column("Topic").OnTable("LearningLogItems").To("Activity");
            Rename.Column("AppointmentTypeID").OnTable("LearningLogItems").To("ActivityTypeID");
            Rename.Column("CallUri").OnTable("LearningLogItems").To("ExternalUri");

            Delete.ForeignKey("FK_LearningLogItems_Methods").OnTable("LearningLogItems");
            Delete.Column("MethodID").FromTable("LearningLogItems");
            Delete.Column("MethodOther").FromTable("LearningLogItems");

            Execute.Sql("UPDATE LearningLogItems SET LoggedByID = NULL WHERE LoggedByID = 0");
            Execute.Sql("UPDATE LearningLogItems SET LinkedCustomisationID = NULL WHERE LinkedCustomisationID = 0");
            Execute.Sql("UPDATE LearningLogItems SET LoggedByAdminID = NULL WHERE LoggedByAdminID = 0");
        }

        public override void Down()
        {
            Execute.Sql("UPDATE LearningLogItems SET LoggedByID = 0 WHERE LoggedByID IS NULL");
            Execute.Sql("UPDATE LearningLogItems SET LinkedCustomisationID = 0 WHERE LinkedCustomisationID IS NULL");
            Execute.Sql("UPDATE LearningLogItems SET LoggedByAdminID = 0 WHERE LoggedByAdminID IS NULL");

            Delete.Column("LastAccessedDate").FromTable("LearningLogItems");
            Delete.ForeignKey("FK_LearningLogItems_LinkedCompetencyLearningResourceID_CompetencyLearningResources_ID").OnTable("LearningLogItems");
            Delete.Column("LinkedCompetencyLearningResourceID").FromTable("LearningLogItems");

            Rename.Column("Activity").OnTable("LearningLogItems").To("Topic");
            Rename.Column("ActivityTypeID").OnTable("LearningLogItems").To("AppointmentTypeID");
            Rename.Column("ExternalUri").OnTable("LearningLogItems").To("CallUri");

            Alter.Table("LearningLogItems")
                .AddColumn("MethodID").AsInt32().Nullable().ForeignKey("FK_LearningLogItems_Methods", "Methods", "MethodID")
                .AddColumn("MethodOther").AsString(255).Nullable()
                .AlterColumn("LoggedByID").AsInt32().NotNullable()
                .AlterColumn("LinkedCustomisationID").AsInt32().NotNullable()
                .AlterColumn("LoggedByAdminID").AsInt32().NotNullable();
        }
    }
}
