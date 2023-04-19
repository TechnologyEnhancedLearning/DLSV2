namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.UserFeedback;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.UserFeedback;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using Microsoft.AspNetCore.Mvc;

    public class UserFeedbackController : Controller
    {
        private readonly IUserFeedbackDataService _userFeedbackDataService;
        private readonly IMultiPageFormService multiPageFormService;

        public UserFeedbackController(
            IUserFeedbackDataService userFeedbackDataService,
            IMultiPageFormService multiPageFormService
        )
        {
            this._userFeedbackDataService = userFeedbackDataService;
            this.multiPageFormService = multiPageFormService;
        }

        [Route("/Index")]
        public Task<IActionResult> Index(string sourceUrl)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            UserFeedbackViewModel userFeedbackViewModel = new()
            {
                UserId = User.GetUserId(),
                SourceUrl = sourceUrl,
            };

            if (userFeedbackViewModel.UserId == null || userFeedbackViewModel.UserId == 0)
            {
                return Task.FromResult<IActionResult>(View("GuestFeedbackStart", userFeedbackViewModel));
            }
            else
            {
                return Task.FromResult(StartUserFeedbackSession(userFeedbackViewModel));
            }
        }

        //https://github.com/TechnologyEnhancedLearning/GDSMultiPageFormService

        //--------------------------------------------------
        // Step Zero

        [HttpGet]
        public IActionResult StartUserFeedbackSession(UserFeedbackViewModel userFeedbackViewModel)
        {

            //Starting a transactional flow
            //A controller method should be created to start the multi-page transactional flow. This should:

            //Clear TempData
            //Invoke the multi - page form service, passing it the model for the data being captured
            //Redirect to an action that will return the view for the first page of the transaction
            //For example:
            //[HttpGet("AddCourseNew")]
            //public IActionResult AddCourseNew()
            //{
            //    //1. Clear Tempdata:
            //    TempData.Clear();
            //    //2. Invoke the multi-page form service, passing it the model for the data being captured:
            //    multiPageFormService.SetMultiPageFormData(
            //        new AddNewCentreCourseTempData(),
            //        MultiPageFormDataFeature.AddNewCourse,
            //        TempData
            //    );
            //    //3. Redirect to an action that will return the view for the first page of the transaction:
            //    return RedirectToAction("SelectCourse");
            //}

            var userFeedbackSessionData = new UserFeedbackSessionData()
            {
                UserID = userFeedbackViewModel.UserId,
                SourceUrl = userFeedbackViewModel.SourceUrl,
                TaskAchieved = null,
                TaskAttempted = null,
                FeedbackText = null,
                TaskDifficulty = null,
            };

            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                userFeedbackSessionData,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );
            return RedirectToAction("UserFeedbackTaskAchieved");
        }

        //--------------------------------------------------
        // Step One
        
        [HttpGet]
        public Task<IActionResult> UserFeedbackTaskAchieved()
        {
            return Task.FromResult<IActionResult>(View("UserFeedbackTaskAchieved"));
        }

        [HttpPost]
        public IActionResult UserFeedbackTaskAchievedSet(UserFeedbackViewModel userFeedbackViewModel)
        {

//            Update the data on submit for each step of the flow
//            On submit their selection for each step of the form, the POST method should:


//            Use the multipage form service to retrieve transactional data
//            Update the data with the selections submitted by the user
//            Store the updated data using the multipage form service
//Redirect to the GET action method for the next step in the transaction
//For example:
//[HttpPost("AddCourse/SelectCourse")]
//                public IActionResult SelectCourse(
//            int? applicationId,
//            string? categoryFilterString = null,
//            string? topicFilterString = null
//        )
//                {
//                    //1. Use the multipage form service to retrieve transactional data:
//                    var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
//                        MultiPageFormDataFeature.AddNewCourse,
//                        TempData
//                    );

//                    if (applicationId == null)
//                    {
//                        ModelState.AddModelError("ApplicationId", "Select a course");
//                        return View(
//                            "AddNewCentreCourse/SelectCourse",
//                            GetSelectCourseViewModel(
//                                categoryFilterString,
//                                topicFilterString
//                            )
//                        );
//                    }

//                    var centreId = User.GetCentreId();
//                    var categoryId = User.GetAdminCourseCategoryFilter();

//                    var selectedApplication =
//                        courseService.GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryId)
//                            .Single(ap => ap.ApplicationId == applicationId);
//                    //2. Update the data with the selections submitted by the user:
//                    data.CategoryFilter = categoryFilterString;
//                    data.TopicFilter = topicFilterString;
//                    data!.SetApplicationAndResetModels(selectedApplication);
//                    //3. Store the updated data using the multipage form service:
//                    multiPageFormService.SetMultiPageFormData(data, MultiPageFormDataFeature.AddNewCourse, TempData);
//                    //4: Redirect to the GET action method for the next step in the transaction:
//                    return RedirectToAction("SetCourseDetails");
//                }

            var sessionData = multiPageFormService.GetMultiPageFormData<UserFeedbackSessionData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            sessionData.TaskAchieved = userFeedbackViewModel.TaskAchieved;

            multiPageFormService.SetMultiPageFormData(
                sessionData,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );

            return RedirectToAction("UserFeedbackTaskAttempted");
        }


        //--------------------------------------------------
        // Step Two

        [HttpGet]
        public Task<IActionResult> UserFeedbackTaskAttempted()
        {
            // TODO: Might need read multipage stuff in here maybe? Or just return view earlier rather than this whole method.
            return Task.FromResult<IActionResult>(View("UserFeedbackTaskAttempted"));
        }

        [HttpPost]
        public IActionResult UserFeedbackTaskAttemptedSet(UserFeedbackViewModel userFeedbackViewModel)
        {
            multiPageFormService.SetMultiPageFormData(
                userFeedbackViewModel,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );

            return RedirectToAction("UserFeedbackTaskDifficulty");
        }

        //--------------------------------------------------
        // Step Three

        [HttpGet]
        public IActionResult UserFeedbackTaskDifficulty()
        {
            // TODO: Might need read multipage stuff in here maybe? Or just return view earlier rather than this whole method.
            return View("UserFeedbackTaskDifficulty");
        }

        [HttpPost]
        public IActionResult UserFeedbackTaskDifficultySet(UserFeedbackViewModel userFeedbackViewModel)
        {
            var session = multiPageFormService.GetMultiPageFormData<UserFeedbackSessionData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            multiPageFormService.SetMultiPageFormData(
                userFeedbackViewModel,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );

            return View("UserFeedbackTaskDifficulty");
        }

        //--------------------------------------------------
        // Step Four


        [HttpGet]
        [Route("/UserFeedbackConfirm")]
        public IActionResult UserFeedbackConfirm()
        {
//            Handle submitting the data
//            The POST method for the summary page, triggered by submitting, should:

//            Use the multipage form service to retrieve transactional data
//                Commit the data to the database(using an update or insert service method or API call)
//            Use the multipage form service to remove the transactional data
//            Clear TempData
//            Redirect to a confirmation screen
//                For example:
//            [HttpPost("AddCourse/Summary")]
//            public IActionResult? CreateNewCentreCourse()
//            {
//                //1. Use the multipage form service to retrieve transactional data
//                var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
//                    MultiPageFormDataFeature.AddNewCourse,
//                    TempData
//                );

//                using var transaction = new TransactionScope();

//                var customisation = GetCustomisationFromTempData(data!);
//                //2. Commit the data to the database (using an update or insert service method or API call):
//                var customisationId = courseService.CreateNewCentreCourse(customisation);

//                ...

////3. Use the multipage form service to remove the transactional data:
//                multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddNewCourse, TempData);

//                transaction.Complete();
//                //4. Clear TempData
//                TempData.Clear();
//                TempData.Add("customisationId", customisationId);
//                TempData.Add("applicationName", data.Application!.ApplicationName);
//                TempData.Add("customisationName", data.CourseDetailsData!.CustomisationName);
//                //5. Redirect to a confirmation screen
//                return RedirectToAction("Confirmation");
//            }

            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            var userFeedbackModel = new UserFeedbackViewModel();

            return View("UserFeedbackComplete");
        }

        [HttpPost]
        public async Task<IActionResult> UserFeedbackComplete(
            //string userFeedbackText,
            //bool? taskAchieved,
            //string? taskAttempted,
            //int? taskRating,
            //string sourceUrl
        )
        {
            // TODO: Read this out of the multipage form service and save to db.

//            Handle submitting the data
//            The POST method for the summary page, triggered by submitting, should:

//            Use the multipage form service to retrieve transactional data
//                Commit the data to the database(using an update or insert service method or API call)
//            Use the multipage form service to remove the transactional data
//            Clear TempData
//            Redirect to a confirmation screen
//                For example:
//            [HttpPost("AddCourse/Summary")]
//            public IActionResult? CreateNewCentreCourse()
//            {
//                //1. Use the multipage form service to retrieve transactional data
//                var data = multiPageFormService.GetMultiPageFormData<AddNewCentreCourseTempData>(
//                    MultiPageFormDataFeature.AddNewCourse,
//                    TempData
//                );

//                using var transaction = new TransactionScope();

//                var customisation = GetCustomisationFromTempData(data!);
//                //2. Commit the data to the database (using an update or insert service method or API call):
//                var customisationId = courseService.CreateNewCentreCourse(customisation);

//                ...

////3. Use the multipage form service to remove the transactional data:
//                multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddNewCourse, TempData);

//                transaction.Complete();
//                //4. Clear TempData
//                TempData.Clear();
//                TempData.Add("customisationId", customisationId);
//                TempData.Add("applicationName", data.Application!.ApplicationName);
//                TempData.Add("customisationName", data.CourseDetailsData!.CustomisationName);
//                //5. Redirect to a confirmation screen
//                return RedirectToAction("Confirmation");
//            }

            var userId = User.GetUserId();

            //_userFeedbackDataService.SaveUserFeedback(
            //    userId,
            //    sourceUrl,
            //    taskAchieved,
            //    taskAttempted,
            //    userFeedbackText,
            //    taskRating
            //);

            //TODO: Probs need error handling here with associated user error message.
            return RedirectToAction("UserFeedbackComplete");
        }


        [HttpGet]
        [Route("/GuestFeedbackComplete")]
        public Task<IActionResult> GuestFeedbackComplete()
        {
            // TODO: Populate the 'return to what you were doing' url link/button

            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            var userFeedbackModel = new UserFeedbackViewModel();

            return Task.FromResult<IActionResult>(View("GuestFeedbackComplete", userFeedbackModel));
        }


        //[HttpPost]
        //public IActionResult UserFeedbackTaskAttemptedSave(UserFeedbackViewModel userFeedbackViewModel)
        //{
        //    //    TempData.Clear();
        //    //    multiPageFormService.SetMultiPageFormData(
        //    //        model,
        //    //        MultiPageFormDataFeature.AddUserFeedback,
        //    //        TempData
        //    //    );
        //    //return RedirectToAction("", model);
        //    return View("UserFeedbackTaskDifficulty", userFeedbackViewModel);
        //}


    }
}
