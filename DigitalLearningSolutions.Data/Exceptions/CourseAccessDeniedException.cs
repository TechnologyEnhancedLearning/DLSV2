namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class CourseAccessDeniedException : Exception
    {
        public CourseAccessDeniedException(string message) : base(message) { }

        public CourseAccessDeniedException() { }
    }
}
