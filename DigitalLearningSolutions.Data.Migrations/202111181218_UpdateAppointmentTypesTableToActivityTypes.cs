namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111181218)]
    public class UpdateAppointmentTypesTableToActivityTypes : Migration
    {
        public override void Up()
        {
            Rename.Table("AppointmentTypes").To("ActivityTypes");

            Delete.ForeignKey("FK_LearningLogItems_AdminUsers").OnTable("LearningLogItems");

            Rename.Column("ApptTypeID").OnTable("ActivityTypes").To("ID");

            Alter.Table("LearningLogItems")
                .AlterColumn("ActivityTypeID").AsInt32().NotNullable()
                .ForeignKey("ActivityTypes", "ID");

            Execute.Sql(@"INSERT INTO ActivityTypes
                        (TypeLabel, TypeCaption, SchedulerOrderNum)
                        VALUES
                        ('Learning Hub Resource', 'Learning Hub Resource', (SELECT Max(SchedulerOrderNum)+1 FROM ActivityTypes))");
        }

        public override void Down()
        {
            Execute.Sql(@"DELETE ActivityTypes WHERE TypeLabel = 'Learning Hub Resource'");

            Delete.ForeignKey("FK_LearningLogItems_ActivityTypeID_ActivityTypes_ID").OnTable("LearningLogItems");

            Rename.Column("ID").OnTable("ActivityTypes").To("ApptTypeID");

            // This foreign key was incorrectly named before it was update in the Up, so reverting it to the old bad name
            Alter.Table("LearningLogItems")
                .AlterColumn("ActivityTypeID").AsInt32().NotNullable()
                .ForeignKey("FK_LearningLogItems_AdminUsers", "ActivityTypes", "ApptTypeID");

            Rename.Table("ActivityTypes").To("AppointmentTypes");
        }
    }
}
