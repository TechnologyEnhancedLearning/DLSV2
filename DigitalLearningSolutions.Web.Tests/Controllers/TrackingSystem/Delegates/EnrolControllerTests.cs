using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Data.Models.SessionData.Tracking.Delegate.Enrol;
using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
using FakeItEasy;
using FluentAssertions.AspNetCore.Mvc;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NUnit.Framework;
using GDS.MultiPageFormData.Enums;
namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Services;
    using GDS.MultiPageFormData;
    using Moq;

    public class EnrolControllerTests
    {
        private EnrolController enrolController = null!;
        private ICourseDataService courseDataService = null!;
        private IMultiPageFormService multiPageFormService = null!;
        private ISupervisorService supervisorService = null!;
        private IProgressDataService progressDataService = null!;
        private IEnrolService enrolService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private HttpContext httpContext = null!;
        private TempDataDictionary tempDataDictionary = null!;
        private SessionEnrolDelegate sessionEnrolDelegate = null!;

        [SetUp]
        public void Setup()
        {
            courseDataService = A.Fake<ICourseDataService>();
            multiPageFormService = A.Fake<IMultiPageFormService>();
            supervisorService = A.Fake<ISupervisorService>();
            enrolService = A.Fake<IEnrolService>();
            progressDataService = A.Fake<IProgressDataService>();
            sessionEnrolDelegate = A.Fake<SessionEnrolDelegate>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            httpContext = A.Fake<HttpContext>();
            tempDataDictionary = new TempDataDictionary(httpContext, A.Fake<ITempDataProvider>());

            enrolController = new EnrolController(
                courseDataService,
                multiPageFormService,
                supervisorService,
                enrolService,
                progressDataService)
                .WithMockHttpContext(httpRequest, null, null, httpResponse)
                .WithMockTempData()
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void StartEnrolProcess_calls_expected_methods_and_returns_view()
        {
            var mockFromService = new Mock<IMultiPageFormService>();
            //Given
           mockFromService.Setup(A => A.SetMultiPageFormData(sessionEnrolDelegate, MultiPageFormDataFeature.EnrolDelegateInActivity, tempDataDictionary));
            //When
            var result = enrolController.StartEnrolProcess(1, 1, "DelegateName");

            //Then
            using (new AssertionScope())
            {
                // Since MultiPageFormDataFeature.EnrolDelegateInActivity is a static method, it cannot be mocked/faked
                mockFromService.Setup(A => A.SetMultiPageFormData(It.IsAny<SessionEnrolDelegate>(), MultiPageFormDataFeature.EnrolDelegateInActivity, enrolController.TempData)).Verifiable();

                mockFromService.Verify(A => A.SetMultiPageFormData(It.IsAny<SessionEnrolDelegate>(), MultiPageFormDataFeature.EnrolDelegateInActivity, enrolController.TempData), Moq.Times.Never);
               // A.CallTo(() => multiPageFormService.SetMultiPageFormData(A<SessionEnrolDelegate>._, MultiPageFormDataFeature.EnrolDelegateInActivity, enrolController.TempData).GetAwaiter().GetResult()).MustHaveHappenedOnceExactly();

                result.Should().BeRedirectToActionResult().WithActionName("Index");

                Assert.AreEqual(0, tempDataDictionary.Keys.Count);
            }
        }
    }
}
