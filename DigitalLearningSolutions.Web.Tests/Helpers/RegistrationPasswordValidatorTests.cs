using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NUnit.Framework;
using DigitalLearningSolutions.Web.Helpers;
using FluentAssertions;

namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    public class RegistrationPasswordValidatorTests
    {
        [Test]
        [TestCase(null, "Fred", "Bloggs", 0)]
        [TestCase("sdfEE123$$", "Fred", "Bloggs", 0)]
        [TestCase("Fredfred123!", "Fred", "Bloggs", 1)]
        [TestCase("bloggsbloggs$$2", "Fred", "Bloggs", 1)]
        public void ValidatePassword_modified_modelState_correctly(string? password, string forename, string surname, int resultLen)
        {
            //Given
            ModelStateDictionary modelState = new ModelStateDictionary();

            // When
            RegistrationPasswordValidator.ValidatePassword(password, forename, surname, modelState);

            //Then
            modelState.Count.Should().Be(resultLen);
            if(resultLen == 0)
            {
                modelState.IsValid.Should().BeTrue();
            }
            else
            {
                modelState.IsValid.Should().BeFalse();
            }
        }
    }
}
