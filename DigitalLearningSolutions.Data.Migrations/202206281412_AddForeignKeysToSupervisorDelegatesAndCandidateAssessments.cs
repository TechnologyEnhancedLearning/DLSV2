namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202206281412)]
    public class AddForeignKeysToSupervisorDelegatesAndCandidateAssessments : Migration
    {
        public override void Up()
        {
            // TODO HEEDLS-932 does the column name want to be capitalised?
            Alter.Table("SupervisorDelegates")
                .AddColumn("DelegateUserID").AsInt32().Nullable()
                .ForeignKey("Users", "ID");

            Alter.Table("CandidateAssessments")
                .AddColumn("DelegateUserID").AsInt32().Nullable()
                .ForeignKey("Users", "ID")
                .AddColumn("CentreID").AsInt32().Nullable()
                .ForeignKey("Centres", "CentreID");

            // script to populate columns
            Execute.Script("DigitalLearningSolutions.Data.Migrations/Scripts/HEEDLS-932-PopulateNewKeysOnSupervisorDelegatesAndCandidateAssessments.sql");

            // remove null option on columns
            Alter.Table("CandidateAssessments")
                .AlterColumn("DelegateUserID").AsInt32().NotNullable()
                .AlterColumn("CentreID").AsInt32().NotNullable();

            // remove constraints
            Delete.ForeignKey().FromTable("SupervisorDelegates").ForeignColumn("CandidateId")
                .ToTable("Candidates").PrimaryColumn("CandidateID");
            Alter.Table("SupervisorDelegates").AlterColumn("CandidateId").AsInt32().Nullable();

            Delete.ForeignKey().FromTable("CandidateAssessments").ForeignColumn("CandidateID")
                .ToTable("Candidates").PrimaryColumn("CandidateID");
            Alter.Table("CandidateAssessments").AlterColumn("CandidateID").AsInt32().Nullable();

            // rename column
            Rename.Column("CandidateID").OnTable("SupervisorDelegates").To("CandidateID_deprecated");
            Rename.Column("CandidateID").OnTable("CandidateAssessments").To("CandidateID_deprecated");
        }

        public override void Down()
        {
            Rename.Column("CandidateID_deprecated").OnTable("SupervisorDelegates").To("CandidateID");
            Rename.Column("CandidateID_deprecated").OnTable("CandidateAssessments").To("CandidateID");

            Alter.Table("SupervisorDelegates").AlterColumn("CandidateID").AsInt32().Nullable()
                .ForeignKey("DelegateAccounts", "ID");

            Alter.Table("CandidateAssessments").AlterColumn("CandidateID").AsInt32().Nullable()
                .ForeignKey("DelegateAccounts", "ID");

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
