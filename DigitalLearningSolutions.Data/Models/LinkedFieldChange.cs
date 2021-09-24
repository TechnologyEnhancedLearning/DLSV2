namespace DigitalLearningSolutions.Data.Models
{
    public class LinkedFieldChange
    {
        public LinkedFieldChange(int linkedFieldNumber, string? oldValue, string? newValue)
        {
            LinkedFieldNumber = linkedFieldNumber;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public int LinkedFieldNumber { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
