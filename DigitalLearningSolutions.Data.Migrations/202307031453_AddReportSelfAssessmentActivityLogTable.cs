namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202307031453)]
    public class AddReportSelfAssessmentActivityLogTable : Migration
    {
        public override void Up()
        {
            Create.Table("ReportSelfAssessmentActivityLog")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("DelegateID").AsInt32().Nullable()
                .ForeignKey("FK_ReportSelfAssessmentActivityLog_DelegateID_DelegateAccounts_ID", "DelegateAccounts", "ID").WithDefaultValue(0)
                .WithColumn("UserID").AsInt32().Nullable()
                .ForeignKey("FK_ReportSelfAssessmentActivityLog_UserID_Users_ID", "Users", "ID").WithDefaultValue(0)
                .WithColumn("CentreID").AsInt32().Nullable()
                .ForeignKey("FK_ReportSelfAssessmentActivityLog_CentreID_Centres_CentreID", "Centres", "CentreID").WithDefaultValue(0)
                .WithColumn("RegionID").AsInt32().Nullable()
                .ForeignKey("FK_ReportSelfAssessmentActivityLog_RegionID_Regions_RegionID", "Regions", "RegionID").WithDefaultValue(0)
                .WithColumn("JobGroupID").AsInt32().Nullable()
                .ForeignKey("FK_ReportSelfAssessmentActivityLog_JobGroupID_JobGroups_JobGroupID", "JobGroups", "JobGroupID").WithDefaultValue(0)
                .WithColumn("CategoryID").AsInt32().Nullable()
                .ForeignKey("FK_ReportSelfAssessmentActivityLog_CategoryID_CourseCategories_CourseCategoryID", "CourseCategories", "CourseCategoryID").WithDefaultValue(0)
                .WithColumn("National").AsBoolean().NotNullable().WithDefaultValue(0)
                .WithColumn("SelfAssessmentID").AsInt32().Nullable()
                .ForeignKey("FK_ReportSelfAssessmentActivityLog_SelfAssessmentID_SelfAssessments_ID", "SelfAssessments", "ID").WithDefaultValue(0)
                .WithColumn("ActivityDate").AsDateTime().NotNullable()
                .WithColumn("Enrolled").AsBoolean().NotNullable().WithDefaultValue(0)
                .WithColumn("Submitted").AsBoolean().NotNullable().WithDefaultValue(0)
                .WithColumn("SignedOff").AsBoolean().NotNullable().WithDefaultValue(0)
                ;
            //Insert enrolments into the new table:
            Execute.Sql(
                @$"INSERT INTO ReportSelfAssessmentActivityLog (DelegateID, UserID, CentreID, RegionID, JobGroupID, CategoryID, [National], SelfAssessmentID, ActivityDate, Enrolled, Submitted, SignedOff)
                    SELECT da.ID, ca.DelegateUserID, ca.CentreID, ce.RegionID, u.JobGroupID, sa.CategoryID, sa.[National], ca.SelfAssessmentID, ca.StartedDate, 1, 0, 0
                    FROM   CandidateAssessments AS ca INNER JOIN
                                 Users AS u ON ca.DelegateUserID = u.ID INNER JOIN
                                 SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                                 Centres AS ce ON ca.CentreID = ce.CentreID INNER JOIN
                                 DelegateAccounts AS da ON ca.DelegateUserID = da.UserID AND ca.CentreID = da.CentreID
                    WHERE (ca.NonReportable = 0);"
            );
            //Insert submitted self assessments into the new table:
            Execute.Sql(
                @$"INSERT INTO ReportSelfAssessmentActivityLog
                                 (DelegateID, UserID, CentreID, RegionID, JobGroupID, CategoryID, [National], SelfAssessmentID, ActivityDate, Enrolled, Submitted, SignedOff)
                   SELECT da.ID, ca.DelegateUserID, ca.CentreID, ce.RegionID, u.JobGroupID, sa.CategoryID, sa.[National], ca.SelfAssessmentID, ca.SubmittedDate, 0, 1, 0
                   FROM   CandidateAssessments AS ca INNER JOIN
                                 Users AS u ON ca.DelegateUserID = u.ID INNER JOIN
                                 SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                                 Centres AS ce ON ca.CentreID = ce.CentreID INNER JOIN
                                 DelegateAccounts AS da ON ca.DelegateUserID = da.UserID AND ca.CentreID = da.CentreID
                    WHERE (ca.NonReportable = 0) AND (ca.SubmittedDate IS NOT NULL);"
            );
            //Insert signed off self assessments into the new table:
            Execute.Sql(
               @$"INSERT INTO ReportSelfAssessmentActivityLog
                                 (DelegateID, UserID, CentreID, RegionID, JobGroupID, CategoryID, [National], SelfAssessmentID, ActivityDate, Enrolled, Submitted, SignedOff)
                   SELECT da.ID, ca.DelegateUserID, ca.CentreID, ce.RegionID, u.JobGroupID, sa.CategoryID, sa.[National], ca.SelfAssessmentID, casv.Verified, 0, 0, 1
                    FROM   CandidateAssessments AS ca INNER JOIN
                                 Users AS u ON ca.DelegateUserID = u.ID INNER JOIN
                                 SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                                 Centres AS ce ON ca.CentreID = ce.CentreID INNER JOIN
                                 DelegateAccounts AS da ON ca.DelegateUserID = da.UserID AND ca.CentreID = da.CentreID INNER JOIN
                                 CandidateAssessmentSupervisors AS cas ON ca.ID = cas.CandidateAssessmentID INNER JOIN
                                 CandidateAssessmentSupervisorVerifications AS casv ON cas.ID = casv.CandidateAssessmentSupervisorID
                   WHERE (ca.NonReportable = 0) AND (NOT (casv.Verified IS NULL)) AND (casv.SignedOff = 1);"
           );
            Execute.Sql(Properties.Resources.TD_2117_CreatePopulateReportSelfAssessmentActivityLog_SP);
        }

        public override void Down()
        {
            Execute.Sql(
               @$"DROP PROCEDURE [dbo].[PopulateReportSelfAssessmentActivityLog]"
           );
            Delete.Table("ReportSelfAssessmentActivityLog");
        }
    }
}
