namespace DigitalLearningSolutions.Web.ModelBinders
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Model binder to allow use of Enumeration classes as endpoint parameters.
    /// Inspired by https://ankitvijay.net/2020/06/14/enumeration-class-query-string/
    /// </summary>
    /// <typeparam name="T">The enumeration class being bound to.</typeparam>
    public class EnumerationQueryStringModelBinder<T> : IModelBinder where T : Enumeration
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
                bindingContext.Result = ModelBindingResult.Success(default(T));
            }
            else if (Enumeration.TryGetFromIdOrName<T>(enumerationName.FirstValue, out var result, true))
            {
                bindingContext.Result = ModelBindingResult.Success(result!);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();

                bindingContext.ModelState.AddModelError(
                    bindingContext.FieldName,
                    $"{enumerationName.FirstValue} is not supported.");
            }

            return Task.CompletedTask;
        }
    }

    public static class EnumerationQueryStringModelBinder
    {
        public static EnumerationQueryStringModelBinder<T> CreateInstance<T>() where T : Enumeration
        {
            return new EnumerationQueryStringModelBinder<T>();
        }
    }

    public class EnumerationQueryStringModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var fullyQualifiedAssemblyName = context.Metadata.ModelType.FullName;

            if (fullyQualifiedAssemblyName == null)
            {
                return null;
            }

            var enumType = context.Metadata.ModelType.Assembly.GetType(fullyQualifiedAssemblyName, false);

            if (enumType == null || !enumType.IsSubclassOf(typeof(Enumeration)))
            {
                return null;
            }

            var methodInfo = typeof(EnumerationQueryStringModelBinder)
                .GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.Public);

            if (methodInfo == null)
            {
                throw new InvalidOperationException("Invalid operation");
            }

            var genericMethod = methodInfo.MakeGenericMethod(enumType);
            var invoke = genericMethod.Invoke(null, null);

            return invoke as IModelBinder;
        }
    }
}
