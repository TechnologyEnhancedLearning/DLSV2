
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
    using Microsoft.FeatureManagement;
    using Microsoft.Extensions.Configuration;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System;
    using Microsoft.AspNetCore.Http;
    using System.IO;

    public class RequestSupportTicketController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IUserDataService userDataService;
        private readonly ICentresDataService centresDataService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IRequestSupportTicketDataService requestSupportTicketDataService;
        private readonly IFreshdeskService freshdeskService;

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
        }
        [Route("Support/RequestSupportTicket")]
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
            var userCentreEmail = userDataService.GetCentreEmail(User.GetUserId() ?? 0, centreId);
            var adminUserID = User.GetAdminId();
            var centreName = centresDataService.GetCentreName(centreId);
            setRequestSupportData(userName, userCentreEmail, adminUserID ?? 0, centreName);
            return View("Request", model);
        }

        [Route("RequestSupport/TypeofRequest")]
        public IActionResult TypeofRequest(DlsSubApplication dlsSubApplication)
        {
            var requestTypes = requestSupportTicketDataService.GetRequestTypes();
            var data = TempData.Peek<RequestSupportTicketData>()!;
            var model = new RequestTypeViewModel(requestTypes.ToList(), data);
            return View("TypeOfRequest", model);
        }
        [HttpPost]
        [Route("RequestSupport/setRequestType")]
        public IActionResult setRequestType(RequestTypeViewModel RequestTypemodel, int requestType)
        {
            var requestTypes = requestSupportTicketDataService.GetRequestTypes();
            var reqType = requestTypes.ToList().Where(x => x.ID == requestType)
                .Select(ticketRequestTypes => new { ticketRequestTypes.RequestTypes, ticketRequestTypes.FreshdeskRequestTypes }).FirstOrDefault();

           // string reqType = requestTypes.ToList().Where(x => x.ID == requestType).Select(x => x.RequestTypes).FirstOrDefault();
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
            var model = new RequestSummaryViewModel(data);
            return View("RequestSummary", model);
        }
        [Route("RequestSupport/RequestSummary")]
        public IActionResult RequestSummary(RequestSummaryViewModel RequestTypemodel)

        {
            //if(!ModelState.IsValid)
            //{
            //    return View("RequestDetails", RequestTypemodel);
            //}
            var data = TempData.Peek<RequestSupportTicketData>()!;

            var model = new RequestSummaryViewModel(data);
            data.setRequestSubjectDetails(model);
            return View("RequestSummary", model);
        }
        [HttpPost]
        [Route("RequestSupport/SetRequestSummary")]
        public IActionResult SetRequestSummary(RequestSummaryViewModel requestDetailsmodel)

        {
            if(requestDetailsmodel.RequestSubject==null)
            {
                ModelState.AddModelError("RequestSubject", "Please enter request summary");
                return View("RequestSummary", requestDetailsmodel);
            }
            if (!ModelState.IsValid)
            {
                return View("RequestSummary", requestDetailsmodel);
            }
            var data = TempData.Peek<RequestSupportTicketData>()!;
            data.setRequestSubjectDetails(requestDetailsmodel);
            TempData.Set(data);
            var model = new RequestAttachmentViewModel();
            return View("RequestAttachment", model);
        }
        [Route("RequestSupport/RequestAttachment")]
        public IActionResult RequestAttachment(RequestAttachmentViewModel model)

        {
            var data = TempData.Peek<RequestSupportTicketData>()!;
            TempData.Set(data);
            model = new RequestAttachmentViewModel(data);
            return View("RequestAttachment", model);
        }
        [HttpPost]
        [Route("RequestSupport/SetAttachment")]
        public IActionResult SetAttachment(RequestAttachmentViewModel requestAttachmentmodel)

        {
            if (!ModelState.IsValid)
            {
                if (requestAttachmentmodel.ImageFiles == null)
                {
                    return View("RequestAttachment", requestAttachmentmodel);
                }
            }
            List<RequestAttachment> RequestAttachmentList = new List<RequestAttachment>();
            foreach (var item in requestAttachmentmodel.ImageFiles)
            {
                string fileName = UploadFile(item);
                var RequestAttachment = new RequestAttachment
                {
                    FileName = fileName
                };
                RequestAttachmentList.Add(RequestAttachment);
            }
            var data = TempData.Peek<RequestSupportTicketData>()!;
            data.setImageFiles(RequestAttachmentList);
            TempData.Set(data);
            var model = new RequestAttachmentViewModel(data);
           
            return View("RequestAttachment", model);
        }
        public IActionResult DeleteImage(string imageName, string imageId)

        {

            var data = TempData.Peek<RequestSupportTicketData>()!;
            data.RequestAttachment.RemoveAll((x) => x.FileName == imageName && x.Id == imageId);
            TempData.Set(data);
            var model = new RequestAttachmentViewModel(data);
            return View("RequestAttachment", model);
        }
        [HttpGet]
        [Route("RequestSupport/SupportSummary")]
        public IActionResult SupportSummary(SupportSummaryViewModel supportSummaryViewModel)

        {

            var data = TempData.Peek<RequestSupportTicketData>()!;
            data.RequestType = "DLS " + data.RequestType;
            var model = new SupportSummaryViewModel(data);
            return View("SupportTicketSummaryPage", model);

        }
        [HttpPost]
        [Route("RequestSupport/SubmitSupportSummary")]
        public IActionResult SubmitSupportSummary(SupportSummaryViewModel model)

        {
            var data = TempData.Peek<RequestSupportTicketData>()!;
            List<RequestAttachment> RequestAttachmentList = new List<RequestAttachment>();
            string uploadDir = string.Empty;
            string fileName = null;
#if (DEBUG)
            uploadDir = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, "Uploads\\");
#elif (RELEASE)
                uploadDir = System.IO.Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
#endif
            if (data.RequestAttachment != null)
            {
                foreach (var file in data.RequestAttachment)
                {
                    fileName = uploadDir + file.FileName;
                    byte[] FileBytes = System.IO.File.ReadAllBytes(fileName);
                    var attachment = new RequestAttachment()
                    {
                        Id = Guid.NewGuid().ToString(),
                        FileName = fileName.Split('_')[2],
                        FullFileName = fileName,
                        Content = FileBytes
                    };
                    RequestAttachmentList.Add(attachment);
                }

                data.RequestAttachment.RemoveAll((x) => x.Content == null);
                data.setImageFiles(RequestAttachmentList);
            }

            data.UserCentreEmail = "test@gmail.com";

            var result = freshdeskService.CreateNewTicket(data);
            if (result.StatusCode == 200)
            {
                long? ticketId = result.TicketId;
                //DeleteFilesAfterSubmitSupportTicket(data.RequestAttachment);
                //TempData.Clear();
                return View("SuccessPage");
            }
            else
            {
                int? errorCode = result.StatusCode;
                string errorMess = result.FullErrorDetails;
                return View("RequestError");
            }

            //If success
            return View("SuccessPage");
            //
            //else error page
            // 
        }
        private void DeleteFilesAfterSubmitSupportTicket(List<RequestAttachment> RequestAttachment)
        {
            string uploadDir = string.Empty;
            foreach (var attachment in RequestAttachment)
            {
#if (DEBUG)
                uploadDir = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, "Uploads", attachment.FullFileName);
#elif (RELEASE)
                uploadDir = System.IO.Path.Combine(webHostEnvironment.WebRootPath, "Uploads", attachment.FullFileName);
#endif
                if (System.IO.File.Exists(uploadDir))
                {
                    // If file found, delete it
                    System.IO.File.Delete(uploadDir);
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
            //var content = ResizeProfilePicture(file);
            //var stream = new MemoryStream(content);
            //IFormFile file1 = new FormFile(stream, 0, file.Length, "name", "fileName");
            string uploadDir = string.Empty;
            string fileName = null;
            if (file != null)
            {
#if (DEBUG)
                uploadDir = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, "Uploads");
#elif (RELEASE)
                uploadDir = System.IO.Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
#endif
                fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = System.IO.Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return fileName;
        }
    }
}
