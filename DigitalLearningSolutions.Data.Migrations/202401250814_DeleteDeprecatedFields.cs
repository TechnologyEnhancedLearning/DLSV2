namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202401250814)]
    public class DeleteDeprecatedFields : Migration
    {
        public override void Up()
        {
            Delete.Column("Login_deprecated").FromTable("AdminAccounts");
            Delete.Column("Password_deprecated").FromTable("AdminAccounts");
            Delete.Column("ConfigAdmin_deprecated").FromTable("AdminAccounts");
            Delete.Column("Forename_deprecated").FromTable("AdminAccounts");
            Delete.Column("Surname_deprecated").FromTable("AdminAccounts");
            Delete.Column("Email_deprecated").FromTable("AdminAccounts");
            Delete.Column("Approved_deprecated").FromTable("AdminAccounts");
            Delete.Column("PasswordReminder_deprecated").FromTable("AdminAccounts");
            Delete.Column("PasswordReminderHash_deprecated").FromTable("AdminAccounts");
            Delete.Column("PasswordReminderDate_deprecated").FromTable("AdminAccounts");
            Delete.Column("EITSProfile_deprecated").FromTable("AdminAccounts");
            Delete.Column("TCAgreed_deprecated").FromTable("AdminAccounts");
            Delete.Column("FailedLoginCount_deprecated").FromTable("AdminAccounts");
            Delete.Column("ProfileImage_deprecated").FromTable("AdminAccounts");
            Delete.Column("SkypeHandle_deprecated").FromTable("AdminAccounts");
            Delete.Column("PublicSkypeLink_deprecated").FromTable("AdminAccounts");
            Delete.Column("ResetPasswordID_deprecated").FromTable("AdminAccounts");

            if (Schema.Table("DelegateAccounts").Index("IX_DelegateAccounts_Email").Exists())
                Delete.Index("IX_DelegateAccounts_Email").OnTable("DelegateAccounts");
            Delete.Column("FirstName_deprecated").FromTable("DelegateAccounts");
            Delete.Column("LastName_deprecated").FromTable("DelegateAccounts");
            Delete.Column("JobGroupID_deprecated").FromTable("DelegateAccounts");
            Delete.Column("AliasID_deprecated").FromTable("DelegateAccounts");
            Delete.Column("Email_deprecated").FromTable("DelegateAccounts");
            Delete.Column("SkipPW_deprecated").FromTable("DelegateAccounts");
            Delete.Column("ResetHash_deprecated").FromTable("DelegateAccounts");
            Delete.Column("SkypeHandle_deprecated").FromTable("DelegateAccounts");
            Delete.Column("PublicSkypeLink_deprecated").FromTable("DelegateAccounts");
            Delete.Column("ProfileImage_deprecated").FromTable("DelegateAccounts");
            Delete.Column("HasBeenPromptedForPrn_deprecated").FromTable("DelegateAccounts");
            Delete.Column("ProfessionalRegistrationNumber_deprecated").FromTable("DelegateAccounts");
            Delete.Column("LearningHubAuthID_deprecated").FromTable("DelegateAccounts");
            Delete.Column("HasDismissedLhLoginWarning_deprecated").FromTable("DelegateAccounts");
            Delete.Column("ResetPasswordID_deprecated").FromTable("DelegateAccounts");

            Delete.Column("CandidateID_deprecated").FromTable("SupervisorDelegates");

            Delete.Column("CandidateID_deprecated").FromTable("CandidateAssessments");
        }

        public override void Down()
        {
            Alter.Table("AdminAccounts").AddColumn("Login_deprecated").AsString(250).Nullable();
            Alter.Table("AdminAccounts").AddColumn("Password_deprecated").AsString(250).Nullable();
            Alter.Table("AdminAccounts").AddColumn("ConfigAdmin_deprecated").AsBoolean().NotNullable().WithDefaultValue(0);
            Alter.Table("AdminAccounts").AddColumn("Forename_deprecated").AsString(250).Nullable();
            Alter.Table("AdminAccounts").AddColumn("Surname_deprecated").AsString(250).Nullable();
            Alter.Table("AdminAccounts").AddColumn("Email_deprecated").AsString(255).Nullable();
            Alter.Table("AdminAccounts").AddColumn("Approved_deprecated").AsBoolean().NotNullable().WithDefaultValue(1);
            Alter.Table("AdminAccounts").AddColumn("PasswordReminder_deprecated").AsBoolean().NotNullable().WithDefaultValue(0);
            Alter.Table("AdminAccounts").AddColumn("PasswordReminderHash_deprecated").AsString(64).Nullable();
            Alter.Table("AdminAccounts").AddColumn("PasswordReminderDate_deprecated").AsDateTime().Nullable();
            Alter.Table("AdminAccounts").AddColumn("EITSProfile_deprecated").AsString(int.MaxValue).Nullable();
            Alter.Table("AdminAccounts").AddColumn("TCAgreed_deprecated").AsDateTime().Nullable();
            Alter.Table("AdminAccounts").AddColumn("FailedLoginCount_deprecated").AsInt32().NotNullable().WithDefaultValue(0);
            Alter.Table("AdminAccounts").AddColumn("ProfileImage_deprecated").AsBinary().Nullable();
            Alter.Table("AdminAccounts").AddColumn("SkypeHandle_deprecated").AsString(100).Nullable();
            Alter.Table("AdminAccounts").AddColumn("PublicSkypeLink_deprecated").AsBoolean().NotNullable().WithDefaultValue(0);
            Alter.Table("AdminAccounts").AddColumn("ResetPasswordID_deprecated").AsInt32().Nullable();
            Alter.Table("AdminAccounts").AddColumn("Login_deprecated").AsString(250).Nullable();
            Alter.Table("AdminAccounts").AddColumn("Login_deprecated").AsString(250).Nullable();
            Alter.Table("AdminAccounts").AddColumn("Login_deprecated").AsString(250).Nullable();

            Alter.Table("DelegateAccounts").AddColumn("FirstName_deprecated").AsString(250).Nullable();
            Alter.Table("DelegateAccounts").AddColumn("LastName_deprecated").AsString(250).Nullable();
            Alter.Table("DelegateAccounts").AddColumn("JobGroupID_deprecated").AsInt32().NotNullable().WithDefaultValue(1);
            Alter.Table("DelegateAccounts").AddColumn("AliasID_deprecated").AsString(250).Nullable();
            Alter.Table("DelegateAccounts").AddColumn("Email_deprecated").AsString(255).Nullable();
            Alter.Table("DelegateAccounts").AddColumn("SkipPW_deprecated").AsBoolean().NotNullable().WithDefaultValue(0);
            Alter.Table("DelegateAccounts").AddColumn("ResetHash_deprecated").AsString(255).Nullable();
            Alter.Table("DelegateAccounts").AddColumn("SkypeHandle_deprecated").AsString(100).Nullable();
            Alter.Table("DelegateAccounts").AddColumn("PublicSkypeLink_deprecated").AsBoolean().NotNullable().WithDefaultValue(0);
            Alter.Table("DelegateAccounts").AddColumn("ProfileImage_deprecated").AsBinary().Nullable();
            Alter.Table("DelegateAccounts").AddColumn("HasBeenPromptedForPrn_deprecated").AsBoolean().NotNullable().WithDefaultValue(0);
            Alter.Table("DelegateAccounts").AddColumn("ProfessionalRegistrationNumber_deprecated").AsString(32).Nullable();
            Alter.Table("DelegateAccounts").AddColumn("LearningHubAuthID_deprecated").AsInt32().Nullable();
            Alter.Table("DelegateAccounts").AddColumn("HasDismissedLhLoginWarning_deprecated").AsBoolean().NotNullable().WithDefaultValue(0);
            Alter.Table("DelegateAccounts").AddColumn("ResetPasswordID_deprecated").AsInt32().Nullable();

            Alter.Table("SupervisorDelegates").AddColumn("CandidateID_deprecated").AsInt32().Nullable();

            Alter.Table("CandidateAssessments").AddColumn("CandidateID_deprecated").AsInt32().Nullable();
        }
    }
}
