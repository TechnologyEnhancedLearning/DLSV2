using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Common;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Services
{
    public interface ICommonService
    {
        //GET DATA
        IEnumerable<Brand> GetBrandListForCentre(int centreId);
        IEnumerable<Category> GetCategoryListForCentre(int centreId);
        IEnumerable<Topic> GetTopicListForCentre(int centreId);
        IEnumerable<Brand> GetAllBrands();
        IEnumerable<Category> GetAllCategories();
        IEnumerable<Topic> GetAllTopics();
        IEnumerable<(int, string)> GetCoreCourseCategories();
        IEnumerable<(int, string)> GetCentreTypes();
        IEnumerable<(int, string)> GetSelfAssessmentBrands(bool supervised);
        IEnumerable<(int, string)> GetSelfAssessmentCategories(bool supervised);
        IEnumerable<(int, string)> GetSelfAssessmentCentreTypes(bool supervised);
        IEnumerable<(int, string)> GetSelfAssessmentRegions(bool supervised);
        IEnumerable<(int, string)> GetAllRegions();
        IEnumerable<(int, string)> GetSelfAssessments(bool supervised);
        IEnumerable<(int, string)> GetSelfAssessmentCentres(bool supervised);
        IEnumerable<(int, string)> GetCourseCentres();
        IEnumerable<(int, string)> GetCoreCourseBrands();
        IEnumerable<(int, string)> GetCoreCourses();
        string? GetBrandNameById(int brandId);
        string? GetApplicationNameById(int applicationId);
        string? GetCategoryNameById(int categoryId);
        string? GetTopicNameById(int topicId);
        string? GenerateCandidateNumber(string firstName, string lastName);
        string? GetCentreTypeNameById(int centreTypeId);
        //INSERT DATA
        int InsertBrandAndReturnId(string brandName, int centreId);
        int InsertCategoryAndReturnId(string categoryName, int centreId);
        int InsertTopicAndReturnId(string topicName, int centreId);

    }
    public class CommonService : ICommonService
    {
        private readonly ICommonDataService commonDataService;
        public CommonService(ICommonDataService commonDataService)
        {
            this.commonDataService = commonDataService;
        }
        public string? GenerateCandidateNumber(string firstName, string lastName)
        {
            return commonDataService.GenerateCandidateNumber(firstName, lastName);
        }

        public IEnumerable<Brand> GetAllBrands()
        {
            return commonDataService.GetAllBrands();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return commonDataService.GetAllCategories();
        }

        public IEnumerable<(int, string)> GetAllRegions()
        {
            return commonDataService.GetAllRegions();
        }

        public IEnumerable<Topic> GetAllTopics()
        {
            return commonDataService.GetAllTopics();
        }

        public string? GetApplicationNameById(int applicationId)
        {
            return commonDataService.GetApplicationNameById(applicationId);
        }

        public IEnumerable<Brand> GetBrandListForCentre(int centreId)
        {
            return commonDataService.GetBrandListForCentre(centreId);
        }

        public string? GetBrandNameById(int brandId)
        {
            return commonDataService.GetBrandNameById(brandId);
        }

        public IEnumerable<Category> GetCategoryListForCentre(int centreId)
        {
            return commonDataService.GetCategoryListForCentre(centreId);
        }

        public string? GetCategoryNameById(int categoryId)
        {
            return commonDataService.GetCategoryNameById(categoryId);
        }

        public string? GetCentreTypeNameById(int centreTypeId)
        {
            return commonDataService.GetCentreTypeNameById(centreTypeId);
        }

        public IEnumerable<(int, string)> GetCentreTypes()
        {
            return commonDataService.GetCentreTypes();
        }

        public IEnumerable<(int, string)> GetCoreCourseBrands()
        {
            return commonDataService.GetCoreCourseBrands();
        }

        public IEnumerable<(int, string)> GetCoreCourseCategories()
        {
            return commonDataService.GetCoreCourseCategories();
        }

        public IEnumerable<(int, string)> GetCoreCourses()
        {
            return commonDataService.GetCoreCourses();
        }

        public IEnumerable<(int, string)> GetCourseCentres()
        {
            return commonDataService.GetCourseCentres();
        }

        public IEnumerable<(int, string)> GetSelfAssessmentBrands(bool supervised)
        {
            return commonDataService.GetSelfAssessmentBrands(supervised);
        }

        public IEnumerable<(int, string)> GetSelfAssessmentCategories(bool supervised)
        {
            return commonDataService.GetSelfAssessmentCategories(supervised);
        }

        public IEnumerable<(int, string)> GetSelfAssessmentCentres(bool supervised)
        {
            return commonDataService.GetSelfAssessmentCentres(supervised);
        }

        public IEnumerable<(int, string)> GetSelfAssessmentCentreTypes(bool supervised)
        {
            return commonDataService.GetSelfAssessmentCentreTypes(supervised);
        }

        public IEnumerable<(int, string)> GetSelfAssessmentRegions(bool supervised)
        {
            return commonDataService.GetSelfAssessmentRegions(supervised);
        }

        public IEnumerable<(int, string)> GetSelfAssessments(bool supervised)
        {
            return commonDataService.GetSelfAssessments(supervised);
        }

        public IEnumerable<Topic> GetTopicListForCentre(int centreId)
        {
            return commonDataService.GetTopicListForCentre(centreId);
        }

        public string? GetTopicNameById(int topicId)
        {
            return commonDataService.GetTopicNameById(topicId);
        }

        public int InsertBrandAndReturnId(string brandName, int centreId)
        {
            return commonDataService.InsertBrandAndReturnId(brandName, centreId);
        }

        public int InsertCategoryAndReturnId(string categoryName, int centreId)
        {
            return commonDataService.InsertCategoryAndReturnId(categoryName, centreId);
        }

        public int InsertTopicAndReturnId(string topicName, int centreId)
        {
            return commonDataService.InsertTopicAndReturnId(topicName, centreId);
        }
    }
}
