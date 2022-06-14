namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Common;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class CourseSetupControllerTests
    {
        private const string CookieName = "CourseFilter";

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

        private readonly List<CourseStatisticsWithAdminFieldResponseCounts> courses =
            new List<CourseStatisticsWithAdminFieldResponseCounts>
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

        private IConfiguration config = null!;

        private CourseSetupController controller = null!;
        private CourseSetupController controllerWithCookies = null!;
        private ICourseService courseService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IMultiPageFormDataService multiPageFormDataService = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private ISectionService sectionService = null!;
        private ITutorialService tutorialService = null!;

        [SetUp]
        public void Setup()
        {
            courseService = A.Fake<ICourseService>();
            tutorialService = A.Fake<ITutorialService>();
            sectionService = A.Fake<ISectionService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            config = A.Fake<IConfiguration>();
            multiPageFormDataService = A.Fake<IMultiPageFormDataService>();

            A.CallTo(
                () => courseService.GetCentreSpecificCourseStatisticsWithAdminFieldResponseCounts(
                    A<int>._,
                    A<int>._,
                    false
                )
            ).Returns(courses);

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
                    searchSortFilterPaginateService,
                    config,
                    multiPageFormDataService
                )
                .WithDefaultContext()
                .WithMockUser(true, 101)
                .WithMockTempData();
            const string cookieValue = "Status|Active|false";

            controllerWithCookies = new CourseSetupController(
                    courseService,
                    tutorialService,
                    sectionService,
                    searchSortFilterPaginateService,
                    config,
                    multiPageFormDataService
                )
                .WithMockHttpContext(httpRequest, CookieName, cookieValue, httpResponse)
                .WithMockUser(true, 101)
                .WithMockTempData();
        }

        [Test]
        public void Index_calls_expected_methods_and_returns_view()
        {
            // When
            var result = controllerWithCookies.Index();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => courseService.GetCentreCourseDetails(A<int>._, A<int?>._)).MustHaveHappened();
                A.CallTo(
                    () => searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                        A<IEnumerable<CourseStatisticsWithAdminFieldResponseCounts>>._,
                        A<SearchSortFilterAndPaginateOptions>._
                    )
                ).MustHaveHappened();
                A.CallTo(
                        () => httpResponse.Cookies.Append(
                            CookieName,
                            A<string>._,
                            A<CookieOptions>._
                        )
                    )
                    .MustHaveHappened();
                result.Should().BeViewResult().WithDefaultViewName();
            }
        }

        [Test]
        public void AddCourseNew_sets_new_temp_data()
        {
            // When
            var result = controller.AddCourseNew();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormDataService.SetMultiPageFormData(
                        A<AddNewCentreCourseData>._,
                        MultiPageFormDataFeature.AddNewCourse,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("SelectCourse");
            }
        }

        [Test]
        public void SelectCourse_post_updates_temp_data_and_redirects()
        {
            SetAddNewCentreCourseTempData();
            A.CallTo(
                () => courseService.GetApplicationOptionsAlphabeticalListForCentre(
                    ControllerContextHelper.CentreId,
                    ControllerContextHelper.AdminCategoryId,
                    null
                )
            ).Returns(new[] { application });

            // When
            var result = controller.SelectCourse(application.ApplicationId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormDataService.SetMultiPageFormData(
                        A<AddNewCentreCourseData>.That.Matches(
                            d => d.Application!.ApplicationId == application.ApplicationId &&
                                d.CourseDetailsData == null &&
                                d.CourseOptionsData == null &&
                                d.CourseContentData == null &&
                                d.SectionContentData == null
                        ),
                        MultiPageFormDataFeature.AddNewCourse,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("SetCourseDetails");
            }
        }

        [Test]
        public void SelectCourse_does_not_redirect_with_null_applicationId()
        {
            SetAddNewCentreCourseTempData();

            // When
            var result = controller.SelectCourse(applicationId: null);

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
                A.CallTo(
                    () => multiPageFormDataService.SetMultiPageFormData(
                        A<AddNewCentreCourseData>.That.Matches(d => d.CourseDetailsData != null),
                        MultiPageFormDataFeature.AddNewCourse,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
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
                A.CallTo(
                    () => multiPageFormDataService.SetMultiPageFormData(
                        A<AddNewCentreCourseData>.That.Matches(d => d.CourseOptionsData != null),
                        MultiPageFormDataFeature.AddNewCourse,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
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
                A.CallTo(
                    () => multiPageFormDataService.SetMultiPageFormData(
                        A<AddNewCentreCourseData>.That.Matches(d => d.CourseContentData != null),
                        MultiPageFormDataFeature.AddNewCourse,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
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
            var setCourseContentModel = new CourseContentData(
                new List<Section> { section1, section2 },
                false,
                new List<int> { 1, 2 }
            );
            SetAddNewCentreCourseTempData(application, courseContentModel: setCourseContentModel);

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
            var setCourseContentModel = new CourseContentData(
                new List<Section> { section },
                false,
                new List<int> { 1 }
            );
            SetAddNewCentreCourseTempData(application, courseContentModel: setCourseContentModel);

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
            var setCourseContentModel = new CourseContentData(
                new List<Section> { section1, section2 },
                false,
                new List<int> { 1, 2 }
            );
            SetAddNewCentreCourseTempData(application, courseContentModel: setCourseContentModel);

            A.CallTo(
                () => tutorialService.GetTutorialsForSection(A<int>._)
            ).Returns(new List<Tutorial> { new Tutorial(1, "Test name", true, true) });

            // When
            var result = controller.SetSectionContent(model, "save");

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormDataService.SetMultiPageFormData(
                        A<AddNewCentreCourseData>.That.Matches(d => d.SectionContentData != null),
                        MultiPageFormDataFeature.AddNewCourse,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeRedirectToActionResult().WithActionName("SetSectionContent");
            }
        }

        [Test]
        public void SetSectionContent_post_updates_temp_data_and_redirects_to_summary_if_no_sections_left()
        {
            // Given
            var section = new Section(1, "Test name");
            var model = new SetSectionContentViewModel(section, 0, true);
            var setCourseContentModel = new CourseContentData(
                new List<Section> { section },
                false,
                new List<int> { 1 }
            );
            SetAddNewCentreCourseTempData(courseContentModel: setCourseContentModel);

            A.CallTo(
                () => tutorialService.GetTutorialsForSection(1)
            ).Returns(new List<Tutorial> { new Tutorial(1, "Test name", true, true) });

            // When
            var result = controller.SetSectionContent(model, "save");

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => multiPageFormDataService.SetMultiPageFormData(
                        A<AddNewCentreCourseData>.That.Matches(d => d.SectionContentData != null),
                        MultiPageFormDataFeature.AddNewCourse,
                        controller.TempData
                    )
                ).MustHaveHappenedOnceExactly();
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
            var sectionData = new SectionContentData(
                section,
                0,
                true,
                new List<Tutorial> { tutorial }
            );

            var setCourseOptionsModel = new CourseOptionsData(true, true, true, true);
            SetAddNewCentreCourseTempData(
                application,
                GetSetCourseDetailsData(GetSetCourseDetailsViewModel()),
                setCourseOptionsModel,
                new CourseContentData(),
                new List<SectionContentData> { sectionData }
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
            string applicationName = "Test",
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
                ApplicationName = applicationName,
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

        private static CourseDetailsData GetSetCourseDetailsData(SetCourseDetailsViewModel model)
        {
            return new CourseDetailsData(
                model.ApplicationId,
                model.ApplicationName,
                model.CustomisationName,
                model.PasswordProtected,
                model.Password,
                model.ReceiveNotificationEmails,
                model.NotificationEmails,
                model.PostLearningAssessment,
                model.IsAssessed,
                model.DiagAssess,
                model.TutCompletionThreshold,
                model.DiagCompletionThreshold
            );
        }

        private void SetAddNewCentreCourseTempData(
            ApplicationDetails? selectedApplication = null,
            CourseDetailsData? setCourseDetailsModel = null,
            CourseOptionsData setCourseOptionsModel = null!,
            CourseContentData courseContentModel = null!,
            List<SectionContentData>? setSectionContentModels = null
        )
        {
            var initialTempData = new AddNewCentreCourseData
            {
                Application = selectedApplication,
                CourseDetailsData = setCourseDetailsModel,
                CourseOptionsData = setCourseOptionsModel,
                CourseContentData = courseContentModel,
                SectionContentData = setSectionContentModels,
            };
            A.CallTo(
                () => multiPageFormDataService.GetMultiPageFormData<AddNewCentreCourseData>(
                    A<MultiPageFormDataFeature>._,
                    A<ITempDataDictionary>._
                )
            ).Returns(initialTempData);
        }
    }
}
