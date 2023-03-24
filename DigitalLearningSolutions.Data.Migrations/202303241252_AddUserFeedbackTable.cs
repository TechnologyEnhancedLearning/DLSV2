﻿namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202303241252)]
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
                .WithColumn("TaskAttempted").AsString(255).NotNullable()
                .WithColumn("FeedbackText").AsString(5000).NotNullable()
                .WithColumn("TaskRating").AsInt32().Nullable();
        }

        public override void Down()
        {
            Delete.Table("UserFeedback");
        }
    }
}
