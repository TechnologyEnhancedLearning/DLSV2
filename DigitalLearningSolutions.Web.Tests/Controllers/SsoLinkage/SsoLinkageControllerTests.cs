using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Utilities;
using DigitalLearningSolutions.Web.Controllers;
using DigitalLearningSolutions.Web.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using FluentAssertions.Execution;
using Pipelines.Sockets.Unofficial;
using System.Security.Claims;
using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using DigitalLearningSolutions.Data.Models.Signposting;

namespace DigitalLearningSolutions.Web.Tests.Controllers.SsoLinkage
{
    internal class SsoLinkageControllerTests
    {
        private ILogger<SsoLinkageController> logger = null!;
        private IUserService userService = null!;
        private ILearningHubLinkService learningHubLinkService = null!;
        private SsoLinkageController controller = null!;

        [SetUp]
        public void SetUp()
        {
            logger = A.Fake<ILogger<SsoLinkageController>>();
            userService = A.Fake<IUserService>();
            learningHubLinkService = A.Fake<ILearningHubLinkService>();

            controller = new SsoLinkageController(
                logger,
                userService,
                learningHubLinkService
                )
                .WithDefaultContext()
                .WithMockUser(false);
        }


        [Test]
        public void Index_should_redirect_to_home_index()
        {
            //Given - User is already linked to LH account
            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>.Ignored)).Returns(67890);
           
            // When
            var result = controller.Index();

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Index_should_render_SsoLink_form()
        {
            //Given - User is not linked to LH account
            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>.Ignored)).Returns(null);

            // When
            var result = controller.Index();

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void LinkAccount_should_redirect_to_home_index()
        {
            // Given - User is already linked to LH account
            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>.Ignored)).Returns(67890);

            // When
            var result = controller.LinkAccount();

            // Then
            result.Should().BeRedirectToActionResult()
                .WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void LinkAccount_should_redirect_to_linkingurl()
        {
            // Given - User is already linked to LH account
            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>.Ignored)).Returns(null);
            controller.HttpContext.Session = A.Fake<ISession>();
            A.CallTo(() => learningHubLinkService.GetLinkingUrl(A<string>.Ignored)).Returns("https://someurl.com");

            // When
            var result = controller.LinkAccount();

            // Then
            result.Should().BeRedirectResult("https://someurl.com");
        }

        [Test]
        public void AccountLinked_should_render_account_linked_page()
        {
            // Given - ModelState is valid
            LinkLearningHubRequest linkLearningHubRequest= new LinkLearningHubRequest();
            controller.HttpContext.Session = A.Fake<ISession>();

            // When
            var result = controller.AccountLinked(linkLearningHubRequest);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }
    }
}
