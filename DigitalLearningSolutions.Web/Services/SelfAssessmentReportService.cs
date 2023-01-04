namespace DigitalLearningSolutions.Data.Services
{
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public interface ISelfAssessmentReportService
    {
        byte[] GetDigitalCapabilityExcelExportForCentre(int centreId);
    }
    public class SelfAssessmentReportService : ISelfAssessmentReportService
    {
        private readonly IDCSAReportDataService dcsaReportDataService;

        public SelfAssessmentReportService(
            IDCSAReportDataService dcsaReportDataService
        )
        {
            this.dcsaReportDataService = dcsaReportDataService;
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
                    x.Email,
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
            AddSheetToWorkbook(workbook, "Delegate Completion Status", delegateCompletionStatus);
            AddSheetToWorkbook(workbook, "Assessment Outcome Summary", outcomeSummary);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        private static void AddSheetToWorkbook(IXLWorkbook workbook, string sheetName, IEnumerable<object>? dataObjects)
        {
            var sheet = workbook.Worksheets.Add(sheetName);
            var table = sheet.Cell(1, 1).InsertTable(dataObjects);
            table.Theme = XLTableTheme.TableStyleLight9;
            sheet.Columns().AdjustToContents();
        }
    }
}
