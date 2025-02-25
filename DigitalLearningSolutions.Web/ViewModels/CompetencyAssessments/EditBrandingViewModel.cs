using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class EditBrandingViewModel
    {
        public EditBrandingViewModel() { }
        public EditBrandingViewModel(CompetencyAssessment competencyAssessment, SelectList brandSelectList, SelectList categorySelectList, bool? taskStatus)
        {
            ID = competencyAssessment.ID;
            CompetencyAssessmentName = competencyAssessment.CompetencyAssessmentName;
            BrandID = competencyAssessment.BrandID;
            CategoryID = competencyAssessment.CategoryID;
            Brand = competencyAssessment.Brand;
            Category = competencyAssessment.Category;
            UserRole = competencyAssessment.UserRole;
            BrandSelectList = brandSelectList;
            CategorySelectList = categorySelectList;
            TaskStatus = taskStatus;
        }
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public int BrandID { get; set; }
        public int CategoryID { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public int UserRole { get; set; }
        public bool? TaskStatus { get; set; }
        public SelectList? BrandSelectList { get; set; }
        public SelectList? CategorySelectList { get; set; }
    }
}
