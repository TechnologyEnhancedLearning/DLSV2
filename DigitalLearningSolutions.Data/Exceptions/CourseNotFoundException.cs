namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class CourseNotFoundException : Exception
    {
        public CourseNotFoundException(string message) : base(message) { }

        public CourseNotFoundException() { }
    }
}
