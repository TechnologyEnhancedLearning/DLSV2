
namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using DigitalLearningSolutions.Web.ViewModels.Support.RequestSupportTicket;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using System.Collections.Generic;
    using System;
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using Microsoft.AspNetCore.Authorization;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;

    [Route("/{dlsSubApplication}/RequestSupport")]
    [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.Support))]
    [TypeFilter(typeof(ValidateAllowedDlsSubApplication), Arguments = new object[] { new[] { nameof(DlsSubApplication.TrackingSystem), nameof(DlsSubApplication.Frameworks) } })]
    public class RequestSupportTicketController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IUserDataService userDataService;
        private readonly ICentresDataService centresDataService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IRequestSupportTicketDataService requestSupportTicketDataService;
        private readonly IFreshdeskService freshdeskService;
        private readonly IMultiPageFormService multiPageFormService;
        string uploadDir = string.Empty;
        public RequestSupportTicketController(IConfiguration configuration
                                        , IUserDataService userDataService
                                        , ICentresDataService centresDataService
                                        , IWebHostEnvironment webHostEnvironment
                                        , IRequestSupportTicketDataService requestSupportTicketDataService
                                        , IFreshdeskService freshdeskService
                                        , IMultiPageFormService multiPageFormService)
        {
            this.configuration = configuration;
            this.userDataService = userDataService;
            this.centresDataService = centresDataService;
            this.webHostEnvironment = webHostEnvironment;
            this.requestSupportTicketDataService = requestSupportTicketDataService;
            this.freshdeskService = freshdeskService;
            this.multiPageFormService = multiPageFormService;
            uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
        }

        public IActionResult Index(DlsSubApplication dlsSubApplication)
        {
            var model = new RequestSupportTicketViewModel(
                dlsSubApplication,
                SupportPage.RequestSupportTicket,
                configuration.GetCurrentSystemBaseUrl()
            );
            var centreId = User.GetCentreIdKnownNotNull();
            var userName = userDataService.GetUserDisplayName(User.GetUserId() ?? 0);
            var userCentreEmail = requestSupportTicketDataService.GetUserCentreEmail(User.GetUserId() ?? 0, centreId);
            var adminUserID = User.GetAdminId();
            var centreName = centresDataService.GetCentreName(centreId);
            setupRequestSupportData(userName, userCentreEmail, adminUserID ?? 0, centreName);
            return View("Request", model);
        }

        [Route("/{dlsSubApplication}/RequestSupport/TypeofRequest")]
        public IActionResult TypeofRequest(DlsSubApplication dlsSubApplication)
        {
            var requestTypes = requestSupportTicketDataService.GetRequestTypes();
            var data = multiPageFormService.GetMultiPageFormData<RequestSupportTicketData>(
                MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"),
                TempData
            ).GetAwaiter().GetResult();
            var model = new RequestTypeViewModel(requestTypes.ToList(), data);
            return View("TypeOfRequest", model);
        }

        [HttpPost]
        [Route("/{dlsSubApplication}/RequestSupport/setRequestType")]
        public IActionResult setRequestType(DlsSubApplication dlsSubApplication, RequestTypeViewModel RequestTypemodel, int requestType)
        {
            var requestTypes = requestSupportTicketDataService.GetRequestTypes();
            var reqType = requestTypes.ToList().Where(x => x.ID == requestType)
                .Select(ticketRequestTypes => new { ticketRequestTypes.RequestTypes, ticketRequestTypes.FreshdeskRequestTypes }).FirstOrDefault();

            var data = multiPageFormService.GetMultiPageFormData<RequestSupportTicketData>(
                MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"),
                TempData
            ).GetAwaiter().GetResult(); ;
            data.RequestTypeId = requestType;
            data.RequestType = reqType?.RequestTypes;
            data.FreshdeskRequestType = reqType?.FreshdeskRequestTypes;
            setRequestSupportTicketData(data);
            var model1 = new RequestTypeViewModel(requestTypes.ToList(), data);
            if (requestType < 1)
            {
                ModelState.AddModelError("Id", "Please choose a request type");
                return View("TypeOfRequest", model1);
            }
            return RedirectToAction("RequestSummary", new { dlsSubApplication });
        }

        [Route("/{dlsSubApplication}/RequestSupport/RequestSummary")]
        public IActionResult RequestSummary(DlsSubApplication dlsSubApplication, RequestSummaryViewModel RequestTypemodel)
        {
            var data = multiPageFormService.GetMultiPageFormData<RequestSupportTicketData>(
                MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"),
                TempData
            ).GetAwaiter().GetResult(); ;
            var model = new RequestSummaryViewModel(data);
            data.setRequestSubjectDetails(model);
            return View("RequestSummary", model);
        }

        [HttpPost]
        [Route("/{dlsSubApplication}/RequestSupport/SetRequestSummary")]
        public IActionResult SetRequestSummary(DlsSubApplication dlsSubApplication, RequestSummaryViewModel requestDetailsmodel)
        {
            if (requestDetailsmodel.RequestSubject == null)
            {
                ModelState.AddModelError("RequestSubject", "Please enter request summary");
                return View("RequestSummary", requestDetailsmodel);
            }
            if (requestDetailsmodel.RequestDescription == null)
            {
                ModelState.AddModelError("RequestDescription", "Please enter request description");
                return View("RequestSummary", requestDetailsmodel);
            }
            if (!ModelState.IsValid)
            {
                return View("RequestSummary", requestDetailsmodel);
            }
            var data = multiPageFormService.GetMultiPageFormData<RequestSupportTicketData>(
                MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"),
                TempData
            ).GetAwaiter().GetResult(); ;
            data.setRequestSubjectDetails(requestDetailsmodel);
            setRequestSupportTicketData(data);
            return RedirectToAction("RequestAttachment", new { dlsSubApplication });
        }

        [Route("/{dlsSubApplication}/RequestSupport/RequestAttachment")]
        public IActionResult RequestAttachment(DlsSubApplication dlsSubApplication, RequestAttachmentViewModel model)
        {
            var data = multiPageFormService.GetMultiPageFormData<RequestSupportTicketData>(
                MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"),
                TempData
            ).GetAwaiter().GetResult(); ;
            setRequestSupportTicketData(data);
            model = new RequestAttachmentViewModel(data);
            return View("RequestAttachment", model);
        }

        [HttpPost]
        [Route("/{dlsSubApplication}/RequestSupport/SetAttachment")]
        public IActionResult SetAttachment(DlsSubApplication dlsSubApplication, RequestAttachmentViewModel requestAttachmentmodel)
        {
            var data = multiPageFormService.GetMultiPageFormData<RequestSupportTicketData>(
                MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"),
                TempData
            ).GetAwaiter().GetResult(); ;
            requestAttachmentmodel.RequestAttachment = data.RequestAttachment;
            if (requestAttachmentmodel.ImageFiles == null)
            {
                //requestAttachmentmodel.RequestAttachment = data.RequestAttachment;
                ModelState.AddModelError("ImageFiles", "Please select at least one image");
                return View("RequestAttachment", requestAttachmentmodel);
            }
            if (!ModelState.IsValid)
            {
                return View("RequestAttachment", requestAttachmentmodel);
            }
            (bool? fileExtension, bool? fileSize) = validateUploadedImages(requestAttachmentmodel);
            if (fileExtension == true)
            {
                //requestAttachmentmodel.RequestAttachment = data.RequestAttachment;
                ModelState.AddModelError("FileExtensionError", "File must be in valid image formats jpg, jpeg, png, bmp or mp4 video format");
                return View("RequestAttachment", requestAttachmentmodel);
            }
            if (fileSize == true)
            {
                //requestAttachmentmodel.RequestAttachment = data.RequestAttachment;
                ModelState.AddModelError("FileSizeError", "Maximum allowed file size is 20MB");
                return View("RequestAttachment", requestAttachmentmodel);
            }
            List<RequestAttachment> RequestAttachmentList = new List<RequestAttachment>();
            foreach (var item in requestAttachmentmodel.ImageFiles)
            {
                string fileName = UploadFile(item);
                var RequestAttachment = new RequestAttachment
                {
                    OriginalFileName = item.FileName,
                    FileName = fileName,
                    FullFileName = uploadDir + fileName,
                    SizeMb = Convert.ToDouble(item.Length.ToSize(FileSizeCalc.SizeUnits.MB))
                };
                RequestAttachmentList.Add(RequestAttachment);
            }

            data.setImageFiles(RequestAttachmentList);
            setRequestSupportTicketData(data);
            return RedirectToAction("RequestAttachment", new { dlsSubApplication });
        }

        [Route("/{dlsSubApplication}/RequestSupport/SetAttachment/DeleteImage")]
        public IActionResult DeleteImage(DlsSubApplication dlsSubApplication, string imageName)
        {
            var data = multiPageFormService.GetMultiPageFormData<RequestSupportTicketData>(
                MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"),
                TempData
            ).GetAwaiter().GetResult(); ;
            if (data.RequestAttachment != null)
            {
                var attachmentToRemove = data.RequestAttachment.FirstOrDefault(a => a.FileName == imageName);
                if (attachmentToRemove != null)
                {
                    data.RequestAttachment.Remove(attachmentToRemove);
                    var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads", attachmentToRemove.FullFileName);
                    if (System.IO.File.Exists(uploadDir))
                    {
                        System.IO.File.Delete(uploadDir);
                    }
                }
            }
            setRequestSupportTicketData(data);
            return RedirectToAction("RequestAttachment", new { dlsSubApplication });
        }

        [HttpGet]
        [Route("/{dlsSubApplication}/RequestSupport/SupportSummary")]
        public IActionResult SupportSummary(DlsSubApplication dlsSubApplication, SupportSummaryViewModel supportSummaryViewModel)
        {
            var data = multiPageFormService.GetMultiPageFormData<RequestSupportTicketData>(
                MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"),
                TempData
            ).GetAwaiter().GetResult(); ;
            var model = new SupportSummaryViewModel(data);
            return View("SupportTicketSummaryPage", model);
        }

        [HttpPost]
        [Route("/{dlsSubApplication}/RequestSupport/SubmitSupportSummary")]
        public IActionResult SubmitSupportSummary(DlsSubApplication dlsSubApplication, SupportSummaryViewModel model)

        {
            var data = multiPageFormService.GetMultiPageFormData<RequestSupportTicketData>(
                MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"),
                TempData
            ).GetAwaiter().GetResult(); ;
            data.GroupId = configuration.GetFreshdeskCreateTicketGroupId();
            data.ProductId = configuration.GetFreshdeskCreateTicketProductId();
            List<RequestAttachment> RequestAttachmentList = new List<RequestAttachment>();
            string fileName = null;
            if (data.RequestAttachment != null)
            {
                foreach (var file in data.RequestAttachment)
                {
                    fileName = uploadDir + file.FileName;
                    byte[] FileBytes = System.IO.File.ReadAllBytes(fileName);
                    var attachment = new RequestAttachment()
                    {
                        Id = Guid.NewGuid().ToString(),
                        OriginalFileName = file.OriginalFileName,
                        FileName = file.FileName,
                        FullFileName = fileName,
                        Content = FileBytes
                    };
                    RequestAttachmentList.Add(attachment);
                }

                data.RequestAttachment.RemoveAll((x) => x.Content == null);
                data.setImageFiles(RequestAttachmentList);
            }
            data.RequestType = "DLS " + data.RequestType;
            data.RequestSubject = data.RequestSubject + $" (DLS centre: {data.CentreName})";
            var result = freshdeskService.CreateNewTicket(data);
            if (result.StatusCode == 200)
            {
                long? ticketId = result.TicketId;
                if (data.RequestAttachment != null)
                {
                    DeleteFilesAfterSubmitSupportTicket(data.RequestAttachment);
                }
                multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"), TempData);
                TempData.Clear();
                var responseModel = new FreshDeskResponseViewModel(ticketId, null);
                return View("SuccessPage", responseModel);
            }
            else
            {
                int? errorCode = result.StatusCode;
                string errorMess = result.StatusMeaning;
                if (string.IsNullOrEmpty(errorMess))
                { errorMess = result.FullErrorDetails; }
                var responseModel = new FreshDeskResponseViewModel(null, errorCode + ": " + errorMess);
                if (data.RequestAttachment != null)
                {
                    DeleteFilesAfterSubmitSupportTicket(data.RequestAttachment);
                }
                multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"), TempData);
                TempData.Clear();
                return View("RequestError", responseModel);
            }
        }

        private void DeleteFilesAfterSubmitSupportTicket(List<RequestAttachment> RequestAttachment)
        {
            if (RequestAttachment != null)
            {
                foreach (var attachment in RequestAttachment)
                {
                    var uploadDir = System.IO.Path.Combine(
                        webHostEnvironment.WebRootPath,
                        "Uploads",
                        attachment.FullFileName
                    );
                    if (System.IO.File.Exists(uploadDir))
                    {
                        // If file found, delete it
                        System.IO.File.Delete(uploadDir);
                    }
                }
            }
        }

        private void setupRequestSupportData(string userName, string userCentreEmail, int adminUserID, string centreName)
        {
            TempData.Clear();
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"), TempData);
            var requestSupportData = new RequestSupportTicketData(userName, userCentreEmail, adminUserID, centreName);
            setRequestSupportTicketData(requestSupportData);
        }

        private void setRequestSupportTicketData(RequestSupportTicketData requestSupportTicketData)
        {
            multiPageFormService.SetMultiPageFormData(
                requestSupportTicketData,
                MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"),
                TempData
            );
        }

        private string UploadFile(IFormFile file)
        {
            string uploadDir = string.Empty;
            string fileName = null;
            if (file != null)
            {
                uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return fileName;
        }

        private (bool, bool) validateUploadedImages(RequestAttachmentViewModel requestAttachmentmodel)
        {
            var totalFileSize = 0.00;
            if (requestAttachmentmodel.RequestAttachment != null)
            {
                foreach (var item in requestAttachmentmodel.RequestAttachment)
                {
                    totalFileSize = totalFileSize + item.SizeMb??0;
                }
            }
            foreach (var item in requestAttachmentmodel.ImageFiles)
            {
                var extension = Path.GetExtension(item.FileName);
                if (!requestAttachmentmodel.AllowedExtensions.Contains(extension))
                {
                    requestAttachmentmodel.FileExtensionFlag = true;
                    return (requestAttachmentmodel.FileExtensionFlag ?? false, requestAttachmentmodel.FileSizeFlag ?? false);
                }
                var fileSize = Convert.ToDouble(item.Length.ToSize(FileSizeCalc.SizeUnits.MB));
                totalFileSize = totalFileSize + fileSize;

            }
            if (totalFileSize > requestAttachmentmodel.SizeLimit)
            {
                requestAttachmentmodel.FileSizeFlag = true;
            }
            return (requestAttachmentmodel.FileExtensionFlag ?? false, requestAttachmentmodel.FileSizeFlag ?? false);
        }
    }
}
