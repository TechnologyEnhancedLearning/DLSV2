namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Attributes;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Linq;

    public class SupervisorDelegateViewModel
    {
        public SupervisorDelegateViewModel(SupervisorDelegateDetail detail, ReturnPageQuery returnPageQuery, IEnumerable<Category> categories, int? adminCategoryId)
        {
            Id = detail.ID;
            FirstName = detail.FirstName;
            LastName = detail.LastName;
            DelegateEmail = detail.DelegateEmail;
            CandidateAssessmentCount = detail.CandidateAssessmentCount;
            ReturnPageQuery = returnPageQuery;
            SelfAssessmentCategory = AdminCategoryHelper.CategoryIdToAdminCategory(adminCategoryId);
            SelfAssessmentCategories = SelectListHelper.MapOptionsToSelectListItems(
                categories.Select(c => (c.CourseCategoryID, c.CategoryName)),
                adminCategoryId
            );
        }
        public SupervisorDelegateViewModel(SupervisorDelegateDetail detail, ReturnPageQuery returnPageQuery)
        {
            Id = detail.ID;
            FirstName = detail.FirstName;
            LastName = detail.LastName;
            DelegateEmail = detail.DelegateEmail;
            CandidateAssessmentCount = detail.CandidateAssessmentCount;
            ReturnPageQuery = returnPageQuery;
        }
        public SupervisorDelegateViewModel() { }
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int CandidateAssessmentCount { get; set; }
        public string DelegateEmail { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "Please tick the checkbox to confirm you wish to perform this action")]
        public bool ActionConfirmed { get; set; }
        [DefaultValue(false)]
        public bool ConfirmedRemove { get; set; }
        public int SelfAssessmentCategory { get; set; }
        public IEnumerable<SelectListItem>? SelfAssessmentCategories { get; set; }
    }
}
