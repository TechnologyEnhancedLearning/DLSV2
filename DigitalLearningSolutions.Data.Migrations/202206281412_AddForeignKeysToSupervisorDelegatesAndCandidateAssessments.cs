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
                .AddColumn("DelegateUserId").AsInt32().Nullable()
                .ForeignKey("Users", "ID");

            Alter.Table("CandidateAssessments")
                .AddColumn("DelegateUserId").AsInt32().Nullable()
                .ForeignKey("Users", "ID")
                .AddColumn("CentreId").AsInt32().Nullable()
                .ForeignKey("Centres", "ID");

            // script to populate columns

            // remove null option on columns
            Alter.Table("SupervisorDelegates")
                .AlterColumn("DelegateUserId").AsInt32().NotNullable();
            Alter.Table("CandidateAssessments")
                .AlterColumn("DelegateUserId").AsInt32().NotNullable()
                .AlterColumn("CentreId").AsInt32().NotNullable();

            // remove constraints
            Delete.ForeignKey().FromTable("SupervisorDelegates").ForeignColumn("CandidateId")
                .ToTable("Candidates").PrimaryColumn("CandidateId");
            Alter.Table("SupervisorDelegates").AlterColumn("CandidateId").AsInt32().Nullable();

            Delete.ForeignKey().FromTable("CandidateAssessments").ForeignColumn("CandidateId")
                .ToTable("Candidates").PrimaryColumn("CandidateId");
            Alter.Table("CandidateAssessments").AlterColumn("CandidateId").AsInt32().Nullable();

            // rename column
            Rename.Column("CandidateId").OnTable("SupervisorDelegates").To("CandidateId_deprecated");
            Rename.Column("CandidateId").OnTable("CandidateAssessments").To("CandidateId_deprecated");
        }

        public override void Down()
        {
            Rename.Column("CandidateId_deprecated").OnTable("SupervisorDelegates").To("CandidateId");
            Rename.Column("CandidateId_deprecated").OnTable("CandidateAssessments").To("CandidateId");

            // this will fail if there is null data, what do?
            Alter.Table("SupervisorDelegates").AlterColumn("CandidateId").AsInt32().Nullable();
        }
    }
}
