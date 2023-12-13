
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
    using DigitalLearningSolutions.Web.Extensions;
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
        string uploadDir = string.Empty;
        public RequestSupportTicketController(IConfiguration configuration,
                                        IUserDataService userDataService, ICentresDataService centresDataService
                                       , IWebHostEnvironment webHostEnvironment
                                       , IRequestSupportTicketDataService requestSupportTicketDataService
                                        , IFreshdeskService freshdeskService)
        {
            this.configuration = configuration;
            this.userDataService = userDataService;
            this.centresDataService = centresDataService;
            this.webHostEnvironment = webHostEnvironment;
            this.requestSupportTicketDataService = requestSupportTicketDataService;
            this.freshdeskService = freshdeskService;
            uploadDir = System.IO.Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
        }

        public IActionResult Index(DlsSubApplication dlsSubApplication)
        {
            TempData.Clear();
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
            setRequestSupportData(userName, userCentreEmail, adminUserID ?? 0, centreName);
            return View("Request", model);
        }

        [Route("/{dlsSubApplication}/RequestSupport/TypeofRequest")]
        public IActionResult TypeofRequest(DlsSubApplication dlsSubApplication)
        {
            var requestTypes = requestSupportTicketDataService.GetRequestTypes();
            var data = TempData.Peek<RequestSupportTicketData>()!;
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

            var data = TempData.Peek<RequestSupportTicketData>()!;
            data.RequestTypeId = requestType;
            data.RequestType = reqType?.RequestTypes;
            data.FreshdeskRequestType = reqType?.FreshdeskRequestTypes;
            TempData.Set(data);
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
            var data = TempData.Peek<RequestSupportTicketData>()!;
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
            var data = TempData.Peek<RequestSupportTicketData>()!;
            data.setRequestSubjectDetails(requestDetailsmodel);
            TempData.Set(data);
            return RedirectToAction("RequestAttachment", new { dlsSubApplication });
        }

        [Route("/{dlsSubApplication}/RequestSupport/RequestAttachment")]
        public IActionResult RequestAttachment(DlsSubApplication dlsSubApplication, RequestAttachmentViewModel model)
        {
            var data = TempData.Peek<RequestSupportTicketData>()!;
            TempData.Set(data);
            model = new RequestAttachmentViewModel(data);
            return View("RequestAttachment", model);
        }

        [HttpPost]
        [Route("/{dlsSubApplication}/RequestSupport/SetAttachment")]
        public IActionResult SetAttachment(DlsSubApplication dlsSubApplication, RequestAttachmentViewModel requestAttachmentmodel)
        {
            var data = TempData.Peek<RequestSupportTicketData>()!;

            if (requestAttachmentmodel.ImageFiles == null)
            {
                requestAttachmentmodel.RequestAttachment = data.RequestAttachment;
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
                requestAttachmentmodel.RequestAttachment = data.RequestAttachment;
                ModelState.AddModelError("FileExtensionError", "File must be in valid image formats jpg, jpeg, png, bmp or mp4 video format");
                return View("RequestAttachment", requestAttachmentmodel);
            }
            if (fileSize == true)
            {
                requestAttachmentmodel.RequestAttachment = data.RequestAttachment;
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
                    FullFileName = uploadDir + fileName
                };
                RequestAttachmentList.Add(RequestAttachment);
            }

            data.setImageFiles(RequestAttachmentList);
            TempData.Set(data);
            return RedirectToAction("RequestAttachment", new { dlsSubApplication });
        }

        [Route("/{dlsSubApplication}/RequestSupport/SetAttachment/DeleteImage")]
        public IActionResult DeleteImage(DlsSubApplication dlsSubApplication, string imageName)
        {
            var data = TempData.Peek<RequestSupportTicketData>()!;
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
            TempData.Set(data);
            return RedirectToAction("RequestAttachment", new { dlsSubApplication });
        }

        [HttpGet]
        [Route("/{dlsSubApplication}/RequestSupport/SupportSummary")]
        public IActionResult SupportSummary(DlsSubApplication dlsSubApplication, SupportSummaryViewModel supportSummaryViewModel)
        {
            var data = TempData.Peek<RequestSupportTicketData>()!;
            var model = new SupportSummaryViewModel(data);
            return View("SupportTicketSummaryPage", model);
        }

        [HttpPost]
        [Route("/{dlsSubApplication}/RequestSupport/SubmitSupportSummary")]
        public IActionResult SubmitSupportSummary(DlsSubApplication dlsSubApplication, SupportSummaryViewModel model)

        {
            var data = TempData.Peek<RequestSupportTicketData>()!;
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

        private void setRequestSupportData(string userName, string userCentreEmail, int adminUserID, string centreName)
        {
            var requestSupportData = new RequestSupportTicketData(userName, userCentreEmail, adminUserID, centreName);
            TempData.Set(requestSupportData);
        }

        private string UploadFile(IFormFile file)
        {
            string uploadDir = string.Empty;
            string fileName = null;
            if (file != null)
            {
                uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = System.IO.Path.Combine(uploadDir, fileName);
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
            foreach (var item in requestAttachmentmodel.ImageFiles)
            {
                var extension = System.IO.Path.GetExtension(item.FileName);
                if (!requestAttachmentmodel.AllowedExtensions.Contains(extension))
                {
                    requestAttachmentmodel.FileExtensionFlag = true;
                    return (requestAttachmentmodel.FileExtensionFlag ?? false, requestAttachmentmodel.FileSizeFlag ?? false);
                }
                var fileSize = Convert.ToDouble(item.Length.ToSize(FileSizeCalc.SizeUnits.MB));
                totalFileSize = totalFileSize + fileSize;
                if (fileSize > requestAttachmentmodel.SizeLimit || totalFileSize > requestAttachmentmodel.SizeLimit)
                {
                    requestAttachmentmodel.FileSizeFlag = true;
                }
            }
            return (requestAttachmentmodel.FileExtensionFlag ?? false, requestAttachmentmodel.FileSizeFlag ?? false);
        }
    }
}
