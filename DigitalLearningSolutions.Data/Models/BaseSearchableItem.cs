namespace DigitalLearningSolutions.Data.Models
{
    public abstract class BaseSearchableItem
    {
        public string? SearchableNameValue;

        public abstract string SearchableName { get; set; }
    }
}
