namespace DigitalLearningSolutions.Data.Enums
{
    using System;
    using System.ComponentModel;

    [TypeConverter(typeof(EnumerationTypeConverter<CompletionStatus>))]
    public class CompletionStatus : Enumeration
    {
        public static readonly CompletionStatus Complete = new CompletionStatus(
            0,
            nameof(Complete),
            "Complete"
        );

        public static readonly CompletionStatus Incomplete = new CompletionStatus(
            0,
            nameof(Incomplete),
            "Incomplete"
        );

        public static readonly CompletionStatus Removed = new CompletionStatus(
            0,
            nameof(Removed),
            "Removed"
        );

        public readonly string DisplayText;

        private CompletionStatus(int id, string name, string displayText) : base(id, name)
        {
            DisplayText = displayText;
        }

        public static implicit operator CompletionStatus(string value)
        {
            try
            {
                return FromName<CompletionStatus>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public static implicit operator string(CompletionStatus completionStatus)
        {
            return completionStatus.Name;
        }
    }
}
