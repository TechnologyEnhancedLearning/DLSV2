using System;

namespace DigitalLearningSolutions.Data.Exceptions
{
    public class DeleteUserException : Exception
    {
        public DeleteUserException(string message)
            : base(message)
        {
        }
    }
}
