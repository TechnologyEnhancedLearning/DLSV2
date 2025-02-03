using ClosedXML.Excel;
using DigitalLearningSolutions.Data.Exceptions;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.ViewModels.Frameworks.Import;
using GDS.MultiPageFormData.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [Route("/Framework/{frameworkId}/{tabname}/Import")]
        public IActionResult ImportCompetencies(int frameworkId, string tabname, bool isNotBlank)
        {
            var adminId = GetAdminId();
            var framework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
                return StatusCode(403);
            var model = new ImportCompetenciesViewModel(framework, isNotBlank);

            return View("Developer/Import/Index", model);
        }
        public IActionResult DownloadCompetencies(int frameworkId, int DownloadOption, string vocabulary)
        {
            string fileName = DownloadOption == 2 ? $"DLS {FrameworkVocabularyHelper.VocabularyPlural(vocabulary)} for Bulk Update {clockUtility.UtcToday:yyyy-MM-dd}.xlsx" : $"DLS {FrameworkVocabularyHelper.VocabularyPlural(vocabulary)} for Bulk Upload.xlsx";
            var content = importCompetenciesFromFileService.GetCompetencyFileForFramework(
                    frameworkId, DownloadOption == 2 ? false : true, vocabulary
                );
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/{tabname}/Import")]
        [Route("/Framework/{frameworkId}/{tabname}/Import/Uploaded")]
        public IActionResult StartImport(ImportCompetenciesFormData model, int frameworkId, string tabname, bool isNotBlank)
        {
            if (!ModelState.IsValid)
                return View("Developer/Import/Index", model);
            try
            {
                var adminUserID = User.GetAdminIdKnownNotNull();
                var workbook = new XLWorkbook(model.ImportFile.OpenReadStream());
                if (!workbook.Worksheets.Contains(ImportCompetenciesFromFileService.CompetenciesSheetName))
                {
                    ModelState.AddModelError("ImportFile", CommonValidationErrorMessages.InvalidCompetenciesUploadExcelFile);
                    var adminId = GetAdminId();
                    var framework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
                    var viewModel = new ImportCompetenciesViewModel(framework, isNotBlank);
                    return View("Developer/Import/Index", viewModel);
                }
                var competenciesFileName = FileHelper.UploadFile(webHostEnvironment, model.ImportFile);
                setupBulkUploadData(frameworkId, adminUserID, competenciesFileName, tabname, isNotBlank);

                return RedirectToAction("ImportCompleted", "Frameworks", new { frameworkId, tabname });
            }
            catch (DocumentFormat.OpenXml.Packaging.OpenXmlPackageException)
            {
                ModelState.AddModelError("DelegatesFile", "The Excel file has at least one cell containing an invalid hyperlink or email address.");
                var adminId = GetAdminId();
                var framework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
                var viewModel = new ImportCompetenciesViewModel(framework, isNotBlank);
                return View("Developer/Import/Index", viewModel);
            }
            catch (InvalidHeadersException)
            {
                return View("Developer/Import/ImportFailed");
            }
        }
        [Route("/Framework/{frameworkId}/{tabname}/Import/Uploaded")]
        public IActionResult ImportCompleted()
        {
            var data = GetBulkUploadData();
            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
            var filePath = Path.Combine(uploadDir, data.CompetenciesFileName);
            var workbook = new XLWorkbook(filePath);
            try
            {
                var results = importCompetenciesFromFileService.PreProcessCompetenciesTable(workbook, data.FrameworkVocubulary, data.FrameworkId);
                var resultsModel = new ImportCompetenciesPreProcessViewModel(results, data) { IsNotBlank = data.IsNotBlank, TabName = data.TabName };
                data.CompetenciesToProcessCount = resultsModel.ToProcessCount;
                data.CompetenciesToAddCount = resultsModel.CompetenciesToAddCount;
                data.CompetenciesToUpdateCount = resultsModel.ToUpdateOrSkipCount;
                data.CompetenciesToReorderCount = results.CompetencyReorderedCount;
                setBulkUploadData(data);
                return View("Developer/Import/ImportCompleted", resultsModel);
            }
            catch (InvalidHeadersException)
            {
                FileHelper.DeleteFile(webHostEnvironment, data.CompetenciesFileName);
                return View("Developer/Import/ImportFailed");
            }
        }
        [Route("/Framework/{frameworkId}/{tabname}/Import/Ordering")]
        public IActionResult ApplyCompetencyOrdering()
        {
            var data = GetBulkUploadData();
            if (data.CompetenciesToReorderCount > 0)
            {
                var model = new ApplyCompetencyOrderingViewModel(data.FrameworkId, data.FrameworkName, data.FrameworkVocubulary, data.CompetenciesToReorderCount, data.ReorderCompetenciesOption);
                return View("Developer/Import/ApplyCompetencyOrdering", model);
            }
            return RedirectToAction("AddAssessmentQuestions", "Frameworks", new { frameworkId = data.FrameworkId, tabname = data.TabName });
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/{tabname}/Import/Ordering")]
        public IActionResult ApplyCompetencyOrdering(int reorderCompetenciesOption)
        {
            var data = GetBulkUploadData();

            if (data.ReorderCompetenciesOption != reorderCompetenciesOption)
            {
                data.ReorderCompetenciesOption = reorderCompetenciesOption;
                setBulkUploadData(data);
            }
            return RedirectToAction("AddAssessmentQuestions", "Frameworks", new { frameworkId = data.FrameworkId, tabname = data.TabName });
        }
        [Route("/Framework/{frameworkId}/{tabname}/Import/AssessmentQuestions")]
        public IActionResult AddAssessmentQuestions()
        {
            var data = GetBulkUploadData();
            var adminId = GetAdminId();
            var defaultQuestions = frameworkService.GetFrameworkDefaultQuestionsById(data.FrameworkId, adminId);
            if (!data.DefaultQuestionIDs.Any() && defaultQuestions.Any() && data.AddDefaultAssessmentQuestions == true)
            {
                var defaultQuestionsList = new List<int>();
                foreach (var question in defaultQuestions)
                {
                    defaultQuestionsList.Add(question.ID);
                }
                data.DefaultQuestionIDs = defaultQuestionsList;
                setBulkUploadData(data);
            }
            var questionList = frameworkService.GetAssessmentQuestions(data.FrameworkId, adminId).ToList();
            var questionSelectList = new SelectList(questionList, "ID", "Label");
            var model = new AddAssessmentQuestionsViewModel
                (
                data.FrameworkId,
                data.FrameworkName,
                data.FrameworkVocubulary,
                data.PublishStatusID,
                data.CompetenciesToAddCount,
                data.CompetenciesToUpdateCount,
                data.CompetenciesToReorderCount,
                defaultQuestions,
                questionSelectList
                );
            model.AddDefaultAssessmentQuestions = data.AddDefaultAssessmentQuestions;
            model.AddCustomAssessmentQuestion = data.AddCustomAssessmentQuestion;
            model.DefaultAssessmentQuestionIDs = data.DefaultQuestionIDs;
            model.CustomAssessmentQuestionID = data.CustomAssessmentQuestionID;
            return View("Developer/Import/AddAssessmentQuestions", model);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/{tabname}/Import/AssessmentQuestions")]
        public IActionResult AddAssessmentQuestions(AddAssessmentQuestionsFormData model)
        {
            var data = GetBulkUploadData();
            data.AddDefaultAssessmentQuestions = model.AddDefaultAssessmentQuestions;
            if (model.AddDefaultAssessmentQuestions)
            {
                data.DefaultQuestionIDs = model.DefaultAssessmentQuestionIDs;
            }
            else
            {
                data.DefaultQuestionIDs = [];
            }
            data.AddCustomAssessmentQuestion = model.AddCustomAssessmentQuestion;
            if (model.AddCustomAssessmentQuestion)
            {
                data.CustomAssessmentQuestionID = model.CustomAssessmentQuestionID;
            }
            else
            {
                data.CustomAssessmentQuestionID = null;
            }
            if (data.CompetenciesToUpdateCount > 0)
            {
                data.AddAssessmentQuestionsOption = 2;
                setBulkUploadData(data);
                return RedirectToAction("AddQuestionsToWhichCompetencies", "Frameworks", new { frameworkId = data.FrameworkId, tabname = data.TabName });
            }
            else
            {
                data.AddAssessmentQuestionsOption = 1;
                setBulkUploadData(data);
                return RedirectToAction("ImportSummary", "Frameworks", new { frameworkId = data.FrameworkId, tabname = data.TabName });
            }
        }
        [Route("/Framework/{frameworkId}/{tabname}/Import/AssessmentQuestions/Competencies")]
        public IActionResult AddQuestionsToWhichCompetencies()
        {
            var data = GetBulkUploadData();
            if (data.DefaultQuestionIDs.Count ==0 && data.CustomAssessmentQuestionID == null)
            {
                return RedirectToAction("ImportSummary", "Frameworks", new { frameworkId = data.FrameworkId, tabname = data.TabName });
            }
            var model = new AddQuestionsToWhichCompetenciesViewModel
                (
                data.FrameworkId,
                data.FrameworkName,
                data.FrameworkVocubulary,
                data.DefaultQuestionIDs,
                data.CustomAssessmentQuestionID,
                data.AddAssessmentQuestionsOption,
                data.CompetenciesToProcessCount,
                data.CompetenciesToAddCount,
                data.CompetenciesToUpdateCount
                );
            return View("Developer/Import/AddQuestionsToWhichCompetencies", model);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/{tabname}/Import/AssessmentQuestions/Competencies")]
        public IActionResult AddQuestionsToWhichCompetencies(int AddAssessmentQuestionsOption)
        {
            var data = GetBulkUploadData();
            data.AddAssessmentQuestionsOption = AddAssessmentQuestionsOption;
            setBulkUploadData(data);
            return RedirectToAction("ImportSummary", "Frameworks", new { frameworkId = data.FrameworkId, tabname = data.TabName });
        }
        [Route("/Framework/{frameworkId}/{tabname}/Import/Summary")]
        public IActionResult ImportSummary()
        {
            var data = GetBulkUploadData();
            var model = new ImportSummaryViewModel(data);
            return View("Developer/Import/ImportSummary", model);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/{tabname}/Import/Summary")]
        public IActionResult ImportSummarySubmit()
        {
            var data = GetBulkUploadData();
            var adminId = GetAdminId();
            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
            var filePath = Path.Combine(uploadDir, data.CompetenciesFileName);
            var workbook = new XLWorkbook(filePath);
            var results = importCompetenciesFromFileService.ProcessCompetenciesFromFile(workbook, adminId, data.FrameworkId, data.FrameworkVocubulary, data.ReorderCompetenciesOption, data.AddAssessmentQuestionsOption, data.AddCustomAssessmentQuestion ? (int)data.CustomAssessmentQuestionID : 0, data.AddDefaultAssessmentQuestions ? data.DefaultQuestionIDs : []);
            data.ImportCompetenciesResult = results;
            //TO DO apply ordering changes if required:
            if (data.ReorderCompetenciesOption == 2 && data.CompetenciesToReorderCount > 0)
            {

            }
            setBulkUploadData(data);
            return RedirectToAction("UploadResults", "Frameworks", new { frameworkId = data.FrameworkId, tabname = data.TabName });
        }
        [Route("/Framework/{frameworkId}/{tabname}/Import/Results")]
        public IActionResult UploadResults()
        {
            var data = GetBulkUploadData();
            FileHelper.DeleteFile(webHostEnvironment, data.CompetenciesFileName);
            TempData.Clear();
            var model = new ImportCompetenciesResultsViewModel(data.ImportCompetenciesResult, data.FrameworkId, data.FrameworkName, data.FrameworkVocubulary);
            return View("Developer/Import/UploadResults", model);
        }
        [Route("CancelImport")]
        public IActionResult CancelImport()
        {
            var data = GetBulkUploadData();
            var frameworkId = data.FrameworkId;
            FileHelper.DeleteFile(webHostEnvironment, data.CompetenciesFileName);
            TempData.Clear();
            return RedirectToAction("ViewFramework", new { frameworkId, tabname = "Structure" });
        }
        private void setupBulkUploadData(int frameworkId, int adminUserID, string competenciessFileName, string tabName, bool isNotBlank)
        {
            TempData.Clear();
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("BulkCompetencyDataCWF"), TempData);
            var framework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminUserID);
            var today = clockUtility.UtcToday;
            var bulkUploadData = new BulkCompetenciesData(framework, adminUserID, competenciessFileName, tabName, isNotBlank);
            setBulkUploadData(bulkUploadData);
        }
        private void setBulkUploadData(BulkCompetenciesData bulkUploadData)
        {
            multiPageFormService.SetMultiPageFormData(
                 bulkUploadData,
                 MultiPageFormDataFeature.AddCustomWebForm("BulkCompetencyDataCWF"),
                 TempData
             );
        }

        private BulkCompetenciesData GetBulkUploadData()
        {
            var data = multiPageFormService.GetMultiPageFormData<BulkCompetenciesData>(
               MultiPageFormDataFeature.AddCustomWebForm("BulkCompetencyDataCWF"),
               TempData
           ).GetAwaiter().GetResult();
            return data;
        }
    }
}
