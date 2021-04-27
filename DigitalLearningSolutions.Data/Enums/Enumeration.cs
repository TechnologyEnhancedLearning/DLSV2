namespace DigitalLearningSolutions.Data.Enums
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Enumeration base class as recommended at https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
    /// </summary>
    public abstract class Enumeration
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public |
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

            if (!(obj is Enumeration))
            {
                return false;
            }

            var typeMatches = this.GetType() == obj.GetType();
            var valueMatches = Id.Equals(((Enumeration) obj).Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    }
}
