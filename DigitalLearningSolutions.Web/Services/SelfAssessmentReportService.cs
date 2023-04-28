namespace DigitalLearningSolutions.Data.Services
{
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public interface ISelfAssessmentReportService
    {
        byte[] GetDigitalCapabilityExcelExportForCentre(int centreId);
        byte[] GetSelfAssessmentExcelExportForCentre(int centreId, int selfAssessmentId);
        IEnumerable<SelfAssessmentSelect> GetSelfAssessmentsForReportList(int centreId, int? categoryId);
    }
    public class SelfAssessmentReportService : ISelfAssessmentReportService
    {
        private readonly IDCSAReportDataService dcsaReportDataService;
        private readonly ISelfAssessmentReportDataService selfAssessmentReportDataService;
        public SelfAssessmentReportService(
            IDCSAReportDataService dcsaReportDataService,
            ISelfAssessmentReportDataService selfAssessmentReportDataService
        )
        {
            this.dcsaReportDataService = dcsaReportDataService;
            this.selfAssessmentReportDataService = selfAssessmentReportDataService;
        }
        private static void AddSheetToWorkbook(IXLWorkbook workbook, string sheetName, IEnumerable<object>? dataObjects)
        {
            var sheet = workbook.Worksheets.Add(sheetName);
            var table = sheet.Cell(1, 1).InsertTable(dataObjects);
            table.Theme = XLTableTheme.TableStyleLight9;
            sheet.Columns().AdjustToContents();
        }
        public byte[] GetDigitalCapabilityExcelExportForCentre(int centreId)
        {
            var delegateCompletionStatus = dcsaReportDataService.GetDelegateCompletionStatusForCentre(centreId);
            var outcomeSummary = dcsaReportDataService.GetOutcomeSummaryForCentre(centreId);
            var summary = delegateCompletionStatus.Select(
                x => new
                {
                    x.EnrolledMonth,
                    x.EnrolledYear,
                    x.FirstName,
                    x.LastName,
                    Email = (Guid.TryParse(x.Email, out _) ? string.Empty : x.Email),
                    x.CentreField1,
                    x.CentreField2,
                    x.CentreField3,
                    x.Status
                }
                );
            var details = outcomeSummary.Select(
                x => new
                {
                    x.EnrolledMonth,
                    x.EnrolledYear,
                    x.JobGroup,
                    x.CentreField1,
                    x.CentreField2,
                    x.CentreField3,
                    x.Status,
                    x.LearningLaunched,
                    x.LearningCompleted,
                    x.DataInformationAndContentConfidence,
                    x.DataInformationAndContentRelevance,
                    x.TeachinglearningAndSelfDevelopmentConfidence,
                    x.TeachinglearningAndSelfDevelopmentRelevance,
                    x.CommunicationCollaborationAndParticipationConfidence,
                    x.CommunicationCollaborationAndParticipationRelevance,
                    x.TechnicalProficiencyConfidence,
                    x.TechnicalProficiencyRelevance,
                    x.CreationInnovationAndResearchConfidence,
                    x.CreationInnovationAndResearchRelevance,
                    x.DigitalIdentityWellbeingSafetyAndSecurityConfidence,
                    x.DigitalIdentityWellbeingSafetyAndSecurityRelevance
                }
                );
            using var workbook = new XLWorkbook();
            AddSheetToWorkbook(workbook, "Delegate Completion Status", summary);
            AddSheetToWorkbook(workbook, "Assessment Outcome Summary", outcomeSummary);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public IEnumerable<SelfAssessmentSelect> GetSelfAssessmentsForReportList(int centreId, int? categoryId)
        {
            return selfAssessmentReportDataService.GetSelfAssessmentsForReportList(centreId, categoryId);
        }

        public byte[] GetSelfAssessmentExcelExportForCentre(int centreId, int selfAssessmentId)
        {
            var selfAssessmentReportData = selfAssessmentReportDataService.GetSelfAssessmentReportDataForCentre(centreId, selfAssessmentId);
            var reportData = selfAssessmentReportData.Select(
                      x => new
                      {
                          x.SelfAssessment,
                          x.Learner,
                          x.LearnerActive,
                          x.PRN,
                          x.JobGroup,
                          x.ProgrammeCourse,
                          x.Organisation,
                          x.DepartmentTeam,
                          x.DLSRole,
                          x.Registered,
                          x.Started,
                          x.LastAccessed,
                          x.OptionalProficienciesAssessed,
                          x.SelfAssessedAchieved,
                          x.ConfirmedResults,
                          x.SignOffRequested,
                          x.SignOffAchieved,
                          x.ReviewedDate
                      }
                      );
            using var workbook = new XLWorkbook();
            AddSheetToWorkbook(workbook, "SelfAssessmentLearners", reportData);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
