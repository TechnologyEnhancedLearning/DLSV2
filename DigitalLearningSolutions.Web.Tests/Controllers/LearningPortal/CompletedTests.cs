//namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
//{
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Threading.Tasks;
//    using DigitalLearningSolutions.Data.Helpers;
//    using DigitalLearningSolutions.Data.Models.Courses;
//    using DigitalLearningSolutions.Data.Models.LearningResources;
//    using DigitalLearningSolutions.Web.Tests.TestHelpers;
//    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed;
//    using FakeItEasy;
//    using FizzWare.NBuilder;
//    using FluentAssertions;
//    using FluentAssertions.AspNetCore.Mvc;
//    using NUnit.Framework;

//    public partial class LearningPortalControllerTests
//    {
//        [TestCase(false)]
//        [TestCase(true)]
//        public async Task Completed_action_should_return_view_result_with_correct_api_accessibility_flag(
//            bool apiIsAccessible
//        )
//        {
//            // Given
//            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
//            var completedCourses = new[]
//            {
//                CompletedCourseHelper.CreateDefaultCompletedCourse(),
//                CompletedCourseHelper.CreateDefaultCompletedCourse(),
//            };
//            var completedActionPlanResources = Builder<ActionPlanResource>.CreateListOfSize(2).Build().ToList();
//            var mappedActionPlanResources =
//                completedActionPlanResources.Select(r => new CompletedActionPlanResource(r));
//            var bannerText = "bannerText";
//            A.CallTo(() => courseDataService.GetCompletedCourses(CandidateId)).Returns(completedCourses);
//            A.CallTo(() => actionPlanService.GetCompletedActionPlanResources(CandidateId))
//                .Returns((completedActionPlanResources, apiIsAccessible));
//            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

//            // When
//            var result = await controller.Completed();

//            // Then
//            var expectedModel = new CompletedPageViewModel(
//                completedCourses,
//                mappedActionPlanResources,
//                apiIsAccessible,
//                config,
//                null,
//                "Completed",
//                "Descending",
//                bannerText,
//                1
//            );
//            result.Should().BeViewResult()
//                .Model.Should().BeEquivalentTo(expectedModel);
//        }

//        [Test]
//        public async Task Completed_action_should_have_banner_text()
//        {
//            // Given
//            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");
//            const string bannerText = "Banner text";
//            A.CallTo(() => centresDataService.GetBannerText(CentreId)).Returns(bannerText);

//            // When
//            var completedViewModel = await CompletedCourseHelper.CompletedViewModelFromController(controller);

//            // Then
//            completedViewModel.BannerText.Should().Be(bannerText);
//        }

//        [Test]
//        public async Task Completed_action_should_not_fetch_ActionPlanResources_if_signposting_disabled()
//        {
//            // Given
//            GivenCompletedActivitiesAreEmptyLists();
//            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("false");

//            // When
//            await controller.Completed();

//            // Then
//            A.CallTo(() => actionPlanService.GetCompletedActionPlanResources(CandidateId)).MustNotHaveHappened();
//        }

//        [Test]
//        public async Task Completed_action_should_fetch_ActionPlanResources_if_signposting_enabled()
//        {
//            // Given
//            GivenCompletedActivitiesAreEmptyLists();
//            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");

//            // When
//            await controller.Completed();

//            // Then
//            A.CallTo(() => actionPlanService.GetCompletedActionPlanResources(CandidateId))
//                .MustHaveHappenedOnceExactly();
//        }

//        [Test]
//        public async Task AllCompletedItems_action_should_not_fetch_ActionPlanResources_if_signposting_disabled()
//        {
//            // Given
//            GivenCompletedActivitiesAreEmptyLists();
//            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("false");

//            // When
//            await controller.AllCompletedItems();

//            // Then
//            A.CallTo(() => actionPlanService.GetCompletedActionPlanResources(CandidateId)).MustNotHaveHappened();
//        }

//        [Test]
//        public async Task AllCompletedItems_action_should_fetch_ActionPlanResources_if_signposting_enabled()
//        {
//            // Given
//            GivenCompletedActivitiesAreEmptyLists();
//            A.CallTo(() => config[ConfigHelper.UseSignposting]).Returns("true");

//            // When
//            await controller.AllCompletedItems();

//            // Then
//            A.CallTo(() => actionPlanService.GetCompletedActionPlanResources(CandidateId))
//                .MustHaveHappenedOnceExactly();
//        }

//        private void GivenCompletedActivitiesAreEmptyLists()
//        {
//            A.CallTo(() => courseDataService.GetCompletedCourses(A<int>._)).Returns(new List<CompletedCourse>());
//            A.CallTo(() => actionPlanService.GetCompletedActionPlanResources(A<int>._))
//                .Returns((new List<ActionPlanResource>(), false));
//            A.CallTo(() => centresDataService.GetBannerText(A<int>._)).Returns("bannerText");
//        }
//    }
//}
