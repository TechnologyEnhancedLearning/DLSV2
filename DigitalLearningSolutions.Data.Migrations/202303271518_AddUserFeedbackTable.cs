namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202303271518)]
    public class AddUserFeedbackTable : Migration
    {
        public override void Up()
        {
            Create.Table("UserFeedback")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("SubmittedDate").AsDateTime().NotNullable()
                .WithColumn("UserID").AsInt32().Nullable()
                .ForeignKey("FK_Users_UserID_Users_ID", "Users", "ID").WithDefaultValue(0)
                .WithColumn("SourcePageUrl").AsString(255).NotNullable()
                .WithColumn("TaskAchieved").AsBoolean().Nullable()
                .WithColumn("TaskAttempted").AsString(255).Nullable()
                .WithColumn("FeedbackText").AsString(5000).Nullable()
                .WithColumn("TaskRating").AsInt32().Nullable()
                .WithColumn("UserRoles").AsString(255).Nullable();

            Alter.Table("UserFeedback")
                .AlterColumn("SubmittedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
        }

        public override void Down()
        {
            Delete.Table("UserFeedback");
        }
    }
}
