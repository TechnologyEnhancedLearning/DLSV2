namespace DigitalLearningSolutions.Web.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IEvaluationSummaryService
    {
        IEnumerable<EvaluationResponseBreakdown> GetEvaluationSummary(int centreId, ActivityFilterData filterData);
        byte[] GetEvaluationSummaryFileForCentre(int centreId, ActivityFilterData filterData);
    }

    public class EvaluationSummaryService : IEvaluationSummaryService
    {
        private const string SheetName = "Evaluation Statistics";
        private readonly IEvaluationSummaryDataService evaluationSummaryDataService;

        public EvaluationSummaryService(IEvaluationSummaryDataService evaluationSummaryDataService)
        {
            this.evaluationSummaryDataService = evaluationSummaryDataService;
        }

        public IEnumerable<EvaluationResponseBreakdown> GetEvaluationSummary(
            int centreId,
            ActivityFilterData filterData
        )
        {
            var evaluationSummaryData = evaluationSummaryDataService.GetEvaluationSummaryData(
                centreId,
                filterData.StartDate,
                filterData.EndDate,
                filterData.JobGroupId,
                filterData.CourseCategoryId,
                filterData.CustomisationId
            );
            return MapDataToEvaluationResponseBreakdowns(evaluationSummaryData);
        }

        public byte[] GetEvaluationSummaryFileForCentre(int centreId, ActivityFilterData filterData)
        {
            using var workbook = new XLWorkbook();

            var evaluationSummaries = GetEvaluationSummary(centreId, filterData).ToList();

            var sheet = workbook.Worksheets.Add(SheetName);

            sheet.Cell(1, 1).Value = "Total responses";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 2).Value = evaluationSummaries.First().TotalResponses;

            AddQuestionTableToSheet(sheet, 3, evaluationSummaries[0]);
            AddQuestionTableToSheet(sheet, 8, evaluationSummaries[1]);
            AddQuestionTableToSheet(sheet, 13, evaluationSummaries[2]);
            AddQuestionTableToSheet(sheet, 18, evaluationSummaries[3]);
            AddQuestionTableToSheet(sheet, 27, evaluationSummaries[4]);
            AddQuestionTableToSheet(sheet, 32, evaluationSummaries[5]);
            AddQuestionTableToSheet(sheet, 38, evaluationSummaries[6]);

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static void AddQuestionTableToSheet(
            IXLWorksheet sheet,
            int questionRow,
            EvaluationResponseBreakdown evaluationSummary
        )
        {
            sheet.Cell(questionRow, 1).Value = evaluationSummary.Question;
            sheet.Cell(questionRow, 1).Style.Font.Bold = true;
            var range = sheet.Cell(questionRow + 1, 1).InsertData(evaluationSummary.Responses);
            range.Column(3).Style.NumberFormat.Format = "0.0%";
        }

        private static IEnumerable<EvaluationResponseBreakdown> MapDataToEvaluationResponseBreakdowns(
            EvaluationAnswerCounts data
        )
        {
            var q1 = new EvaluationResponseBreakdown(
                "Increased productivity?",
                new List<(string response, int count)>
                {
                    ("Yes", data.Q1Yes),
                    ("No", data.Q1No),
                    ("No response", data.Q1NoResponse)
                }
            );
            var q2 = new EvaluationResponseBreakdown(
                "Gained new skills?",
                new List<(string response, int count)>
                {
                    ("Yes", data.Q2Yes),
                    ("No", data.Q2No),
                    ("No response", data.Q2NoResponse)
                }
            );
            var q3 = new EvaluationResponseBreakdown(
                "Perform faster?",
                new List<(string response, int count)>
                {
                    ("Yes", data.Q3Yes),
                    ("No", data.Q3No),
                    ("No response", data.Q3NoResponse)
                }
            );
            var q4 = new EvaluationResponseBreakdown(
                "Time saving per week",
                new List<(string response, int count)>
                {
                    ("0 hrs", data.Q4Hrs0),
                    ("Less than 1 hr", data.Q4HrsLt1),
                    ("1 to 2 hrs", data.Q4Hrs1To2),
                    ("2 to 4 hrs", data.Q4Hrs2To4),
                    ("4 to 6 hrs", data.Q4Hrs4To6),
                    ("More than 6 hrs", data.Q4HrsGt6),
                    ("No response", data.Q4NoResponse)
                }
            );
            var q5 = new EvaluationResponseBreakdown(
                "Pass on skills?",
                new List<(string response, int count)>
                {
                    ("Yes", data.Q5Yes),
                    ("No", data.Q5No),
                    ("No response", data.Q5NoResponse)
                }
            );
            var q6 = new EvaluationResponseBreakdown(
                "Help with patients/clients?",
                new List<(string response, int count)>
                {
                    ("Yes, directly", data.Q6YesDirectly),
                    ("Yes, indirectly", data.Q6YesIndirectly),
                    ("No", data.Q6No),
                    ("No response", data.Q6NoResponse)
                }
            );
            var q7 = new EvaluationResponseBreakdown(
                "Recommended materials?",
                new List<(string response, int count)>
                {
                    ("Yes", data.Q7Yes),
                    ("No", data.Q7No),
                    ("No response", data.Q7NoResponse)
                }
            );
            return new List<EvaluationResponseBreakdown> { q1, q2, q3, q4, q5, q6, q7 };
        }
    }
}
