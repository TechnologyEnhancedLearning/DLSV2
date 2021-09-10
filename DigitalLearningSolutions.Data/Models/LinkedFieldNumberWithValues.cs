namespace DigitalLearningSolutions.Data.Models
{
    public class LinkedFieldNumberWithValues
    {
        public LinkedFieldNumberWithValues(int linkedFieldNumber, string? oldValue, string? newValue)
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
