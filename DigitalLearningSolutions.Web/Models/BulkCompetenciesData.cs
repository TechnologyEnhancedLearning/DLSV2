using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Data.Models.Frameworks.Import;
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
        public bool AddDefaultAssessmentQuestions { get; set; } = true;
        public bool AddCustomAssessmentQuestion { get; set; } = false;
        public List<int> DefaultQuestionIDs { get; set; } = [];
        public int? CustomAssessmentQuestionID { get; set; }
        public int AddAssessmentQuestionsOption { get; set; } //1 = only added, 2 = added and updated, 3 = all uploaded
        public int CompetenciesToProcessCount { get; set; }
        public int CompetenciesToAddCount { get; set; }
        public int CompetenciesToUpdateCount { get; set; }
        public int CompetenciesToReorderCount { get; set; }
        public int ReorderCompetenciesOption { get; set; } = 1; //1 = ignore order, 2 = apply order
        public int LastRowProcessed { get; set; }
        public int SubtotalCompetenciesAdded { get; set; }
        public int SubtotalCompetenciesUpdated { get; set; }
        public int SubTotalSkipped { get; set; }
        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; } = Enumerable.Empty<(int, string)>();
        public ImportCompetenciesResult ImportCompetenciesResult { get;set;}
    }
}
