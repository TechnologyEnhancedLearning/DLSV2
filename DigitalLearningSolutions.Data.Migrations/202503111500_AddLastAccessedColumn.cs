namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202503111500)]
    public class AddLastAccessedColumn : Migration
    {
        public override void Up()
        {
            Alter.Table("Users").AddColumn("LastAccessed").AsDateTime().Nullable();
            Alter.Table("DelegateAccounts").AddColumn("LastAccessed").AsDateTime().Nullable();
            Alter.Table("AdminAccounts").AddColumn("LastAccessed").AsDateTime().Nullable();

            Execute.Sql("UPDATE u SET LastAccessed = (SELECT MAX(s.LoginTime) FROM DelegateAccounts da JOIN Sessions s ON da.ID = s.CandidateId WHERE da.UserID = u.ID) FROM users u;");
            Execute.Sql("UPDATE da SET LastAccessed = (SELECT MAX(s.LoginTime) FROM Sessions s WHERE s.CandidateId = da.ID) FROM DelegateAccounts da;");
            Execute.Sql("UPDATE da SET LastAccessed = (SELECT ca.LastAccessed FROM CandidateAssessments ca WHERE ca.ID = da.ID) FROM DelegateAccounts da where da.LastAccessed IS NULL;");
            Execute.Sql("UPDATE AA SET LastAccessed = (SELECT MAX(AdS.LoginTime) FROM AdminSessions AdS WHERE AdS.AdminID = AA.ID) FROM AdminAccounts AA;");
        }
        public override void Down()
        {
            Delete.Column("LastAccessed").FromTable("Competencies");
            Delete.Column("LastAccessed").FromTable("DelegateAccounts");
            Delete.Column("LastAccessed").FromTable("AdminAccounts");
        }
    }
}
