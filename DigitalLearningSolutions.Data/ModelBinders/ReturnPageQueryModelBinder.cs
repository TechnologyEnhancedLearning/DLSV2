namespace DigitalLearningSolutions.Data.ModelBinders
{
    using System;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

    public class ReturnPageQueryModelBinder : IModelBinder
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
                bindingContext.Result = ModelBindingResult.Success(new ReturnPageQuery(1, ""));
            }
            else if (ReturnPageQuery.TryGetFromFormData(enumerationName.FirstValue, out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
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

    public class ReturnPageQueryModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(ReturnPageQuery))
            {
                return new BinderTypeModelBinder(typeof(ReturnPageQueryModelBinder));
            }

            return null;
        }
    }
}
