namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.FrameworksController;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class RedirectToErrorEmptySessionDataTests
    {
        private ActionExecutingContext? _context;
        private FrameworksController? _controller;

        [SetUp]
        public void Setup()
        {
            _controller =
                new FrameworksController(
                        A.Fake<IFrameworkService>(),
                        A.Fake<ICommonService>(),
                        A.Fake<IFrameworkNotificationService>(),
                        A.Fake<ILogger<FrameworksController>>(),
                        A.Fake<IImportCompetenciesFromFileService>(),
                        A.Fake<ICompetencyLearningResourcesDataService>(),
                        A.Fake<ILearningHubApiClient>(),
                        A.Fake<ISearchSortFilterPaginateService>(),
                        A.Fake<IMultiPageFormService>()
                    )
                    .WithDefaultContext().WithMockTempData();


            _context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                _controller
            );
        }

        [Test]
        public void Raises_410_error_if_no_temp_data_matching_model()
        {
            // Given

            // When
            new RedirectEmptySessionData<MultiPageFormDataFeature>().OnActionExecuting(_context!);

            // Then
            _context!.Result.Should().BeRedirectToActionResult(HttpStatusCode.Gone.ToString(), null);
        }

        [Test]
        public void Does_not_raise_410_error_if_there_is_temp_data_matching_model()
        {
            // Given
            _controller!.TempData[MultiPageFormDataFeature.AddNewFramework.TempDataKey] = Guid.NewGuid();

            // When
            new RedirectEmptySessionData<ResetPasswordData>().OnActionExecuting(_context!);

            // Then
            _context!.Result.Should().BeRedirectToActionResult();
        }
    }
}

