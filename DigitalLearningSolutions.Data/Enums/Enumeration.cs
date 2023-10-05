namespace DigitalLearningSolutions.Data.Enums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Enumeration base class as recommended at https://ankitvijay.net/2020/05/21/introduction-enumeration-class/
    /// and https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
    /// </summary>
    public abstract class Enumeration
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<T>();

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Enumeration objAsEnumeration))
            {
                return false;
            }

            var typeMatches = this.GetType() == obj.GetType();
            var valueMatches = Id.Equals(objAsEnumeration.Id);

            return typeMatches && valueMatches;
        }

        public static bool TryGetFromIdOrName<T>(
            string idOrName,
            out T? enumeration,
            bool ignoreCase = false
        )
            where T : Enumeration
        {
            var comparison =
                ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            return TryParse(item => string.Equals(item.Name, idOrName, comparison), out enumeration) ||
                   int.TryParse(idOrName, out var id) &&
                   TryParse(item => item.Id == id, out enumeration);
        }

        protected static bool TryParse<TEnumeration>(
            Func<TEnumeration, bool> predicate,
            out TEnumeration? enumeration)
            where TEnumeration : Enumeration
        {
            enumeration = GetAll<TEnumeration>().FirstOrDefault(predicate);
            return enumeration != null;
        }

        public static T FromId<T>(int id) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(id, "nameOrValue", item => item.Id == id);
            return matchingItem;
        }

        public static T FromName<T>(string name) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(name, "name", item => item.Name == name);
            return matchingItem;
        }

        private static TEnumeration Parse<TEnumeration, TIntOrString>(
            TIntOrString nameOrValue,
            string description,
            Func<TEnumeration, bool> predicate)
            where TEnumeration : Enumeration
        {
            var matchingItem = GetAll<TEnumeration>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                throw new InvalidOperationException(
                    $"'{nameOrValue}' is not a valid {description} in {typeof(TEnumeration)}");
            }

            return matchingItem;
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    }

    public class EnumerationTypeConverter<T> : TypeConverter where T : Enumeration
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type? sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType!);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
        {
            return value is string casted
                ? Enumeration.FromName<T>(casted)
                : base.ConvertFrom(context, culture, value!);
        }

        public override object? ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type? destinationType
        )
        {
            return destinationType == typeof(string) && value is Enumeration casted
                ? casted.Name
                : base.ConvertTo(context, culture, value, destinationType!);
        }
    }
}
