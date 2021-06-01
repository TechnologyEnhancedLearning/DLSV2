﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class DelegateApprovalsControllerTests
    {
        private IDelegateApprovalsService delegateApprovalsService = null!;
        private DelegateApprovalsController delegateApprovalsController = null!;

        [SetUp]
        public void Setup()
        {
            delegateApprovalsService = A.Fake<IDelegateApprovalsService>();
            delegateApprovalsController = new DelegateApprovalsController(delegateApprovalsService);
        }

        [Test]
        public void PostApproveDelegate_calls_correct_method()
        {
            // Given
            A.CallTo(() => delegateApprovalsService.ApproveDelegate(2)).DoesNothing();

            // When
            var result = delegateApprovalsController.ApproveDelegate(2);

            // Then
            A.CallTo(() => delegateApprovalsService.ApproveDelegate(2)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void PostApproveDelegatesForCentre_calls_correct_method()
        {
            // Given
            A.CallTo(() => delegateApprovalsService.ApproveAllUnapprovedDelegatesForCentre(2)).DoesNothing();

            // When
            var result = delegateApprovalsController.ApproveDelegatesForCentre(2);

            // Then
            A.CallTo(() => delegateApprovalsService.ApproveAllUnapprovedDelegatesForCentre(2)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }
    }
}
