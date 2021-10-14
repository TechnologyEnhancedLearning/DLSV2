namespace DigitalLearningSolutions.Web.ModelBinders
{
    using System;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

    public class DlsSubApplicationModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var enumerationName = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
            if (string.IsNullOrEmpty(enumerationName.FirstValue))
            {
                bindingContext.Result = ModelBindingResult.Success(DlsSubApplication.Default);
            }
            else if (DlsSubApplication.TryGetFromUrlSegment(enumerationName.FirstValue, out var result, true))
            {
                bindingContext.Result = ModelBindingResult.Success(result!);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();

                bindingContext.ModelState.AddModelError(
                    bindingContext.FieldName,
                    $"{enumerationName.FirstValue} is not supported."
                );
            }

            return Task.CompletedTask;
        }
    }

    public class DlsSubApplicationModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(DlsSubApplication))
            {
                return new BinderTypeModelBinder(typeof(DlsSubApplicationModelBinder));
            }

            return null;
        }
    }
}
