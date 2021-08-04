namespace DigitalLearningSolutions.Data.Enums
{
    public class ReportInterval : Enumeration
    {
        public static readonly ReportInterval Days = new ReportInterval(0, nameof(Days));
        public static readonly ReportInterval Weeks = new ReportInterval(1, nameof(Weeks));
        public static readonly ReportInterval Months = new ReportInterval(2, nameof(Months));
        public static readonly ReportInterval Quarters = new ReportInterval(3, nameof(Quarters));
        public static readonly ReportInterval Years = new ReportInterval(4, nameof(Years));

        public ReportInterval(int id, string name) : base(id, name)
        { }
    }
}
