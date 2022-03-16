﻿namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202203161448)]
    public class RestructureAdminAndDelegateTables : Migration
    {
        public override void Up()
        {
            Rename.Table("AdminUsers").To("AdminAccounts");
            Alter.Table("AdminAccounts").AddColumn("UserID").AsInt32().Nullable().ForeignKey("Users", "ID");
            Alter.Column("CategoryID").OnTable("AdminAccounts").AsInt32().Nullable()
                .ForeignKey("CourseCategories", "CourseCategoryID");
            Alter.Table("AdminAccounts").AddColumn("EmailVerified").AsDateTime().Nullable();

            Rename.Column("UserAdmin").OnTable("AdminAccounts").To("IsSuperAdmin");
            Rename.Column("CentreAdmin").OnTable("AdminAccounts").To("IsCentreAdmin");
            Rename.Column("SummaryReports").OnTable("AdminAccounts").To("IsReportsViewer");
            Rename.Column("ContentManager").OnTable("AdminAccounts").To("IsContentManager");
            Rename.Column("ContentCreator").OnTable("AdminAccounts").To("IsContentCreator");
            Rename.Column("Supervisor").OnTable("AdminAccounts").To("IsSupervisor");
            Rename.Column("Trainer").OnTable("AdminAccounts").To("IsTrainer");
            Rename.Column("NominatedSupervisor").OnTable("AdminAccounts").To("IsNominatedSupervisor");

            Rename.Column("Login").OnTable("AdminAccounts").To("Login_deprecated");
            Rename.Column("Password").OnTable("AdminAccounts").To("Password_deprecated");
            Rename.Column("ConfigAdmin").OnTable("AdminAccounts").To("ConfigAdmin_deprecated");
            Rename.Column("Forename").OnTable("AdminAccounts").To("Forename_deprecated");
            Rename.Column("Surname").OnTable("AdminAccounts").To("Surname_deprecated");
            Rename.Column("Approved").OnTable("AdminAccounts").To("Approved_deprecated");
            Rename.Column("PasswordReminder").OnTable("AdminAccounts").To("PasswordReminder_deprecated");
            Rename.Column("PasswordReminderHash").OnTable("AdminAccounts").To("PasswordReminderHash_deprecated");
            Rename.Column("PasswordReminderDate").OnTable("AdminAccounts").To("PasswordReminderDate_deprecated");
            Rename.Column("EITSProfile").OnTable("AdminAccounts").To("EITSProfile_deprecated");
            Rename.Column("TCAgreed").OnTable("AdminAccounts").To("TCAgreed_deprecated");
            Rename.Column("FailedLoginCount").OnTable("AdminAccounts").To("FailedLoginCount_deprecated");
            Rename.Column("ProfileImage").OnTable("AdminAccounts").To("ProfileImage_deprecated");
            Rename.Column("SkypeHandle").OnTable("AdminAccounts").To("SkypeHandle_deprecated");
            Rename.Column("PublicSkypeLink").OnTable("AdminAccounts").To("PublicSkypeLink_deprecated");
            Rename.Column("ResetPasswordID").OnTable("AdminAccounts").To("ResetPasswordID_deprecated");

            Rename.Table("Candidates").To("DelegateAccounts");
            Alter.Table("DelegateAccounts").AddColumn("UserID").AsInt32().NotNullable().ForeignKey("Users", "ID");

            Alter.Column("DateRegistered").OnTable("DelegateAccounts").AsDateTime().Nullable();
            Alter.Column("CandidateNumber").OnTable("DelegateAccounts").AsString(20).NotNullable();
            Rename.Column("EmailAddress").OnTable("DelegateAccounts").To("Email");
            Alter.Table("DelegateAccounts").AddColumn("EmailVerified").AsDateTime().Nullable();
            Alter.Column("Answer1").OnTable("DelegateAccounts").AsString(100).Nullable();
            Alter.Column("Answer2").OnTable("DelegateAccounts").AsString(100).Nullable();
            Alter.Column("Answer3").OnTable("DelegateAccounts").AsString(100).Nullable();

            Rename.Column("FirstName").OnTable("DelegateAccounts").To("FirstName_deprecated");
            Rename.Column("LastName").OnTable("DelegateAccounts").To("LastName_deprecated");
            Rename.Column("JobGroupID").OnTable("DelegateAccounts").To("JobGroupID_deprecated");
            Rename.Column("AliasID").OnTable("DelegateAccounts").To("AliasID_deprecated");
            Rename.Column("Password").OnTable("DelegateAccounts").To("Password_deprecated");
            Rename.Column("SkipPW").OnTable("DelegateAccounts").To("SkipPW_deprecated");
            Rename.Column("ResetHash").OnTable("DelegateAccounts").To("ResetHash_deprecated");
            Rename.Column("SkypeHandle").OnTable("DelegateAccounts").To("SkypeHandle_deprecated");
            Rename.Column("PublicSkypeLink").OnTable("DelegateAccounts").To("PublicSkypeLink_deprecated");
            Rename.Column("ProfileImage").OnTable("DelegateAccounts").To("ProfileImage_deprecated");
            Rename.Column("ResetPasswordID").OnTable("DelegateAccounts").To("ResetPasswordID_deprecated");
            Rename.Column("HasBeenPromptedForPrn").OnTable("DelegateAccounts").To("HasBeenPromptedForPrn_deprecated");
            Rename.Column("ProfessionalRegistrationNumber").OnTable("DelegateAccounts")
                .To("ProfessionalRegistrationNumber_deprecated");
            Rename.Column("LearningHubAuthID").OnTable("DelegateAccounts").To("LearningHubAuthID_deprecated");
            Rename.Column("HasDismissedLhLoginWarning").OnTable("DelegateAccounts")
                .To("HasDismissedLhLoginWarning_deprecated");
        }

        public override void Down()
        {
            Rename.Column("Login_deprecated").OnTable("AdminAccounts").To("Login");
            Rename.Column("Password_deprecated").OnTable("AdminAccounts").To("Password");
            Rename.Column("ConfigAdmin_deprecated").OnTable("AdminAccounts").To("ConfigAdmin");
            Rename.Column("Forename_deprecated").OnTable("AdminAccounts").To("Forename");
            Rename.Column("Surname_deprecated").OnTable("AdminAccounts").To("Surname");
            Rename.Column("Approved_deprecated").OnTable("AdminAccounts").To("Approved");
            Rename.Column("PasswordReminder_deprecated").OnTable("AdminAccounts").To("PasswordReminder");
            Rename.Column("PasswordReminderHash_deprecated").OnTable("AdminAccounts").To("PasswordReminderHash");
            Rename.Column("PasswordReminderDate_deprecated").OnTable("AdminAccounts").To("PasswordReminderDate");
            Rename.Column("EITSProfile_deprecated").OnTable("AdminAccounts").To("EITSProfile");
            Rename.Column("TCAgreed_deprecated").OnTable("AdminAccounts").To("TCAgreed");
            Rename.Column("FailedLoginCount_deprecated").OnTable("AdminAccounts").To("FailedLoginCount");
            Rename.Column("ProfileImage_deprecated").OnTable("AdminAccounts").To("ProfileImage");
            Rename.Column("SkypeHandle_deprecated").OnTable("AdminAccounts").To("SkypeHandle");
            Rename.Column("PublicSkypeLink_deprecated").OnTable("AdminAccounts").To("PublicSkypeLink");
            Rename.Column("ResetPasswordID_deprecated").OnTable("AdminAccounts").To("ResetPasswordID");

            Rename.Column("IsUserAdmin").OnTable("AdminAccounts").To("SuperAdmin");
            Rename.Column("IsCentreAdmin").OnTable("AdminAccounts").To("CentreAdmin");
            Rename.Column("IsReportsViewer").OnTable("AdminAccounts").To("SummaryReports");
            Rename.Column("IsContentManager").OnTable("AdminAccounts").To("ContentManager");
            Rename.Column("IsContentCreator").OnTable("AdminAccounts").To("ContentCreator");
            Rename.Column("IsSupervisor").OnTable("AdminAccounts").To("Supervisor");
            Rename.Column("IsTrainer").OnTable("AdminAccounts").To("Trainer");
            Rename.Column("IsNominatedSupervisor").OnTable("AdminAccounts").To("NominatedSupervisor");

            Delete.Column("EmailVerified").FromTable("AdminAccounts");
            Delete.ForeignKey("FK_AdminAccounts_CategoryID_CourseCategories_CourseCategoryID").OnTable("AdminAccounts");
            Alter.Column("CategoryID").OnTable("AdminAccounts").AsInt32().NotNullable();
            Delete.ForeignKey("FK_AdminAccounts_UserID_Users_ID").OnTable("AdminAccounts");
            Delete.Column("UserID").FromTable("AdminAccounts");
            Rename.Table("AdminAccounts").To("AdminUsers");

            Rename.Column("FirstName_deprecated").OnTable("DelegateAccounts").To("FirstName");
            Rename.Column("LastName_deprecated").OnTable("DelegateAccounts").To("LastName");
            Rename.Column("JobGroupID_deprecated").OnTable("DelegateAccounts").To("JobGroupID");
            Rename.Column("AliasID_deprecated").OnTable("DelegateAccounts").To("AliasID");
            Rename.Column("Password_deprecated").OnTable("DelegateAccounts").To("Password");
            Rename.Column("SkipPW_deprecated").OnTable("DelegateAccounts").To("SkipPW");
            Rename.Column("ResetHash_deprecated").OnTable("DelegateAccounts").To("ResetHash");
            Rename.Column("SkypeHandle_deprecated").OnTable("DelegateAccounts").To("SkypeHandle");
            Rename.Column("PublicSkypeLink_deprecated").OnTable("DelegateAccounts").To("PublicSkypeLink");
            Rename.Column("ProfileImage_deprecated").OnTable("DelegateAccounts").To("ProfileImage");
            Rename.Column("ResetPasswordID_deprecated").OnTable("DelegateAccounts").To("ResetPasswordID");
            Rename.Column("HasBeenPromptedForPrn_deprecated").OnTable("DelegateAccounts").To("HasBeenPromptedForPrn");
            Rename.Column("ProfessionalRegistrationNumber_deprecated").OnTable("DelegateAccounts")
                .To("ProfessionalRegistrationNumber");
            Rename.Column("LearningHubAuthID_deprecated").OnTable("DelegateAccounts").To("LearningHubAuthID");
            Rename.Column("HasDismissedLhLoginWarning_deprecated").OnTable("DelegateAccounts")
                .To("HasDismissedLhLoginWarning");

            Alter.Column("DateRegistered").OnTable("DelegateAccounts").AsDateTime().NotNullable();
            Alter.Column("CandidateNumber").OnTable("DelegateAccounts").AsCustom("varchar(250)").NotNullable();
            Rename.Column("Email").OnTable("DelegateAccounts").To("EmailAddress");
            Delete.Column("EmailVerified").FromTable("DelegateAccounts");
            Alter.Table("DelegateAccounts").AddColumn("Answer1").AsCustom("varchar(100)").Nullable();
            Alter.Column("Answer2").OnTable("DelegateAccounts").AsCustom("varchar(100)").Nullable();
            Alter.Column("Answer3").OnTable("DelegateAccounts").AsCustom("varchar(100)").Nullable();

            Delete.ForeignKey("FK_DelegateAccounts_UserID_Users_ID").OnTable("DelegateAccounts");
            Delete.Column("UserID").FromTable("DelegateAccounts");
            Rename.Table("DelegateAccounts").To("Candidates");
        }
    }
}
