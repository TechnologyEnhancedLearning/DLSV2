namespace DigitalLearningSolutions.Web.Tests.Controllers.Support
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.Support;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Support.Faqs;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class FaqControllerTests
    {
        // TODO HEEDLS-608 Review below tests to determine if any in here are excess to requirements,
        // i.e. if the test is covered under HEEDLS-608 tests

        private readonly IEnumerable<Faq> Faqs = new List<Faq>
        {
            FaqTestHelper.GetDefaultFaq(),
            FaqTestHelper.GetDefaultFaq(2, weighting: 95, aHtml: "word documents", qText: "doc help"),
            FaqTestHelper.GetDefaultFaq(3, weighting: 75, aHtml: "excel documents", qText: "spreadsheets"),
        };

        private IConfiguration configuration = null!;
        private IFaqsService faqsService = null!;
        private IFeatureManager featureManager = null!;

        [SetUp]
        public void Setup()
        {
            featureManager = A.Fake<IFeatureManager>();
            configuration = A.Fake<IConfiguration>();
            faqsService = A.Fake<IFaqsService>();
            A.CallTo(() => faqsService.GetPublishedFaqsForTargetGroup(0))
                .Returns(Faqs);
            A.CallTo(() => faqsService.GetPublishedFaqByIdForTargetGroup(2, 0))
                .Returns(Faqs.ElementAt(1));
            A.CallTo(() => faqsService.GetPublishedFaqByIdForTargetGroup(5, 0))
                .Returns(null);
            A.CallTo(() => featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem))
                .Returns(true);
        }

        [Test]
        public async Task TrackingSystem_Faqs_page_should_be_shown_for_valid_claims()
        {
            // Given
            var controller = new FaqsController(featureManager, configuration, faqsService)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true);

            // When
            var result = await controller.Index(DlsSubApplication.TrackingSystem);

            // Then
            result.Should().BeViewResult().WithViewName("Faqs");
        }

        [Test]
        public async Task Frameworks_Faqs_page_should_be_shown_for_valid_claims()
        {
            // Given
            var controller = new FaqsController(featureManager, configuration, faqsService)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: false, isFrameworkDeveloper: true);

            // When
            var result = await controller.Index(DlsSubApplication.Frameworks);

            // Then
            result.Should().BeViewResult().WithViewName("Faqs");
        }

        [Test]
        public async Task Invalid_application_name_should_redirect_to_404_page()
        {
            // Given
            var controller = new FaqsController(featureManager, configuration, faqsService)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true, isFrameworkDeveloper: true);

            // When
            var result = await controller.Index(DlsSubApplication.Supervisor);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public async Task Home_page_should_be_shown_when_accessing_tracking_system_faqs_without_appropriate_claims()
        {
            // Given
            var controller = new FaqsController(featureManager, configuration, faqsService)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: false, isFrameworkDeveloper: true);

            // When
            var result = await controller.Index(DlsSubApplication.TrackingSystem);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task
            Home_page_should_be_shown_when_accessing_tracking_system_with_refactored_tracking_system_disabled()
        {
            // Given
            A.CallTo(() => featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem))
                .Returns(false);
            var controller = new FaqsController(featureManager, configuration, faqsService)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: false, isFrameworkDeveloper: true);

            // When
            var result = await controller.Index(DlsSubApplication.TrackingSystem);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Home_page_should_be_shown_when_accessing_frameworks_faqs_without_appropriate_claims()
        {
            // Given
            var controller = new FaqsController(featureManager, configuration, faqsService)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true, isFrameworkDeveloper: false);

            // When
            var result = await controller.Index(DlsSubApplication.Frameworks);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task TrackingSystem_AllFaqs_page_should_be_shown_for_valid_claims()
        {
            // Given
            var controller = new FaqsController(featureManager, configuration, faqsService)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true);

            // When
            var result = await controller.AllFaqItems(DlsSubApplication.TrackingSystem);

            // Then
            result.Should().BeViewResult().WithDefaultViewName("AllFaqItems");
        }

        [Test]
        public async Task TrackingSystem_Faq_view_page_should_be_shown_for_valid_claims()
        {
            // Given
            var controller = new FaqsController(featureManager, configuration, faqsService)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true);

            // When
            var result = await controller.ViewFaq(DlsSubApplication.TrackingSystem, 2);

            // Then
            result.Should().BeViewResult().WithDefaultViewName("ViewFaq")
                .Model.Should().BeOfType<SearchableFaqViewModel>()
                .Subject.Faq.FaqId.Should().Be(2);
        }

        [Test]
        public async Task Not_Found_page_should_be_shown_for_invalid_faq_item_view_request()
        {
            // Given
            var controller = new FaqsController(featureManager, configuration, faqsService)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true);

            // When
            var result = await controller.ViewFaq(DlsSubApplication.TrackingSystem, 5);

            // Then
            result.Should().BeNotFoundResult();
        }
    }
}
