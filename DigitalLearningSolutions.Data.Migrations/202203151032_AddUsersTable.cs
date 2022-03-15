namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202203151032)]
    public class AddUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("PrimaryEmail").AsString(255).Unique().NotNullable()
                .WithColumn("PasswordHash").AsString(70).NotNullable()
                .WithColumn("FirstName").AsString(100).NotNullable()
                .WithColumn("LastName").AsString(100).NotNullable()
                .WithColumn("JobGroupID").AsInt32().NotNullable().ForeignKey("JobGroups", "JobGroupID")
                .WithColumn("ProfessionalRegistrationNumber").AsString(32).Nullable()
                .WithColumn("ProfileImage").AsCustom("image").Nullable()
                .WithColumn("Active").AsBoolean().NotNullable()
                .WithColumn("ResetPasswordID").AsInt32().Nullable().ForeignKey("ResetPassword", "ID")
                .WithColumn("TermsAgreed").AsDateTime().Nullable()
                .WithColumn("FailedLoginCount").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("HasBeenPromptedForPrn").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("LearningHubAuthId").AsInt32().Nullable()
                .WithColumn("HasDismissedLhLoginWarning").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("EmailVerified").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}
