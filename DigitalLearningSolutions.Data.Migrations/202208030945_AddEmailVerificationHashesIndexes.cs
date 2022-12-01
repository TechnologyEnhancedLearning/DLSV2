namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202208030945)]
    public class AddEmailVerificationHashesIndexes : Migration
    {
        public override void Up()
        {
            Create.Index("IX_EmailVerificationHashes_EmailVerificationHash").OnTable("EmailVerificationHashes")
                .OnColumn("EmailVerificationHash")
                .Ascending().WithOptions().NonClustered();
            Create.Index("IX_Users_EmailVerificationHashID").OnTable("Users").OnColumn("EmailVerificationHashID")
                .Ascending().WithOptions().NonClustered();
            Create.Index("IX_UserCentreDetails_EmailVerificationHashID").OnTable("UserCentreDetails")
                .OnColumn("EmailVerificationHashID")
                .Ascending().WithOptions().NonClustered();
        }

        public override void Down()
        {
            Delete.Index("IX_EmailVerificationHashes_EmailVerificationHash").OnTable("EmailVerificationHashes");
            Delete.Index("IX_Users_EmailVerificationHashID").OnTable("NonClustered");
            Delete.Index("IX_UserCentreDetails_EmailVerificationHashID").OnTable("UserCentreDetails");
        }
    }
}
