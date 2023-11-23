using System.Collections.Generic;
using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Web.Controllers;
using DigitalLearningSolutions.Web.ViewModels.UserFeedback;
using GDS.MultiPageFormData;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using FluentAssertions;
using FakeItEasy;
using DigitalLearningSolutions.Data.Models.UserFeedback;
using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
using GDS.MultiPageFormData.Enums;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    public class UserFeedbackControllerTests
    {
        private const int LoggedInUserId = 1;
        private const string SourceUrl = "https://www.example.com";
        private const string SourcePageTitle = "DLS Example Page Title";
        private const string FeedbackText = "Example feedback text";

        private UserFeedbackController _userFeedbackController;
        private IUserFeedbackDataService _userFeedbackDataService = null!;
        private IMultiPageFormService _multiPageFormService = null!;
        private ITempDataDictionary _tempData = null;

        [SetUp]
        public void SetUp()
        {
            _userFeedbackDataService = A.Fake<IUserFeedbackDataService>();
            _multiPageFormService = A.Fake<IMultiPageFormService>();
            _userFeedbackController = new UserFeedbackController(_userFeedbackDataService, _multiPageFormService)
                .WithDefaultContext()
                .WithMockUser(true, userId: LoggedInUserId);
            _tempData = A.Fake<ITempDataDictionary>();
            _userFeedbackController.TempData = _tempData;
        }

        [Test]
        public void UserFeedbackTaskAchieved_ShouldReturnCorrectView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackTaskAchieved(userFeedbackViewModel) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("UserFeedbackTaskAchieved");
            result?.Model.Should().BeOfType<UserFeedbackViewModel>();
        }

        [Test]
        public void UserFeedbackTaskAttempted_ShouldReturnCorrectView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackTaskAttempted(userFeedbackViewModel) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("UserFeedbackTaskAttempted");
            result?.Model.Should().BeOfType<UserFeedbackViewModel>();
        }

        [Test]
        public void UserFeedbackTaskDifficulty_ShouldReturnCorrectView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackTaskDifficulty(userFeedbackViewModel) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("UserFeedbackTaskDifficulty");
            result?.Model.Should().BeOfType<UserFeedbackViewModel>();
        }

        [Test]
        public void UserFeedbackComplete_ShouldReturnCorrectView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackComplete(userFeedbackViewModel) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("UserFeedbackComplete");
            result?.Model.Should().BeOfType<UserFeedbackViewModel>();
        }

        [Test]
        public void GuestFeedbackStart_ShouldReturnCorrectView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.GuestFeedbackStart(userFeedbackViewModel) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("GuestFeedbackStart");
            result?.Model.Should().BeOfType<UserFeedbackViewModel>();
        }

        [Test]
        public void GuestFeedbackComplete_Get_ShouldReturnCorrectView()
        {
            // When
            var result = _userFeedbackController.GuestFeedbackComplete() as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("GuestFeedbackComplete");
            result?.Model.Should().BeOfType<UserFeedbackViewModel>();
        }

        [Test]
        public void Index_WithNullUserId_ShouldRedirectToGuestFeedbackStart()
        {
            // Given
            _userFeedbackController.WithDefaultContext().WithMockUser(false, userId: null);

            // When
            var result = _userFeedbackController.Index(sourceUrl: SourceUrl, sourcePageTitle: SourcePageTitle) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("GuestFeedbackStart");
        }

        [Test]
        public void Index_WithNonNullUserId_ShouldRedirectToStartUserFeedbackSession()
        {
            // Given
            _userFeedbackController.WithDefaultContext().WithMockUser(false, userId: null);

            // When
            var result = _userFeedbackController.Index(sourceUrl: SourceUrl, sourcePageTitle:SourcePageTitle) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("GuestFeedbackStart");
        }

        [Test]
        public void UserFeedbackTaskAchievedSet_ShouldRedirectToUserFeedbackTaskAttempted()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackTaskAchievedSet(userFeedbackViewModel) as RedirectToActionResult;

            // Then
            result.Should().NotBeNull();
            result?.ActionName.Should().Be("UserFeedbackTaskAttempted");
        }

        [Test]
        public void UserFeedbackTaskAttemptedSet_ShouldRedirectToUserFeedbackTaskDifficulty()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackTaskAttemptedSet(userFeedbackViewModel) as RedirectToActionResult;

            // Then
            result.Should().NotBeNull();
            result?.ActionName.Should().Be("UserFeedbackTaskDifficulty");
        }

        [Test]
        public void UserFeedbackTaskDifficultySet_ShouldRedirectToUserFeedbackSave()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackTaskDifficultySet(userFeedbackViewModel) as RedirectToActionResult;

            // Then
            result.Should().NotBeNull();
            result?.ActionName.Should().Be("UserFeedbackSave");
        }

        [Test]
        public void UserFeedbackSave_ShouldCallSaveUserFeedbackAndRedirectToUserFeedbackComplete()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();
            A.CallTo(() => _multiPageFormService.GetMultiPageFormData<UserFeedbackTempData>(
                    MultiPageFormDataFeature.AddUserFeedback, _tempData))
                .Returns(new UserFeedbackTempData());

            // When
            var result = _userFeedbackController.UserFeedbackSave(userFeedbackViewModel) as RedirectToActionResult;

            // Then
            A.CallTo(() => _userFeedbackDataService.SaveUserFeedback(A<int?>._, A<string>._, A<string>._, A<bool?>._, A<string>._, A<string>._, A<int?>._))
                .MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result?.ActionName.Should().Be("UserFeedbackComplete");
        }

        [Test]
        public void GuestFeedbackComplete_ShouldCallSaveUserFeedbackAndRenderGuestFeedbackCompleteView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();
            userFeedbackViewModel.FeedbackText = FeedbackText;

            // When
            var result = _userFeedbackController.GuestFeedbackComplete(userFeedbackViewModel) as RedirectToActionResult;

            // Then
            A.CallTo(() => _userFeedbackDataService.SaveUserFeedback(null, A<string>._, A<string>._, null, A<string>._, A<string>._, null))
                .MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result?.ActionName.Should().Be("GuestFeedbackComplete");
        }

        [Test]
        public void GuestFeedbackComplete_ShouldNotCallSaveUserFeedbackIfNoFeedbackProvided()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.GuestFeedbackComplete(userFeedbackViewModel) as RedirectToActionResult;

            // Then
            A.CallTo(() => _userFeedbackDataService.SaveUserFeedback(null, A<string>._, A<string>._, null, A<string>._, A<string>._, null))
                .MustNotHaveHappened();
            result.Should().NotBeNull();
            result?.ActionName.Should().Be("GuestFeedbackComplete");
        }

        [Test]
        public void UserFeedbackReturnToUrl_ShouldRedirectToSourceUrl()
        {
            // Given
            string sourceUrl = "https://example.com";

            // When
            var result = _userFeedbackController.UserFeedbackReturnToUrl(sourceUrl) as RedirectResult;

            // Then
            result.Should().NotBeNull();
            result?.Url.Should().Be(sourceUrl);
        }

        [Test]
        public void StartUserFeedbackSession_ShouldSetTempDataAndRedirectToUserFeedbackTaskAchieved()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();
            
            // When
            var result = _userFeedbackController.StartUserFeedbackSession(userFeedbackViewModel) as RedirectToActionResult;

            // Then
            A.CallTo(() => _multiPageFormService.SetMultiPageFormData(A<UserFeedbackTempData>._, MultiPageFormDataFeature.AddUserFeedback, _tempData))
                .MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result?.ActionName.Should().Be("UserFeedbackTaskAchieved");
        }

        [Test]
        public void UserFeedbackTaskAchieved_ShouldRenderUserFeedbackTaskAchievedView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackTaskAchieved(userFeedbackViewModel) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("UserFeedbackTaskAchieved");
        }

        [Test]
        public void UserFeedbackTaskAttempted_ShouldRenderUserFeedbackTaskAttemptedView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackTaskAttempted(userFeedbackViewModel) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("UserFeedbackTaskAttempted");
        }

        [Test]
        public void UserFeedbackTaskDifficulty_ShouldRenderUserFeedbackTaskDifficultyView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackTaskDifficulty(userFeedbackViewModel) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("UserFeedbackTaskDifficulty");
        }

        [Test]
        public void GuestFeedbackStart_ShouldRenderGuestFeedbackStartView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.GuestFeedbackStart(userFeedbackViewModel) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("GuestFeedbackStart");
        }

        [Test]
        public void UserFeedbackComplete_ShouldRenderUserFeedbackCompleteView()
        {
            // Given
            var userFeedbackViewModel = new UserFeedbackViewModel();

            // When
            var result = _userFeedbackController.UserFeedbackComplete(userFeedbackViewModel) as ViewResult;

            // Then
            result.Should().NotBeNull();
            result?.ViewName.Should().Be("UserFeedbackComplete");
        }
    }
}
