namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class AdminRoleFullException : Exception
    {
        public AdminRoleFullException(string message) : base(message) { }
    }
}
