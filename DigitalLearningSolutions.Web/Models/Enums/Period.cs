namespace DigitalLearningSolutions.Web.Models.Enums
{
    using DigitalLearningSolutions.Data.Enums;

    public class Period : Enumeration
    {
        public static readonly Period Week = new Period(0, nameof(Week), 7);
        public static readonly Period Fortnight = new Period(1, nameof(Fortnight), 14);
        public static readonly Period Month = new Period(2, nameof(Month), 30);
        public static readonly Period Quarter = new Period(3, nameof(Quarter), 91);
        public static readonly Period Year = new Period(4, nameof(Year), 365);

        public int Days { get; set; }

        private Period(int id, string name, int numberOfDays) : base(id, name)
        {
            Days = numberOfDays;
        }
    }
}
