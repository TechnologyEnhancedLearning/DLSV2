using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.ViewComponents;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using NUnit.Framework;

    public class ViewComponentValueToSetHelperTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ValueToSetForSimpleType_returns_model_value()
        {
            // Given
            var aspFor = "TestElement";
            var populateWithCurrentValue = true;
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            IEnumerable<string> errorMessages;

            var model = new { TestElement = "SimpleValue" };

            // When
            var result =
                ViewComponentValueToSetHelper.ValueToSetForSimpleType(
                    model, aspFor, populateWithCurrentValue, viewData, out errorMessages
                );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo("SimpleValue");
            }
        }

        //[Test]
        //public void ValueToSetForComplexType_returns_model_value()
        //{
        //    // Given
        //    var aspFor = "SubElement.SubValue";
        //    var populateWithCurrentValue = true;
        //    var modelState = new ModelStateDictionary();
        //    var modelMetadataProvider = new EmptyModelMetadataProvider();
        //    var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        //    IEnumerable<string> errorMessages;
        //    var types = aspFor.Split('.');

        //    var modelLeaf = new { Name = "NameValue" };
        //    var simpleModel = new { TestElement = modelLeaf };
        //    var complexModel = new { SubModel = simpleModel };

        //    // When
        //    var result =
        //        ViewComponentValueToSetHelper.ValueToSetForComplexType(
        //            complexModel, aspFor, populateWithCurrentValue, types, viewData, out errorMessages
        //        );

        //    // Then
        //    using (new AssertionScope())
        //    {
        //        result.Should().BeEquivalentTo("SubValue");
        //    }
        //}
    }
}
