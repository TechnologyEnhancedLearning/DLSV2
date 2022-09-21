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
        private ModelStateDictionary? _modelState;
        private EmptyModelMetadataProvider? _modelMetadataProvider;
        private ViewDataDictionary? _viewData;
        private bool _populateWithCurrentValue;

        [SetUp]
        public void Setup()
        {
            _modelState = new ModelStateDictionary();
            _modelMetadataProvider = new EmptyModelMetadataProvider();
            _viewData = new ViewDataDictionary(_modelMetadataProvider, _modelState);
            _populateWithCurrentValue = true;
        }

        [Test]
        public void ValueToSetForSimpleType_returns_model_value()
        {
            // Given
            const string aspFor = "Description";
            const string modelItemValue = "Framework description text.";
            var model = new { Description = modelItemValue };

            // When
            var result =
                ViewComponentValueToSetHelper.ValueToSetForSimpleType(
                    model, aspFor, _populateWithCurrentValue, _viewData!, out var errorMessages
                );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(modelItemValue);
                errorMessages.Should().HaveCount(0);
            }
        }

        [Test]
        public void ValueToSetForComplexType_returns_model_value()
        {
            // Given
            const string subModelItemValue = "Description of competency group base.";
            var subModelItem = new { Description = subModelItemValue };
            var complexModel = new { CompetencyGroupBase = subModelItem };

            const string aspFor = "CompetencyGroupBase.Description";
            var types = aspFor.Split('.');

            // When
            var result =
                ViewComponentValueToSetHelper.ValueToSetForComplexType(
                    complexModel, aspFor, _populateWithCurrentValue, types, _viewData!, out var errorMessages
                );

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(subModelItemValue);
                errorMessages.Should().HaveCount(0);
            }
        }

        [Test]
        public void DeriveValueToSet_returns_derived_complex_model_value()
        {
            // Given
            const string subModelItemValue = "Description of competency.";
            var subModelItem = new { Description = subModelItemValue };
            var complexModel = new { Competency = subModelItem };

            var aspFor = "Competency.Description";

            // When
            var result = ViewComponentValueToSetHelper.DeriveValueToSet(ref aspFor, _populateWithCurrentValue, complexModel, _viewData!, out var errorMessages);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(subModelItemValue);
                errorMessages.Should().HaveCount(0);
            }
        }

        [Test]
        public void DeriveValueToSet_returns_derived_simple_model_value()
        {
            // Given
            const string modelItemValue = "Description of framework.";
            var simpleModel = new { Description = modelItemValue };

            var aspFor = "Description";

            // When
            var result = ViewComponentValueToSetHelper.DeriveValueToSet(ref aspFor, _populateWithCurrentValue, simpleModel, _viewData!, out var errorMessages);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(modelItemValue);
                errorMessages.Should().HaveCount(0);
            }
        }

    }
}
