namespace DigitalLearningSolutions.Data.Models
{
    public class LinkedFieldChange
    {
        public LinkedFieldChange(int linkedFieldNumber, string linkedFieldName, string? oldValue, string? newValue)
        {
            LinkedFieldNumber = linkedFieldNumber;
            LinkedFieldName = linkedFieldName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public int LinkedFieldNumber { get; set; }
        public string LinkedFieldName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
