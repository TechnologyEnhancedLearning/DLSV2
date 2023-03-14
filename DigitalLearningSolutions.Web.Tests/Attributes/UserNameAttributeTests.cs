namespace DigitalLearningSolutions.Web.Tests.Attributes
{
    using System;
    using System.Security.Claims;
    using System.Collections.Generic;

    using NUnit.Framework;
    using DigitalLearningSolutions.Web.Attributes;
    using FakeItEasy;
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;


    public class UserNameAttributeTests
    {
        [Test]
        [TestCase("Fred", "Bloggs", "R2d2$$c£PO", false)]
        [TestCase("Fred", "Bloggs", "fredbloggs", true)]
        [TestCase("Fred", "Bloggs", "Bloggs1234", true)]
        public void Password_might_contain_part_of_username(string forename, string surname, string password, bool expectedResult)
        {
            // Given
            IList<Claim> claimCollection = new List<Claim>()
            {
                new Claim("UserForename", forename),
                new Claim("UserSurname", surname)
            };

            var fakeClaimsPrincipal = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => fakeClaimsPrincipal.Claims).Returns(claimCollection);

            var fakeHttpContext = A.Fake<HttpContext>();
            A.CallTo(() => fakeHttpContext.User).Returns(fakeClaimsPrincipal);

            var fakeService = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => fakeService.HttpContext).Returns(fakeHttpContext);

            var fakeServiceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => fakeServiceProvider.GetService(typeof(IHttpContextAccessor))).Returns(fakeService);

            PasswordData passwordData = new PasswordData() { Password = password };
            var validationContext = new ValidationContext(passwordData, fakeServiceProvider, null);

            var validationResults = new List<ValidationResult>();

            // When
            Validator.TryValidateObject(passwordData, validationContext, validationResults, true);

            // Then
            Assert.AreEqual(expectedResult, (validationResults.Count > 0));
        }

        private class PasswordData
        {
            [UserName("error message")]
            public string? Password { get; set; }
        }
    }
}
