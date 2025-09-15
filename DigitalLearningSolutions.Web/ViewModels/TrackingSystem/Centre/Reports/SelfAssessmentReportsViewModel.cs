namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using System.Collections.Generic;

    public class SelfAssessmentReportsViewModel
    {
        public SelfAssessmentReportsViewModel(
            IEnumerable<SelfAssessmentSelect> selfAssessmentSelects, int? adminCategoryId, int categoryId, bool dSATreportIsPublish
            IEnumerable<SelfAssessmentSelect> selfAssessmentSelects, int? adminCategoryId, int categoryId, bool showTableauLink
            )
        {
            SelfAssessmentSelects = selfAssessmentSelects;
            AdminCategoryId = adminCategoryId;
            CategoryId = categoryId;
            DSATreportIsPublish = dSATreportIsPublish;
            ShowTableauLink = showTableauLink;
        }
        public IEnumerable<SelfAssessmentSelect> SelfAssessmentSelects { get; set; }
        public int? AdminCategoryId { get; set; }
        public int CategoryId { get; set; }
        public bool DSATreportIsPublish { get; set; }
        public bool ShowTableauLink { get; set; } = false;
    }
}
