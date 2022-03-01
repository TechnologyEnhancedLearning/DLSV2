﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class CourseSetupControllerTests
    {
        private readonly ApplicationDetails application = new ApplicationDetails
        {
            ApplicationId = 1,
            ApplicationName = "Test Name",
            CategoryName = "Test Category Name",
            CourseTopicId = 1,
            CourseTopic = "Topic",
            PLAssess = true,
            DiagAssess = true,
        };

        private readonly List<ApplicationDetails> applicationOptions = new List<ApplicationDetails>
        {
            new ApplicationDetails
            {
                ApplicationId = 1,
                ApplicationName = "Test Name",
                CategoryName = "Test Category Name",
                CourseTopicId = 1,
                CourseTopic = "Topic",
                PLAssess = true,
                DiagAssess = true,
            },
        };

        private readonly CentreCourseDetails details = Builder<CentreCourseDetails>.CreateNew()
            .With(
                x => x.Courses = new List<CourseStatisticsWithAdminFieldResponseCounts>
                {
                    new CourseStatisticsWithAdminFieldResponseCounts
                    {
                        ApplicationName = "Course",
                        CustomisationName = "Customisation",
                        Active = true,
                        CourseTopic = "Topic 1",
                        CategoryName = "Category 1",
                        HideInLearnerPortal = true,
                        DelegateCount = 1,
                        CompletedCount = 1,
                    },
                }
            )
            .And(x => x.Categories = new List<string> { "Category 1", "Category 2" })
            .And(x => x.Topics = new List<string> { "Topic 1", "Topic 2" })
            .Build();

        private CourseSetupController controller = null!;
        private CourseSetupController controllerWithCookies = null!;
        private ICourseService courseService = null!;
        private ICourseTopicsService courseTopicsService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private ISectionService sectionService = null!;
        private ITutorialService tutorialService = null!;
        private IConfiguration config = null!;

        [SetUp]
        public void Setup()
        {
            courseService = A.Fake<ICourseService>();
            tutorialService = A.Fake<ITutorialService>();
            sectionService = A.Fake<ISectionService>();
            courseTopicsService = A.Fake<ICourseTopicsService>();
            config = A.Fake<IConfiguration>();

            A.CallTo(() => courseService.GetCentreCourseDetails(A<int>._, A<int?>._)).Returns(details);
            A.CallTo(
                () => courseService.GetApplicationOptionsAlphabeticalListForCentre(A<int>._, A<int?>._, A<int?>._)
            ).Returns(applicationOptions);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();

            controller = new CourseSetupController(
                    courseService,
                    tutorialService,
                    sectionService,
                    courseTopicsService,
                    config
                )
                .WithDefaultContext()
                .WithMockUser(true, 101)
                .WithMockTempData();

            const string cookieName = "CourseFilter";
            const string cookieValue = "Status|Active|false";

            controllerWithCookies = new CourseSetupController(
                    courseService,
                    tutorialService,
                    sectionService,
                    courseTopicsService,
                    config
                )
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true, 101)
                .WithMockTempData();
        }

        [Test]
        public void Index_with_no_query_parameters_uses_cookie_value_for_filterBy()
        {
            // When
            var result = controllerWithCookies.Index();

            // Then
            result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                .Be("Status|Active|false");
        }

        [Test]
        public void Index_with_query_parameters_uses_query_parameter_value_for_filterBy()
        {
            // Given
            const string filterBy = "Status|HideInLearnerPortal|true";

            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = controllerWithCookies.Index(filterBy: filterBy);

            // Then
            result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                .Be(filterBy);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_query_parameter_removes_cookie()
        {
            // Given
            const string filterBy = "CLEAR";

            // When
            var result = controllerWithCookies.Index(filterBy: filterBy);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => httpResponse.Cookies.Delete("CourseFilter")).MustHaveHappened();
                result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                    .BeNull();
            }
        }

        [Test]
        public void Index_with_null_filterBy_and_new_filter_query_parameter_adds_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string newFilterValue = "Status|HideInLearnerPortal|true";

            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = controllerWithCookies.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => httpResponse.Cookies.Append("CourseFilter", newFilterValue, A<CookieOptions>._))
                    .MustHaveHappened();
                result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                    .Be(newFilterValue);
            }
        }

        [Test]
        public void Index_with_CLEAR_filterBy_and_new_filter_query_parameter_sets_new_cookie_value()
        {
            // Given
            const string filterBy = "CLEAR";
            const string newFilterValue = "Status|HideInLearnerPortal|true";

            // When
            var result = controllerWithCookies.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => httpResponse.Cookies.Append("CourseFilter", newFilterValue, A<CookieOptions>._))
                    .MustHaveHappened();
                result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                    .Be(newFilterValue);
            }
        }

        [Test]
        public void Index_with_no_filtering_should_default_to_Active_courses()
        {
            // When
            var result = controller.Index();

            // Then
            result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                .Be("Status|Active|true");
        }

        [Test]
        public void AddCourseNew_sets_new_temp_data()
        {
            // When
            var result = controller.AddCourseNew();

            // Then
            using (new AssertionScope())
            {
                controller.TempData.Peek<AddNewCentreCourseData>().Should().NotBeNull();
                result.Should().BeRedirectToActionResult().WithActionName("SelectCourse");
            }
        }

        [Test]
        public void SelectCourse_post_updates_temp_data_and_redirects()
        {
            var applicationOptionsSelectList = new List<SelectListItem> { new SelectListItem("Test Name", "1") };
            var model = new SelectCourseViewModel(application.ApplicationId, applicationOptionsSelectList);
            SetAddNewCentreCourseTempData();

            // When
            var result = controller.SelectCourse(model);

            // Then
            using (new AssertionScope())
            {
                controller.TempData.Peek<AddNewCentreCourseData>()!.Application.Should()
                    .BeEquivalentTo(application);
                controller.TempData.Peek<AddNewCentreCourseData>()!.SetCourseDetailsModel.Should().BeNull();
                controller.TempData.Peek<AddNewCentreCourseData>()!.SetCourseOptionsModel.Should().BeNull();
                controller.TempData.Peek<AddNewCentreCourseData>()!.SetCourseContentModel.Should().BeNull();
                controller.TempData.Peek<AddNewCentreCourseData>()!.SetSectionContentModels.Should().BeNull();
                result.Should().BeRedirectToActionResult().WithActionName("SetCourseDetails");
            }
        }

        [Test]
        public void SelectCourse_does_not_redirect_with_invalid_model()
        {
            var model = new SelectCourseViewModel { ApplicationId = 1 };
            controller.ModelState.AddModelError("ApplicationId", "Select a course");
            SetAddNewCentreCourseTempData(application);

            // When
            var result = controller.SelectCourse(model);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().ModelAs<SelectCourseViewModel>();
                controller.ModelState["ApplicationId"].Errors[0].ErrorMessage.Should()
                    .Be("Select a course");
            }
        }

        [Test]
        public void
            SaveCourseDetails_correctly_adds_model_error_if_customisation_name_is_not_unique()
        {
            // Given
            var formData = GetSetCourseDetailsViewModel();

            A.CallTo(
                () => courseService.DoesCourseNameExistAtCentre(
                    "Name",
                    101,
                    1,
                    0
                )
            ).Returns(true);

            // When
            var result = controller.SetCourseDetails(formData);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().ModelAs<SetCourseDetailsViewModel>();
                controller.ModelState["CustomisationName"].Errors[0].ErrorMessage.Should()
                    .Be("Course name must be unique, including any additions");
            }
        }

        [Test]
        public void
            SaveCourseDetails_correctly_adds_model_error_if_application_already_exists_with_blank_customisation_name()
        {
            // Given
            var model = GetSetCourseDetailsViewModel(customisationName: "");

            A.CallTo(
                () => courseService.DoesCourseNameExistAtCentre(
                    "",
                    101,
                    1,
                    0
                )
            ).Returns(true);

            // When
            var result = controller.SetCourseDetails(model);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().ModelAs<SetCourseDetailsViewModel>();
                controller.ModelState["CustomisationName"].Errors[0].ErrorMessage.Should()
                    .Be("A course with no add-on already exists");
            }
        }

        [Test]
        public void
            SaveCourseDetails_clears_values_of_conditional_inputs_if_corresponding_checkboxes_or_radios_are_unchecked()
        {
            // Given
            var model = GetSetCourseDetailsViewModel(
                passwordProtected: false,
                receiveNotificationEmails: false,
                isAssessed: true
            );
            SetAddNewCentreCourseTempData();

            A.CallTo(
                () => courseService.DoesCourseNameExistAtCentre(
                    "Name",
                    101,
                    1,
                    0
                )
            ).Returns(false);

            // When
            var result = controller.SetCourseDetails(model);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("SetCourseOptions");
        }

        [Test]
        public void SetCourseDetails_post_updates_temp_data_and_redirects()
        {
            // Given
            var model = GetSetCourseDetailsViewModel();
            SetAddNewCentreCourseTempData();

            // When
            var result = controller.SetCourseDetails(model);

            // Then
            using (new AssertionScope())
            {
                controller.TempData.Peek<AddNewCentreCourseData>()!.SetCourseDetailsModel.Should()
                    .BeEquivalentTo(model);
                result.Should().BeRedirectToActionResult().WithActionName("SetCourseOptions");
            }
        }

        [Test]
        public void SetCourseOptions_post_updates_temp_data_and_redirects()
        {
            // Given
            var model = new EditCourseOptionsFormData(true, true, true);
            SetAddNewCentreCourseTempData();

            // When
            var result = controller.SetCourseOptions(model);

            // Then
            using (new AssertionScope())
            {
                controller.TempData.Peek<AddNewCentreCourseData>()!.SetCourseOptionsModel.Should()
                    .BeEquivalentTo(model);
                result.Should().BeRedirectToActionResult().WithActionName("SetCourseContent");
            }
        }

        [Test]
        public void SetCourseContent_get_redirects_to_summary_if_application_has_no_sections()
        {
            // Given
            SetAddNewCentreCourseTempData(application);

            A.CallTo(
                () => sectionService.GetSectionsThatHaveTutorialsForApplication(1)
            ).Returns(new List<Section>());

            // When
            var result = controller.SetCourseContent();

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Summary");
        }

        [Test]
        public void SetCourseContent_post_updates_temp_data_and_redirects_to_summary_if_IncludeAllSections_is_selected()
        {
            // Given
            var section = new Section(1, "Test name");
            var model = new SetCourseContentViewModel(
                new List<Section> { section },
                true,
                new List<int> { 1 }
            );
            controller.ModelState.AddModelError("SelectedSectionIds", "test message");
            SetAddNewCentreCourseTempData(application);

            A.CallTo(
                () => tutorialService.GetTutorialsForSection(1)
            ).Returns(new List<Tutorial> { new Tutorial(1, "Test name", true, true) });

            // When
            var result = controller.SetCourseContent(model);

            // Then
            using (new AssertionScope())
            {
                controller.TempData.Peek<AddNewCentreCourseData>()!.SetCourseContentModel.Should()
                    .BeEquivalentTo(model);
                result.Should().BeRedirectToActionResult().WithActionName("Summary");
            }
        }

        [Test]
        public void
            SaveCourseContent_does_not_redirect_with_invalid_model()
        {
            // Given
            var sectionModel = new Section(1, "Test name");
            var model = new SetCourseContentViewModel(new List<Section> { sectionModel }, false, null);
            controller.ModelState.AddModelError("SelectedSectionIds", "test message");
            SetAddNewCentreCourseTempData();

            A.CallTo(
                () => tutorialService.GetTutorialsForSection(1)
            ).Returns(new List<Tutorial> { new Tutorial(1, "Test name", true, true) });

            // When
            var result = controller.SetCourseContent(model);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().ModelAs<SetCourseContentViewModel>();
                controller.ModelState["SelectedSectionIds"].Errors[0].ErrorMessage.Should()
                    .Be("test message");
            }
        }

        [Test]
        public void
            SetSectionContent_get_redirects_to_next_section_if_section_has_no_tutorials_and_there_is_another_section()
        {
            // Given
            var section1 = new Section(1, "Test name 1");
            var section2 = new Section(2, "Test name 2");
            var setCourseContentModel = new SetCourseContentViewModel(
                new List<Section> { section1, section2 },
                false,
                new List<int> { 1, 2 }
            );
            SetAddNewCentreCourseTempData(application, setCourseContentModel: setCourseContentModel);

            A.CallTo(
                () => tutorialService.GetTutorialsForSection(1)
            ).Returns(new List<Tutorial>());

            // When
            var result = controller.SetSectionContent(0);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("SetSectionContent");
        }

        [Test]
        public void
            SetSectionContent_get_redirects_to_summary_if_section_has_no_tutorials_and_there_are_no_sections_left()
        {
            // Given
            var section = new Section(1, "Test name 1");
            var setCourseContentModel = new SetCourseContentViewModel(
                new List<Section> { section },
                false,
                new List<int> { 1 }
            );
            SetAddNewCentreCourseTempData(application, setCourseContentModel: setCourseContentModel);

            A.CallTo(
                () => tutorialService.GetTutorialsForSection(1)
            ).Returns(new List<Tutorial>());

            // When
            var result = controller.SetSectionContent(0);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("Summary");
        }

        [Test]
        public void SetSectionContent_post_updates_temp_data_and_redirects_to_next_section_if_there_is_one()
        {
            // Given
            var section1 = new Section(1, "Test name 1");
            var section2 = new Section(2, "Test name 2");
            var model = new SetSectionContentViewModel(section1, 0, true);
            var setCourseContentModel = new SetCourseContentViewModel(
                new List<Section> { section1, section2 },
                false,
                new List<int> { 1, 2 }
            );
            SetAddNewCentreCourseTempData(application, setCourseContentModel: setCourseContentModel);

            A.CallTo(
                () => tutorialService.GetTutorialsForSection(A<int>._)
            ).Returns(new List<Tutorial> { new Tutorial(1, "Test name", true, true) });

            // When
            var result = controller.SetSectionContent(model, "save");

            // Then
            using (new AssertionScope())
            {
                controller.TempData.Peek<AddNewCentreCourseData>()!.SetSectionContentModels.Should()
                    .BeEquivalentTo(new List<SetSectionContentViewModel> { model });
                result.Should().BeRedirectToActionResult().WithActionName("SetSectionContent");
            }
        }

        [Test]
        public void SetSectionContent_post_updates_temp_data_and_redirects_to_summary_if_no_sections_left()
        {
            // Given
            var section = new Section(1, "Test name");
            var model = new SetSectionContentViewModel(section, 0, true);
            var setCourseContentModel = new SetCourseContentViewModel(
                new List<Section> { section },
                false,
                new List<int> { 1 }
            );
            SetAddNewCentreCourseTempData(setCourseContentModel: setCourseContentModel);

            A.CallTo(
                () => tutorialService.GetTutorialsForSection(1)
            ).Returns(new List<Tutorial> { new Tutorial(1, "Test name", true, true) });

            // When
            var result = controller.SetSectionContent(model, "save");

            // Then
            using (new AssertionScope())
            {
                controller.TempData.Peek<AddNewCentreCourseData>()!.SetSectionContentModels.Should()
                    .BeEquivalentTo(new List<SetSectionContentViewModel> { model });
                result.Should().BeRedirectToActionResult().WithActionName("Summary");
            }
        }

        [Test]
        public void Summary_post_resets_temp_data_and_redirects_to_confirmation()
        {
            // Given
            var applicationName = application.ApplicationName;
            var customisationName = GetSetCourseDetailsViewModel().CustomisationName;

            var tutorial = new Tutorial(1, "Tutorial name", true, true);
            var section = new Section(1, "Section name");
            var sectionModel = new SetSectionContentViewModel(
                section,
                0,
                true,
                new List<Tutorial> { tutorial }
            );

            var setCourseOptionsModel = new EditCourseOptionsFormData(true, true, true);
            SetAddNewCentreCourseTempData(
                application,
                GetSetCourseDetailsViewModel(),
                setCourseOptionsModel,
                new SetCourseContentViewModel(),
                new List<SetSectionContentViewModel> { sectionModel }
            );

            A.CallTo(
                () => courseService.CreateNewCentreCourse(A<Customisation>._)
            ).Returns(1);

            A.CallTo(
                () => tutorialService.UpdateTutorialsStatuses(A<IEnumerable<Tutorial>>._, A<int>._)
            ).DoesNothing();

            // When
            var result = controller.CreateNewCentreCourse();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => courseService.CreateNewCentreCourse(A<Customisation>._)
                ).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => tutorialService.UpdateTutorialsStatuses(A<IEnumerable<Tutorial>>._, A<int>._)
                ).MustHaveHappenedOnceExactly();
                controller.TempData.Peek<AddNewCentreCourseData>().Should().BeNull();
                controller.TempData.Peek("customisationId").Should().Be(1);
                controller.TempData.Peek("applicationName").Should().Be(applicationName);
                controller.TempData.Peek("customisationName").Should().Be(customisationName);
                result.Should().BeRedirectToActionResult().WithActionName("Confirmation");
            }
        }

        private static SetCourseDetailsViewModel GetSetCourseDetailsViewModel(
            int applicationId = 1,
            string customisationName = "Name",
            bool passwordProtected = true,
            string password = "Password",
            bool receiveNotificationEmails = true,
            string notificationEmails = "hello@test.com",
            bool postLearningAssessment = true,
            bool isAssessed = false,
            bool diagAssess = true,
            string? tutCompletionThreshold = "90",
            string? diagCompletionThreshold = "75"
        )
        {
            return new SetCourseDetailsViewModel
            {
                ApplicationId = applicationId,
                CustomisationName = customisationName,
                PasswordProtected = passwordProtected,
                Password = password,
                ReceiveNotificationEmails = receiveNotificationEmails,
                NotificationEmails = notificationEmails,
                PostLearningAssessment = postLearningAssessment,
                IsAssessed = isAssessed,
                DiagAssess = diagAssess,
                TutCompletionThreshold = tutCompletionThreshold,
                DiagCompletionThreshold = diagCompletionThreshold,
            };
        }

        private void SetAddNewCentreCourseTempData(
            ApplicationDetails? selectedApplication = null,
            SetCourseDetailsViewModel? setCourseDetailsModel = null,
            EditCourseOptionsFormData setCourseOptionsModel = null!,
            SetCourseContentViewModel setCourseContentModel = null!,
            List<SetSectionContentViewModel>? setSectionContentModels = null
        )
        {
            var initialTempData = new AddNewCentreCourseData
            {
                Application = selectedApplication,
                SetCourseDetailsModel = setCourseDetailsModel,
                SetCourseOptionsModel = setCourseOptionsModel,
                SetCourseContentModel = setCourseContentModel,
                SetSectionContentModels = setSectionContentModels,
            };
            controller.TempData.Set(initialTempData);
        }
    }
}
