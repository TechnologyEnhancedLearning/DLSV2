namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202607300908)]
    public class AddIndexesForGetAllDelegates : Migration
    {
        public override void Up()
        {
            Create.Index("IX_AdminAccounts_Active_UserID_CentreID_ID").OnTable("AdminAccounts")
                .OnColumn("Active").Ascending()
                .OnColumn("UserID").Ascending()
                .OnColumn("CentreID").Ascending()
                .WithOptions().NonClustered()
                .Include("ID");

            Create.Index("IX_DelegateAccounts_CentreID_Active_Approved_RegistrationConfirmationHash_UserID").OnTable("DelegateAccounts")
                .OnColumn("CentreID").Ascending()
                .OnColumn("Active").Ascending()
                .OnColumn("Approved").Ascending()
                .OnColumn("RegistrationConfirmationHash").Ascending()
                .OnColumn("UserID").Ascending()
                .WithOptions().NonClustered()
                .Include("ID")
                .Include("CandidateNumber")
                .Include("DateRegistered")
                .Include("LastAccessed")
                .Include("SelfReg");

            Create.Index("IX_Users_LearningHubAuthID").OnTable("Users")
                .OnColumn("LearningHubAuthID").Ascending()
                .WithOptions().NonClustered();
        }

        public override void Down()
        {
            Delete.Index("IX_AdminAccounts_Active_UserID_CentreID_ID").OnTable("AdminAccounts");
            Delete.Index("IX_DelegateAccounts_CentreID_Active_Approved_RegistrationConfirmationHash_UserID").OnTable("DelegateAccounts");
            Delete.Index("IX_Users_LearningHubAuthID").OnTable("Users");
        }
    }
}
