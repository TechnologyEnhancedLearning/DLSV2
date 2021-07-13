namespace DigitalLearningSolutions.Web.Models.Enums
{
    using DigitalLearningSolutions.Data.Enums;

    public class Period : Enumeration
    {
        public static readonly Period Week = new Period(7, nameof(Week));
        public static readonly Period Fortnight = new Period(14, nameof(Fortnight));
        public static readonly Period Month = new Period(30, nameof(Month));
        public static readonly Period Quarter = new Period(91, nameof(Quarter));
        public static readonly Period Year = new Period(365, nameof(Year));

        private Period(int id, string name) : base(id, name)
        {
        }
    }
}
