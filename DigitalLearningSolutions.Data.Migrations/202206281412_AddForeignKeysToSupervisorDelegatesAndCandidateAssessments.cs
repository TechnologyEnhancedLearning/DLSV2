namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202206281412)]
    public class AddForeignKeysToSupervisorDelegatesAndCandidateAssessments : Migration
    {
        private const string ColumnPopulationSql =
            @"UPDATE SupervisorDelegates
                SET SupervisorDelegates.DelegateUserID = da.UserID
                FROM Candidates c
                    JOIN DelegateAccounts da
                    ON c.CandidateNumber = da.CandidateNumber
	                WHERE c.CandidateID = SupervisorDelegates.CandidateID

              UPDATE CandidateAssessments
	            SET CandidateAssessments.DelegateUserID = da.UserID,
		            CandidateAssessments.CentreID = c.CentreID
	            FROM Candidates c
		            JOIN DelegateAccounts da
		            ON c.CandidateNumber = da.CandidateNumber
	                WHERE c.CandidateID = CandidateAssessments.CandidateID";

        public override void Up()
        {
            Alter.Table("SupervisorDelegates")
                .AddColumn("DelegateUserID").AsInt32().Nullable()
                .ForeignKey("Users", "ID");

            Alter.Table("CandidateAssessments")
                .AddColumn("DelegateUserID").AsInt32().Nullable()
                .ForeignKey("Users", "ID")
                .AddColumn("CentreID").AsInt32().Nullable()
                .ForeignKey("Centres", "CentreID");

            Execute.Sql(ColumnPopulationSql);

            Alter.Table("CandidateAssessments")
                .AlterColumn("DelegateUserID").AsInt32().NotNullable()
                .AlterColumn("CentreID").AsInt32().NotNullable();

            Delete.ForeignKey("FK_SupervisorDelegates_CandidateID_Candidates_CandidateID").OnTable("SupervisorDelegates");
            Alter.Table("SupervisorDelegates").AlterColumn("CandidateID").AsInt32().Nullable();

            Delete.ForeignKey("FK_CandidateAssessments_CandidateID_Candidates_CandidateID").OnTable("CandidateAssessments");
            Alter.Table("CandidateAssessments").AlterColumn("CandidateID").AsInt32().Nullable();

            Rename.Column("CandidateID").OnTable("SupervisorDelegates").To("CandidateID_deprecated");
            Rename.Column("CandidateID").OnTable("CandidateAssessments").To("CandidateID_deprecated");
        }

        public override void Down()
        {
            Rename.Column("CandidateID_deprecated").OnTable("SupervisorDelegates").To("CandidateID");
            Rename.Column("CandidateID_deprecated").OnTable("CandidateAssessments").To("CandidateID");

            Alter.Table("SupervisorDelegates").AlterColumn("CandidateID").AsInt32().Nullable()
                .ForeignKey("FK_SupervisorDelegates_CandidateID_Candidates_CandidateID", "DelegateAccounts", "ID");
            Alter.Table("CandidateAssessments").AlterColumn("CandidateID").AsInt32().NotNullable()
                .ForeignKey("FK_CandidateAssessments_CandidateID_Candidates_CandidateID", "DelegateAccounts", "ID");

            Delete.ForeignKey().FromTable("SupervisorDelegates").ForeignColumn("DelegateUserID")
                .ToTable("Users").PrimaryColumn("ID");
            Delete.Column("DelegateUserID").FromTable("SupervisorDelegates");

            Delete.ForeignKey().FromTable("CandidateAssessments").ForeignColumn("DelegateUserID")
                .ToTable("Users").PrimaryColumn("ID");
            Delete.Column("DelegateUserID").FromTable("CandidateAssessments");

            Delete.ForeignKey().FromTable("CandidateAssessments").ForeignColumn("CentreID")
                .ToTable("Centres").PrimaryColumn("CentreID");
            Delete.Column("CentreID").FromTable("CandidateAssessments");
        }
    }
}
