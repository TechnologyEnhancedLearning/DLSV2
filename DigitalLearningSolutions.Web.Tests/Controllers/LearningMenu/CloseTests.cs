﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public partial class LearningMenuControllerTests
    {
        [Test]
        public void Close_should_close_sessions()
        {
            var learningActivity = "Current";
            // When
            controller.Close(learningActivity);

            // Then
            A.CallTo(() => sessionService.StopDelegateSession(CandidateId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StopDelegateSession(A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, ISession _) => candidateId != CandidateId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Close_should_redirect_to_Current_LearningPortal()
        {
            var learningActivity = "Current";
            // When
            var result = controller.Close(learningActivity);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningPortal")
                .WithActionName("Current");
        }
    }
}
