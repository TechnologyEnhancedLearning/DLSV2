﻿using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Data.Models.SessionData.Tracking.Delegate.Enrol;
using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
using FakeItEasy;
using FluentAssertions.AspNetCore.Mvc;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NUnit.Framework;
using GDS.MultiPageFormData;
using GDS.MultiPageFormData.Enums;

namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Services;

    public class EnrolControllerTests
    {
        private EnrolController enrolController = null!;
        private IMultiPageFormService multiPageFormService = null!;
        private ISupervisorService supervisorService = null!;
        private ICourseService courseService = null!;
        private IEnrolService enrolService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private HttpContext httpContext = null!;
        private TempDataDictionary tempDataDictionary = null!;
        private SessionEnrolDelegate sessionEnrolDelegate = null!;

        [SetUp]
        public void Setup()
        {
            multiPageFormService = A.Fake<IMultiPageFormService>();
            supervisorService = A.Fake<ISupervisorService>();
            enrolService = A.Fake<IEnrolService>();
            courseService = A.Fake<ICourseService>();
            sessionEnrolDelegate = A.Fake<SessionEnrolDelegate>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            httpContext = A.Fake<HttpContext>();
            tempDataDictionary = new TempDataDictionary(httpContext, A.Fake<ITempDataProvider>());

            enrolController = new EnrolController(
                multiPageFormService,
                supervisorService,
                enrolService,
                courseService)
                .WithMockHttpContext(httpRequest, null, null, httpResponse)
                .WithMockTempData()
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void StartEnrolProcess_calls_expected_methods_and_returns_view()
        {
            //Given
            A.CallTo(() => multiPageFormService.SetMultiPageFormData(sessionEnrolDelegate,
                MultiPageFormDataFeature.EnrolDelegateInActivity,
                tempDataDictionary));

            //When
            var result = enrolController.StartEnrolProcess(1, 1, "DelegateName");

            //Then
            using (new AssertionScope())
            {
                // Since MultiPageFormDataFeature.EnrolDelegateInActivity is a static method, it cannot be mocked/faked
                A.CallTo(() => multiPageFormService.SetMultiPageFormData(A<SessionEnrolDelegate>._, MultiPageFormDataFeature.EnrolDelegateInActivity, enrolController.TempData)).MustHaveHappenedOnceExactly();

                result.Should().BeRedirectToActionResult().WithActionName("Index");

                Assert.AreEqual(0, tempDataDictionary.Keys.Count);
            }
        }
    }
}
