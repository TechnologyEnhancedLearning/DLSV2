namespace DigitalLearningSolutions.Data.Models
{
    public abstract class BaseSearchableItem
    {
        internal string? SearchableNameValue;

        public abstract string SearchableName { get; set; }
    }
}
