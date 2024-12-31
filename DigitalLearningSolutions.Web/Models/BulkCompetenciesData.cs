using DigitalLearningSolutions.Data.Models.Frameworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Models
{
    public class BulkCompetenciesData
    {
        public BulkCompetenciesData() { }
        public BulkCompetenciesData(DetailFramework framework, int adminUserId, string competenciesFileName, string tabName, bool isNotBlank)
        {
            FrameworkId = framework.ID;
            FrameworkName = framework.FrameworkName;
            PublishStatusID = framework.PublishStatusID;
            FrameworkVocubulary = framework.FrameworkConfig;
            AdminUserId = adminUserId;
            CompetenciesFileName = competenciesFileName;
            TabName = tabName;
            IsNotBlank = isNotBlank;
        }
        public int FrameworkId { get; set; }
        public string? FrameworkName { get; set; }
        public int PublishStatusID { get; set; }
        public string FrameworkVocubulary { get; set; }
        public string TabName { get; set; }
        public int AdminUserId { get; set; }
        public bool IsNotBlank { get; set; }
        public string CompetenciesFileName { get; set; }
        public List<int> DefaultQuestionIDs { get; set; } = [];
        public List<int> AssessmentQuestionIDs { get; set; } = [];
        public bool AddDefaultAssessmentQuestions { get; set; } = true;
        public bool AddCustomAssessmentQuestion { get; set; } = false;
        public int CompetenciesToProcessCount { get; set; }
        public int CompetenciesToAddCount { get; set; }
        public int CompetenciesToUpdateCount { get; set; }
        public int LastRowProcessed { get; set; }
        public int SubtotalCompetenciesAdded { get; set; }
        public int SubtotalCompetenciesUpdated { get; set; }
        public int SubTotalSkipped { get; set; }
        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; } = Enumerable.Empty<(int, string)>();
    }
}
