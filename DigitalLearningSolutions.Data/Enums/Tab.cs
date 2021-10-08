namespace DigitalLearningSolutions.Data.Enums
{
    using System;
    using System.ComponentModel;

    [TypeConverter(typeof(EnumerationTypeConverter<Tab>))]
    public class Tab : Enumeration
    {
        public static readonly Tab Welcome = new Tab(
            0,
            nameof(Welcome)
        );

        public static readonly Tab FindYourCentre = new Tab(
            1,
            nameof(FindYourCentre)
        );

        public static readonly Tab Pricing = new Tab(
            2,
            nameof(Pricing)
        );

        public static readonly Tab MyAccount = new Tab(
            3,
            nameof(MyAccount)
        );

        public static readonly Tab LogIn = new Tab(
            4,
            nameof(LogIn)
        );

        public static readonly Tab Register = new Tab(
            5,
            nameof(Register)
        );

        public static readonly Tab SwitchApplication = new Tab(
            6,
            nameof(SwitchApplication)
        );

        private Tab(int id, string name) : base(id, name) { }

        public static implicit operator Tab(string value)
        {
            try
            {
                return FromName<Tab>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public static implicit operator string(Tab tab)
        {
            return tab.Name;
        }
    }
}
