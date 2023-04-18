namespace DigitalLearningSolutions.Data.Services
{
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.Export;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public interface ICandidateAssessmentDownloadFileService
    {
        public byte[] GetCandidateAssessmentDownloadFileForCentre(int candidateAssessmentId, int delegateUserId, bool isProtected);
    }

    public class CandidateAssessmentDownloadFileService : ICandidateAssessmentDownloadFileService
    {
        private readonly ISelfAssessmentDataService selfAssessmentDataService;
        private readonly IConfiguration config;

        public CandidateAssessmentDownloadFileService(
            ISelfAssessmentDataService selfAssessmentDataService,
            IConfiguration config
        )
        {
            this.selfAssessmentDataService = selfAssessmentDataService;
            this.config = config;
        }
        public byte[] GetCandidateAssessmentDownloadFileForCentre(int candidateAssessmentId, int delegateUserId, bool isProtected)
        {
            var summaryData = selfAssessmentDataService.GetCandidateAssessmentExportSummary(candidateAssessmentId, delegateUserId);
            var detailData = selfAssessmentDataService.GetCandidateAssessmentExportDetails(candidateAssessmentId, delegateUserId);
            var details = detailData.Select(
                x => new
                {
                    x.Group,
                    x.Competency,
                    x.CompetencyOptional,
                    x.AssessmentQuestion,
                    x.SelfAssessmentRequired,
                    x.SelfAssessmentResult,
                    x.SelfAssessmentComments,
                    x.Reviewer,
                    x.ReviewerPrn,
                    x.Reviewed,
                    x.ReviewerComments,
                    x.ReviewerVerified,
                    x.RoleRequirements
                }
            );
            using var workbook = new XLWorkbook();
            var excelPassword = config.GetExcelPassword();
            AddSummarySheet(workbook, summaryData, excelPassword, isProtected);
            AddSheetToWorkbook(workbook, "Details", details, excelPassword, isProtected);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        private static void AddSheetToWorkbook(IXLWorkbook workbook, string sheetName, IEnumerable<object>? dataObjects, string excelPassword, bool isProtected)
        {
            var sheet = workbook.Worksheets.Add(sheetName);
            var table = sheet.Cell(1, 1).InsertTable(dataObjects);
            table.Theme = XLTableTheme.TableStyleLight9;
            sheet.Columns().AdjustToContents();
            if (isProtected)
            {
                sheet.Protect(excelPassword);
                sheet.Columns().Style.Protection.SetLocked(true);
            }
        }
        private static void AddSummarySheet(IXLWorkbook workbook, CandidateAssessmentExportSummary candidateAssessmentExportSummary, string excelPassword, bool isProtected)
        {
            var sheet = workbook.Worksheets.Add("Summary");
            var rowNum = 1;
            sheet.Cell(rowNum, 1).Value = "Learner";
            sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.CandidateName;
            rowNum++;
            sheet.Cell(rowNum, 1).Value = "Learner PRN";
            sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            sheet.Cell(rowNum, 2).Value = (!string.IsNullOrEmpty(candidateAssessmentExportSummary.CandidatePrn) ? candidateAssessmentExportSummary.CandidatePrn.ToString() : "Not Recorded");
            rowNum++;
            sheet.Cell(rowNum, 1).Value = "Self Assessment";
            sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.SelfAssessment;
            rowNum++;
            sheet.Cell(rowNum, 1).Value = "Start Date";
            sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.StartDate;
            rowNum++;
            sheet.Cell(rowNum, 1).Value = "Self assessment questions";
            sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.QuestionCount;
            rowNum++;
            sheet.Cell(rowNum, 1).Value = "Self assessment responses";
            sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.SelfAssessmentResponseCount;
            rowNum++;
            sheet.Cell(rowNum, 1).Value = "Responses verified";
            sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.ResponsesVerifiedCount;
            rowNum++;
            if (candidateAssessmentExportSummary.QuestionCount > candidateAssessmentExportSummary.NoRequirementsSetCount)
            {
                if (candidateAssessmentExportSummary.NoRequirementsSetCount > 0)
                {
                    sheet.Cell(rowNum, 1).Value = "Questions with no role requirements set";
                    sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.NoRequirementsSetCount;
                    rowNum++;

                }
                if (candidateAssessmentExportSummary.MeetingCount > 0)
                {
                    sheet.Cell(rowNum, 1).Value = "Responses fully meeting role requirements";
                    sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.MeetingCount;
                    rowNum++;
                }
                if (candidateAssessmentExportSummary.PartiallyMeetingCount > 0)
                {
                    sheet.Cell(rowNum, 1).Value = "Responses partially meeting role requirements";
                    sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.PartiallyMeetingCount;
                    rowNum++;
                }
                if (candidateAssessmentExportSummary.NotMeetingCount > 0)
                {
                    sheet.Cell(rowNum, 1).Value = "Responses not meeting role requirements";
                    sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.NotMeetingCount;
                    rowNum++;
                }
            }
            if (candidateAssessmentExportSummary.SignedOff != null)
            {
                sheet.Cell(rowNum, 1).Value = "Sign off status";
                sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                sheet.Cell(rowNum, 2).Value = "This self assessment has been signed off";
                rowNum++;
                sheet.Cell(rowNum, 1).Value = "Sign off date";
                sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.SignedOff;
                rowNum++;
                sheet.Cell(rowNum, 1).Value = "Signed off by";
                sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                sheet.Cell(rowNum, 2).Value = candidateAssessmentExportSummary.Signatory;
                rowNum++;
                sheet.Cell(rowNum, 1).Value = "Signatory PRN";
                sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                sheet.Cell(rowNum, 2).Value = (!string.IsNullOrEmpty(candidateAssessmentExportSummary.SignatoryPrn) ? "Recorded" : "Not Recorded");
                rowNum++;
            }
            else
            {
                sheet.Cell(rowNum, 1).Value = "Sign off status";
                sheet.Cell(rowNum, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                sheet.Cell(rowNum, 2).Value = "This self assessment has not been signed off";
                rowNum++;
            }
            sheet.Cells(true).Style.Font.FontSize = 16;
            sheet.Rows().AdjustToContents();
            sheet.Columns().AdjustToContents();
            sheet.Columns("2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).Font.SetBold();
            if (isProtected)
            {
                sheet.Protect(excelPassword);
                sheet.Columns().Style.Protection.SetLocked(true);
            }
        }
    }
}
