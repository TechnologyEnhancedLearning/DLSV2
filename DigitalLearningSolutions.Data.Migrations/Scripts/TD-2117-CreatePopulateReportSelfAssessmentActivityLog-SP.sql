SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 03/07/2023
-- Description:	Populate the ReportSelfAssessmentActivityLog table with recent activity
-- =============================================
CREATE OR ALTER PROCEDURE PopulateReportSelfAssessmentActivityLog

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   DECLARE @StartDate datetime
SELECT @StartDate = MAX(ActivityDate) FROM ReportSelfAssessmentActivityLog;
--Insert enrolments into the new table:
                INSERT INTO ReportSelfAssessmentActivityLog
                (DelegateID, UserID, CentreID, RegionID, JobGroupID, CategoryID, [National], SelfAssessmentID, ActivityDate, Enrolled, Submitted, SignedOff)
                    SELECT da.ID, ca.DelegateUserID, ca.CentreID, ce.RegionID, u.JobGroupID, sa.CategoryID, sa.[National], ca.SelfAssessmentID, ca.StartedDate, 1, 0, 0
                    FROM   CandidateAssessments AS ca INNER JOIN
                                 Users AS u ON ca.DelegateUserID = u.ID INNER JOIN
                                 SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                                 Centres AS ce ON ca.CentreID = ce.CentreID INNER JOIN
                                 DelegateAccounts AS da ON ca.DelegateUserID = da.UserID AND ca.CentreID = da.CentreID
								 WHERE (ca.NonReportable = 0) AND (ca.StartedDate > @StartDate);
            --Insert submitted self assessments into the new table:
INSERT INTO ReportSelfAssessmentActivityLog
                                 (DelegateID, UserID, CentreID, RegionID, JobGroupID, CategoryID, [National], SelfAssessmentID, ActivityDate, Enrolled, Submitted, SignedOff)
                   SELECT da.ID, ca.DelegateUserID, ca.CentreID, ce.RegionID, u.JobGroupID, sa.CategoryID, sa.[National], ca.SelfAssessmentID, ca.SubmittedDate, 0, 1, 0
                   FROM   CandidateAssessments AS ca INNER JOIN
                                 Users AS u ON ca.DelegateUserID = u.ID INNER JOIN
                                 SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                                 Centres AS ce ON ca.CentreID = ce.CentreID INNER JOIN
                                 DelegateAccounts AS da ON ca.DelegateUserID = da.UserID AND ca.CentreID = da.CentreID
                    WHERE (ca.NonReportable = 0) AND (NOT (ca.SubmittedDate IS NULL)) AND (ca.SubmittedDate > @StartDate);
            --Insert signed off self assessments into the new table:
INSERT INTO ReportSelfAssessmentActivityLog
                                 (DelegateID, UserID, CentreID, RegionID, JobGroupID, CategoryID, [National], SelfAssessmentID, ActivityDate, Enrolled, Submitted, SignedOff)
                   SELECT da.ID, ca.DelegateUserID, ca.CentreID, ce.RegionID, u.JobGroupID, sa.CategoryID, sa.[National], ca.SelfAssessmentID, casv.Verified, 0, 0, 1
                    FROM   CandidateAssessments AS ca INNER JOIN
                                 Users AS u ON ca.DelegateUserID = u.ID INNER JOIN
                                 SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                                 Centres AS ce ON ca.CentreID = ce.CentreID INNER JOIN
                                 DelegateAccounts AS da ON ca.DelegateUserID = da.UserID AND ca.CentreID = da.CentreID INNER JOIN
                                 CandidateAssessmentSupervisors AS cas ON ca.ID = cas.CandidateAssessmentID INNER JOIN
                                 CandidateAssessmentSupervisorVerifications AS casv ON cas.ID = casv.CandidateAssessmentSupervisorID
                   WHERE (ca.NonReportable = 0) AND (NOT (casv.Verified IS NULL)) AND (casv.SignedOff = 1) AND (casv.Verified > @StartDate);
END
GO
