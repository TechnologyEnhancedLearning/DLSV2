namespace DigitalLearningSolutions.Data.Enums
{
    using System;
    using System.ComponentModel;

    [TypeConverter(typeof(EnumerationTypeConverter<NavMenuTab>))]
    public class NavMenuTab : Enumeration
    {
        public static readonly NavMenuTab Welcome = new NavMenuTab(
            0,
            nameof(Welcome)
        );

        public static readonly NavMenuTab FindYourCentre = new NavMenuTab(
            1,
            nameof(FindYourCentre)
        );

        public static readonly NavMenuTab Pricing = new NavMenuTab(
            2,
            nameof(Pricing)
        );

        public static readonly NavMenuTab MyAccount = new NavMenuTab(
            3,
            nameof(MyAccount)
        );

        public static readonly NavMenuTab LogIn = new NavMenuTab(
            4,
            nameof(LogIn)
        );

        public static readonly NavMenuTab Register = new NavMenuTab(
            5,
            nameof(Register)
        );

        public static readonly NavMenuTab SwitchApplication = new NavMenuTab(
            6,
            nameof(SwitchApplication)
        );

        public static readonly NavMenuTab CourseSetup = new NavMenuTab(
            7,
            nameof(CourseSetup)
        );

        private NavMenuTab(int id, string name) : base(id, name) { }

        public static implicit operator NavMenuTab(string value)
        {
            try
            {
                return FromName<NavMenuTab>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public static implicit operator string(NavMenuTab tab)
        {
            return tab.Name;
        }
    }
}
