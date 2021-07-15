namespace DigitalLearningSolutions.Web.Models.Enums
{
    using DigitalLearningSolutions.Data.Enums;

    public class Period : Enumeration
    {
        public static readonly Period Week = new Period(0, nameof(Week));
        public static readonly Period Fortnight = new Period(1, nameof(Fortnight));
        public static readonly Period Month = new Period(2, nameof(Month));
        public static readonly Period Quarter = new Period(3, nameof(Quarter));
        public static readonly Period Year = new Period(4, nameof(Year));

        public int Days { get; set; }

        private Period(int id, string name) : base(id, name)
        {
            var validDayIntervals = new[] { 7, 14, 30, 91, 365 };
            Days = validDayIntervals[id];
        }
    }
}
