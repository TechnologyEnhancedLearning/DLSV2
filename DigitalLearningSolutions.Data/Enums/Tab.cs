namespace DigitalLearningSolutions.Data.Enums
{
    using System;
    using System.ComponentModel;

    [TypeConverter(typeof(EnumerationTypeConverter<RegistrationType>))]
    public class Tab : Enumeration
    {
        public static readonly Tab Welcome = new Tab(
            0,
            nameof(Welcome),
            "Welcome"
        );

        public static readonly Tab FindYourCentre = new Tab(
            0,
            nameof(FindYourCentre),
            "Find your centre"
        );

        public static readonly Tab Pricing = new Tab(
            0,
            nameof(Pricing),
            "Pricing"
        );

        public static readonly Tab MyAccount = new Tab(
            0,
            nameof(MyAccount),
            "My account"
        );

        public static readonly Tab LogIn = new Tab(
                    0,
                    nameof(LogIn),
                    "Log in"
                );

        public static readonly Tab Register = new Tab(
            0,
            nameof(Register),
            "Register"
        );

        public static readonly Tab SwitchApplication = new Tab(
            0,
            nameof(SwitchApplication),
            "Switch application"
        );

        public readonly string DisplayText;

        private Tab(int id, string name, string displayText) : base(id, name)
        {
            DisplayText = displayText;
        }

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
