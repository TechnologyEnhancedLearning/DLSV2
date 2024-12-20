﻿using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Models
{
    public class BulkCompetenciesData
    {
        public BulkCompetenciesData() { }
        public BulkCompetenciesData(int frameworkId, int adminUserId, string competenciesFileName, string tabName)
        {
            FrameworkId = frameworkId;
            AdminUserId = adminUserId;
            CompetenciesFileName = competenciesFileName;
            TabName = tabName;
        }
        public int FrameworkId { get; set; }
        public string TabName { get; set; }
        public int AdminUserId { get; set; }
        public string CompetenciesFileName { get; set; }
        public List<int> AssessmentQuestionIDs { get; set; }
        public int? AddAssessmentQuestionOption { get; set; }
        public int ToProcessCount { get; set; }
        public int ToAddCount { get; set; }
        public int ToUpdateCount { get; set; }
        public int LastRowProcessed { get; set; }
        public int SubtotalCompetenciesAdded { get; set; }
        public int SubtotalCompetenciesUpdated { get; set; }
        public int SubTotalSkipped { get; set; }
        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; } = Enumerable.Empty<(int, string)>();
    }
}
